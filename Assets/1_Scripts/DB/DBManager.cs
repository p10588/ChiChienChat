using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ChiChien.DB
{
    [System.Serializable]
    public class UserData {
        public string uid;
        public string email;
        public string nickName;
        public string friendShipId;
        public UserData(string uid, string email, string nickName, string friendShipId) {
            this.uid = uid;
            this.email = email;
            this.nickName = nickName;
            this.friendShipId = friendShipId;
        }
    }

    [System.Serializable]
    public class FriendData {
        public UserData userData;
        public bool isAccept;
        public string messageId;
        public bool isNew;
        public UserData sender;
        public FriendData(UserData userData, bool isAccept, string messageId, bool isNew, UserData senderUid) {
            this.userData = userData;
            this.isAccept = isAccept;
            this.messageId = messageId;
            this.isNew = isNew;
            this.sender = senderUid;
        }
    }

    public class FriendList
    {
        public Dictionary<string, FriendData> friendList;
    }

    [System.Serializable]
    public class MessageData {
        public string sender;
        public string receiver;
        public string content;
        //public bool isRead;
        public MessageData(string sender, string receiver, string content) {
            this.sender = sender;
            this.receiver = receiver;
            this.content = content;
        }
    }

    public class DBManager {

        public UserDB userDB { get; private set; }
        public FriendDB friendDB { get; private set; }
        public MessageDB messageDB { get; private set; }
        public OnlineStateDB onlineState { get; private set; }
        //public UserDataList userDataList

        private DatabaseReference dbRootRef;

        public DBManager() {
            this.dbRootRef = FirebaseDatabase.DefaultInstance.RootReference;
            this.userDB = new UserDB(dbRootRef);
            this.friendDB = new FriendDB(dbRootRef);
            this.messageDB = new MessageDB(dbRootRef);
            this.onlineState = new OnlineStateDB(dbRootRef);
        }

        public async Task<List<UserData>> QueryAllUserData(string keywords) {
            // Read the data from the database
            Task<DataSnapshot> task = this.dbRootRef.Child("UserData").GetValueAsync();
            try {
                DataSnapshot snapshot = await task;
                if (snapshot != null) {
                    Dictionary<string, object> test = (Dictionary<string, object>)snapshot.Value;

                    List<UserData> resultList = new List<UserData>();

                    foreach (KeyValuePair<string, object> user in test) {
                        Dictionary<string, object> obj = (Dictionary<string, object>)user.Value;
                        if (obj.TryGetValue("nickName", out object nickName)) {
                            string n = ((string)nickName).ToUpper();
                            if (n.Contains(keywords.ToUpper()) && ((string)nickName) != userDB.userData.nickName) {
                                Debug.Log((string)obj["nickName"]);
                                resultList.Add(new UserData((string)obj["uid"], (string)obj["email"],
                                                               (string)obj["nickName"], (string)obj["friendShipId"]));
                            }
                        }
                    }
                    return resultList;
                }
            } catch (AggregateException e) {
                Debug.Log(e);
                return null;
            }
            return null;
        }

    }

    public class MessageDB{
        private DatabaseReference messageDBRef;
        private const string CHILD_NAME = "messageData";
        public Dictionary<string, MessageData> userMessageList { get; private set; }

        public MessageDB(DatabaseReference dbRootRef) {
            this.messageDBRef = dbRootRef.Child(CHILD_NAME);
        }

        public async Task<Dictionary<string, MessageData>> RetriveData(string uid,
                                                                         Action<Dictionary<string, MessageData>> callback = null,
                                                                         Action<Dictionary<string, MessageData>> fallback = null) {
            this.userMessageList
                = await FirebaseDB.GetData<Dictionary<string, MessageData>>(
                    this.messageDBRef.Child(uid).Child("messageList"), callback, fallback
                );

            Debug.Log("Retrive MessageList" + userMessageList);
            return this.userMessageList;
        }

        public async void AddData(string messageId, MessageData message) {
            string messageJson = JsonConvert.SerializeObject(message);
            await this.messageDBRef.Child(messageId).Child("messageList").Push().SetRawJsonValueAsync(messageJson);
        }

        public void RegistorMessageListListener(string messageId, EventHandler<ChildChangedEventArgs> callback) {
            this.messageDBRef.Child(messageId).Child("messageList").ChildAdded += callback;
        }

        public void UnregistorMessageListListener(string messageId, EventHandler<ChildChangedEventArgs> callback) {
            this.messageDBRef.Child(messageId).Child("messageList").ChildAdded -= callback;
        }

        public async void RegistorNewMessageListener(string messageId, EventHandler<ChildChangedEventArgs> callback) {
            Query query = this.messageDBRef.Child(messageId).Child("messageList").OrderByKey().LimitToLast(1);
            DataSnapshot snapshot = await query.GetValueAsync();
            string key = string.Empty;
            if (snapshot != null) {
                Dictionary<string, object> lastValue = (Dictionary<string, object>)snapshot.Value;
                key = (lastValue.Keys.ToList())[0];
            }
            if (!string.IsNullOrEmpty(key)) {
                this.messageDBRef.Child(messageId).Child("messageList").OrderByKey().StartAt(key).ChildAdded += callback;
            }
        }

        public async void UnregistorNewMessageListener(string messageId, EventHandler<ChildChangedEventArgs> callback) {
            Query query = this.messageDBRef.Child(messageId).Child("messageList").OrderByKey().LimitToLast(1);
            DatabaseReference lastRef = query.Reference;
            string key = await FirebaseDB.GetData<string>(lastRef);
            this.messageDBRef.Child(messageId).Child("messageList").OrderByKey().StartAt(key).ChildAdded -= callback;
        }
    }

    public class FriendDB {
        private DatabaseReference friendDBRef;
        private const string CHILD_NAME = "FriendData";
        public FriendList userFriendList { get; private set; }

        public FriendDB(DatabaseReference dbRootRef) {
            this.friendDBRef = dbRootRef.Child(CHILD_NAME);
        }

        public async Task<FriendList> RetriveData(string uid, Action<FriendList> callback = null) {
            this.userFriendList = await FirebaseDB.GetData<FriendList>(this.friendDBRef.Child(uid), callback, null);
            Debug.Log("Retrive FriendList" + userFriendList.friendList);
            return this.userFriendList;
        }

        public async void CreateNewFriendListData(string uid, Action<FriendList> action = null) {
            Dictionary<string, FriendData> friendDatas = new Dictionary<string, FriendData>();
            string defaultNickName = "DefaultNickName";
            UserData defaultUser = new UserData("Default", "Default", defaultNickName, "Default");
            friendDatas.Add(defaultNickName,
                            new FriendData(defaultUser, true, "Default", false, defaultUser));

            FriendList friendList = new FriendList {
                friendList = friendDatas
            };

            await Task.Run(() => FirebaseDB.UpdateData<FriendList>(this.friendDBRef.Child(uid), friendList, (friendList) => this.userFriendList = friendList));
            action?.Invoke(friendList);
        }

        public async void UpdateDataToFreindList(string friendshipId, FriendData friendData,
                                                 Action<Dictionary<string, FriendData>> action = null) {
            FriendList friendListTemp = await RetriveData(friendshipId);
            friendListTemp.friendList[friendData.userData.nickName] = friendData;
            DatabaseReference dbRef = this.friendDBRef.Child(friendshipId).Child("friendList");
            await Task.Run(() => FirebaseDB.UpdateData<Dictionary<string, FriendData>>(dbRef, this.userFriendList.friendList, action));
        }

        public async void AddDataToFreindList(string friendshipId, FriendData friendData,
                                              Action<Dictionary<string, FriendData>> action = null) {
            FriendList friendListTemp = await RetriveData(friendshipId);
            friendListTemp.friendList.Add(friendData.userData.nickName, friendData);
            DatabaseReference dbRef = this.friendDBRef.Child(friendshipId).Child("friendList");
            await Task.Run(() => FirebaseDB.UpdateData<Dictionary<string, FriendData>>(dbRef, this.userFriendList.friendList, action));
        }

        public void RegistorFriendListListener(string friendshipId, Action<FriendList, string> callback,
                                               Action<AggregateException> fallback) {
            void CurrentListener(object o, ValueChangedEventArgs args) {
                if (args.DatabaseError != null) fallback(new AggregateException(new Exception(args.DatabaseError.Message)));
                else
                    callback(JsonUtility.FromJson<FriendList>(args.Snapshot.GetRawJsonValue()) as FriendList,
                             args.Snapshot.Key);
            }
            this.friendDBRef.Child(friendshipId).Child("friendList").ValueChanged += CurrentListener;
        }

        public void RemoveFriendData(string friendshipId, string nickName) {
            DatabaseReference dbRef = this.friendDBRef.Child(friendshipId).Child("friendList").Child(nickName);
            FirebaseDB.RemoveData(dbRef);
        }

    }

    public class UserDB{
        private DatabaseReference userDBRef;
        private const string CHILD_NAME = "UserData";
        public UserData userData { get; private set; }

        public UserDB(DatabaseReference dbRootRef) {
            this.userDBRef = dbRootRef.Child(CHILD_NAME);
        }

        public async Task<UserData> RetriveData(string uid, Action<UserData> callback = null) {
            this.userData = await FirebaseDB.GetData<UserData>(this.userDBRef.Child(uid), callback, null);
            Debug.Log("Retrive UserDataDB" + userData);
            return this.userData;
        }

        public async void CreateNewUserData(string email, string uid, string nickName, string friendID,
                                                   Action<UserData> action = null) {
            UserData userData = new UserData(uid, email, nickName, friendID);
            await Task.Run(() => WriteData<UserData>(uid, userData));
            Debug.Log("Cache UserData");
            this.userData = userData;
        }

        public async void WriteData<UserData>(string uid, UserData obj, Action<UserData> action = null)
            => await Task.Run(() => FirebaseDB.UpdateData<UserData>(this.userDBRef.Child(uid), obj, action));

    }

    public class OnlineStateDB { 
        private DatabaseReference onlineDBRef;
        private const string CHILD_NAME = "OnlineStateData";

        public OnlineStateDB(DatabaseReference dbRootRef) {
            this.onlineDBRef = dbRootRef.Child(CHILD_NAME);
        }

        public async void UpdateOnlineState(string uid, bool online) {
            await Task.Run(() => FirebaseDB.UpdateData<bool>(this.onlineDBRef.Child(uid), online, null));
        }
        public void RegistorOnlineStateListener(string uid, EventHandler<ChildChangedEventArgs> callback) {
            this.onlineDBRef.Child(uid).ChildChanged += callback;
        }
        public void UnregistorOnlineStateListener(string uid, EventHandler<ChildChangedEventArgs> callback) {
            this.onlineDBRef.Child(uid).ChildChanged -= callback;
        }
    }

}



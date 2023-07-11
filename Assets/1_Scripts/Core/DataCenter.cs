using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChiChien.DB;
using Firebase.Database;
using UnityEngine;

namespace ChiChien.Core
{
    
    public class DataCenter : Singleton<DataCenter>
    {
        private DBManager dbMgr;

        public UserData userData => this.dbMgr.userDB.userData;
        public FriendList friendData => this.dbMgr.friendDB.userFriendList;
        public FriendData curMessageFriend { get; private set; }

        protected override void Awake() {
            base.Awake();
            this.dbMgr = new DBManager();
        }

        protected override void Start() {
        }

        private void OnApplicationQuit() {
            Task.WaitAll(Task.Run(()=>this.UpdateSelfOnlineState(false)));
        }

        public async void InitalDBData(string uid, Action finishCallback) {
            await this.dbMgr.userDB.RetriveData(uid);
            await this.dbMgr.friendDB.RetriveData(((UserData)userData).friendShipId);
            finishCallback.Invoke();
        }

        public async void CreateNewUserData(string email, string uid, string nickName, string friendID,
                                      Action<UserData> callback = null) {
            await Task.Run(() => this.dbMgr.userDB.CreateNewUserData(email, uid, nickName, friendID, callback));
            Debug.Log("Create NewUser Data");
        }

        public async void CreateFriendData(string uid, Action<FriendList> callback) {
             await Task.Run(()=>this.dbMgr.friendDB.CreateNewFriendListData(uid, callback));
            Debug.Log("Create FriendData");
        }

        public void CreateMessageData(string messageId) {
            this.dbMgr.messageDB.AddData(messageId, new MessageData("Default", "Default", "Welcome"));
        }

        public void UpdateUserFriendData(FriendData friendData) {
            this.dbMgr.friendDB.UpdateDataToFreindList(userData.friendShipId, friendData);
        }

        public void UpdateOtherUserFriendData(string otherFriendshipId, FriendData friendData) {
            this.dbMgr.friendDB.UpdateDataToFreindList(otherFriendshipId, friendData);
        }

        public void AddUserFriendData(FriendData friendData) {
            this.dbMgr.friendDB.AddDataToFreindList(userData.friendShipId, friendData);
        }

        public void RemoveFriendData(string otherFriendshipId, string nickName) 
            => this.dbMgr.friendDB.RemoveFriendData(otherFriendshipId, nickName);

        public void AddOtherUserFriendData(string otherFriendshipId, FriendData friendData) {
            this.dbMgr.friendDB.AddDataToFreindList(otherFriendshipId, friendData);
        }

        public async Task<FriendList> GetOtherFriendList(string friendShipId) {
            return await this.dbMgr.friendDB.RetriveData(friendShipId);
        }

        public async Task<FriendList> SyncFriendListFromDB() {
            FriendList friendList = await dbMgr.friendDB.RetriveData(userData.friendShipId);
            return friendList;
        }

        public async Task<List<UserData>> QueryAllUserDataNickName(string context)
            => await dbMgr.QueryAllUserData(context);

        public void RegisitorFriendDBListener(Action<FriendList, string> callback,
                                              Action<AggregateException> fallback) {
            if (userData == null) {
                Debug.LogError("Listener UserData Null");
                return;
            }
            this.dbMgr.friendDB.RegistorFriendListListener(userData.friendShipId, callback, fallback);
        }

        public void RegisitorMessageListener(string messageId,
                                             EventHandler<ChildChangedEventArgs> @event) {
            this.dbMgr.messageDB.RegistorMessageListListener(messageId, @event);
        }

        public void UnregisitorMessageListener(string messageId,
                                               EventHandler<ChildChangedEventArgs> @event) {
            this.dbMgr.messageDB.UnregistorMessageListListener(messageId, @event);
        }

        public void RegisitorNewMessageListener(string messageId,
                                             EventHandler<ChildChangedEventArgs> @event) 
            => this.dbMgr.messageDB.RegistorNewMessageListener(messageId, @event);
        

        public void UnregisitorNewMessageListener(string messageId,
                                               EventHandler<ChildChangedEventArgs> @event) 
            => this.dbMgr.messageDB.UnregistorNewMessageListener(messageId, @event);
        

        public async Task<Dictionary<string, MessageData>> GetMessages(string messageId) {
            Debug.Log("Get Message List");
            return await this.dbMgr.messageDB.RetriveData(messageId);
        }

        public void SetCurMessageFriend(FriendData freind) {
            this.curMessageFriend = freind;
        }

        public async void AddMessageDataToDB(MessageData message) {
            await Task.Run(()=>this.dbMgr.messageDB.AddData(this.curMessageFriend.messageId, message));
        }

        public void UpdateSelfOnlineState(bool online) {
            this.dbMgr.onlineState.UpdateOnlineState(userData.uid, online);
        }

        public void RegisitorOnlineStateListener(string uid, EventHandler<ChildChangedEventArgs> @event) {
            this.dbMgr.onlineState.RegistorOnlineStateListener(uid, @event);
        }
        public void UnregisitorOnlineStateListener(string messageId, EventHandler<ChildChangedEventArgs> @event) {
            this.dbMgr.onlineState.UnregistorOnlineStateListener(messageId, @event);
        }
    }
}


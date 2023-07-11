using System;
using System.Collections;
using System.Collections.Generic;
using ChiChien.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace ChiChien.UI
{

    public class FriendTabView : MonoBehaviour
    {
        public GameObject friendItem;
        public Transform itemRoot;
        private List<FriendItem> itemList = new List<FriendItem>();
        private List<DB.FriendData> allFriends = new List<DB.FriendData>();

        private void GenerateAddFriendItem(List<DB.FriendData> friends) {
            ClearAllItem();
            if (friends.Count <= 0) return;
            for(int i = 0; i < friends.Count; i++) {
                FriendItem item = friendItem.Spawn(itemRoot).GetComponent<FriendItem>();
                item.Initalize(friends[i], OpenMessage);
                itemList.Add(item);
                RegistiorListener(item);
            }
        }

        private void RegistiorListener(FriendItem friendItem) {
            Debug.Log("Friend " + friendItem.friendData.userData.nickName + " RegistiorListener M&O");
            Core.DataCenter.Instance.RegisitorNewMessageListener(friendItem.friendData.messageId,
                                                                  friendItem.listenerEventHandler);
            Core.DataCenter.Instance.RegisitorOnlineStateListener(friendItem.friendData.userData.uid, UpdateOnlineSign);
        }
        private void UnregistiorListener(FriendItem friendItem) {
            Debug.Log("Friend " + friendItem.friendData.userData.nickName + " UnregistiorListener M&O");
            Core.DataCenter.Instance.RegisitorNewMessageListener(friendItem.friendData.messageId,
                                                                friendItem.listenerEventHandler);
            Core.DataCenter.Instance.UnregisitorOnlineStateListener(friendItem.friendData.userData.uid,
                                                                    UpdateOnlineSign);
        }

        public void ClearAllItem() {
            for (int i = 0; i < itemList.Count; i++) {
                itemList[i].gameObject.Recycle();
                UnregistiorListener(itemList[i]);
            }
            itemList.Clear();
        }

        public void OnTabViewContentUpdate(List<DB.FriendData> friends) {
            Debug.Log("Friend TabView  Update");
            this.allFriends = friends;
            if (this.gameObject.activeSelf) {
                GenerateAddFriendItem(this.allFriends);
            } else {
                //Red Dot;
            }
        }

        public void OpenMessage(DB.FriendData messageId) {
            Core.DataCenter.Instance.SetCurMessageFriend(messageId);
            Core.GameManager.Instance.ChangeProc(Core.ProcStatus.Message);
        }


        public void UpdateOnlineSign(object o, Firebase.Database.ChildChangedEventArgs args) {
            if (args.DatabaseError != null) Debug.LogError(args.DatabaseError.Message);
            else {
                bool onlineState = JsonConvert.DeserializeObject<bool>(args.Snapshot.GetRawJsonValue());
                Debug.Log("XXXX is Online");
            }
        }
    }
}

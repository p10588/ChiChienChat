using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ChiChien.UI
{
    public class InvitationTabView : MonoBehaviour
    {
        public GameObject friendRequestItem;
        public Transform itemRoot;

        private List<FriendRequestItem> itemList = new List<FriendRequestItem>();

        private List<DB.FriendData> allFriendRequests = new List<DB.FriendData>();

        private void OnEnable() {
            GenerateAddFriendItem(this.allFriendRequests);
        }

        private void OnDisable() {
            ClearAllItem();
        }

        private void GenerateAddFriendItem(List<DB.FriendData> friendList) {
            ClearAllItem();
            if (friendList.Count <= 0) return;
            for (int i = 0; i < friendList.Count; i++) {
                FriendRequestItem item = friendRequestItem.Spawn(itemRoot).GetComponent<FriendRequestItem>();
                Action<DB.FriendData> acceptAction = new Action<DB.FriendData>(AcceptFriendRequest);
                item.Initalize(friendList[i], AcceptFriendRequest, RejectFriendRequest);
                itemList.Add(item);
            }
        }

        public void ClearAllItem() {
            for (int i = 0; i < itemList.Count; i++) {
                itemList[i].gameObject.Recycle();
            }
            itemList.Clear();
        }

        public void OnTabViewContentUpdate(List<DB.FriendData> friendRequests) {
            this.allFriendRequests = friendRequests;
            if (this.gameObject.activeSelf) {
                GenerateAddFriendItem(this.allFriendRequests);
            } else {
                //Red Dot;
            }
        }

        private async void AcceptFriendRequest(DB.FriendData friendData) {
            friendData.isAccept = true;
            Core.DataCenter.Instance.UpdateUserFriendData(friendData);

            DB.FriendList friendList = await Core.DataCenter.Instance.GetOtherFriendList(friendData.sender.friendShipId);
            if(friendList.friendList.TryGetValue(Core.DataCenter.Instance.userData.nickName, out DB.FriendData friendDB)) {
                friendDB.isAccept = true;
                Core.DataCenter.Instance.UpdateOtherUserFriendData(friendData.sender.friendShipId, friendDB);
            }
        }

        private void RejectFriendRequest(DB.FriendData friendData) {
            Core.DataCenter.Instance.RemoveFriendData(Core.DataCenter.Instance.userData.friendShipId,
                                                      friendData.userData.nickName);

            Core.DataCenter.Instance.RemoveFriendData(friendData.sender.friendShipId,
                                                      Core.DataCenter.Instance.userData.nickName);


        }
    }

}

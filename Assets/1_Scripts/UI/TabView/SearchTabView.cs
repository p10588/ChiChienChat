using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiChien.Core;
using ChiChien.DB;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class SearchTabView : MonoBehaviour
    {
        public GameObject addFriendItem;
        public InputField searchInput;
        public Button searchBtn;
        public Transform itemRoot;

        private List<AddFriendItem> itemList = new List<AddFriendItem>();

        public void OnEnable() {
            searchBtn.onClick.RemoveAllListeners();
            searchBtn.onClick.AddListener(() => { OnSearchFriendBtnClick(searchInput.text); });
        }

        public void OnDisable() {
            searchBtn.onClick.RemoveAllListeners();
            ClearAllItem();
            searchInput.text = string.Empty;
        }

        private async void OnSearchFriendBtnClick(string context) {
            var result = await DataCenter.Instance.QueryAllUserDataNickName(context);

            List<UserData> searchUser = result;
            List<UserData> friendUserData = new List<UserData>();

            Dictionary<string, FriendData> friends = DataCenter.Instance.friendData.friendList;
            foreach (KeyValuePair<string, FriendData> f in friends) {
                friendUserData.Add(f.Value.userData);
            }

            var resultList = searchUser.Where(l2 =>!friendUserData.Any(l1 => l1.nickName == l2.nickName)).ToList(); ;


            GenerateAddFriendItem(resultList);
        }

        private void GenerateAddFriendItem(List<DB.UserData> searchList) {
            ClearAllItem();
            for (int i = 0; i < searchList.Count; i++) {               
                AddFriendItem item = addFriendItem.Spawn(itemRoot).GetComponent<AddFriendItem>();
                Action<AddFriendItem, UserData> action = new Action<AddFriendItem,UserData>(SendFriendRequest);
                item.Initalize(searchList[i], action);
                itemList.Add(item);
            }
        }

        public void ClearAllItem() {
            for(int i = 0; i < itemList.Count; i++) {
                itemList[i].gameObject.Recycle();
            }
            itemList.Clear();
        }

        private void SendFriendRequest(AddFriendItem item, UserData friendUserData) {
            string messageId = Guid.NewGuid().ToString();

            FriendData friendData =
                new FriendData(friendUserData, false, messageId, true, DataCenter.Instance.userData);
            DataCenter.Instance.AddUserFriendData(friendData);

            FriendData selfFriendData =
                new FriendData(DataCenter.Instance.userData, false, messageId, true, DataCenter.Instance.userData);
            DataCenter.Instance.AddOtherUserFriendData(friendData.userData.friendShipId, selfFriendData);

            DataCenter.Instance.CreateMessageData(messageId);

            Debug.Log(messageId);
            Debug.Log(selfFriendData.userData.nickName + "Add" + friendData.userData.nickName + "as Friend");
            item.gameObject.Recycle();

        }
    }


}

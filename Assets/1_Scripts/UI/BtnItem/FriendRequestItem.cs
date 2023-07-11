using System;
using System.Collections;
using System.Collections.Generic;
using ChiChien.DB;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class FriendRequestItem : MonoBehaviour
    {
        public Button acceptBtn;
        public Button rejectBtn;
        public Text requestText;

        public void Initalize(FriendData friendData, Action<DB.FriendData> acceptAction,
                              Action<DB.FriendData> rejectAction) {

            this.requestText.text = SetupRequestText(friendData);

            if(friendData.sender.uid != Core.DataCenter.Instance.userData.uid) {
                acceptBtn.gameObject.SetActive(true);
                rejectBtn.gameObject.SetActive(true);
                this.acceptBtn.onClick.AddListener(()=> { acceptAction.Invoke(friendData); });
                this.rejectBtn.onClick.AddListener(() => { rejectAction.Invoke(friendData); });
            } else {
                acceptBtn.gameObject.SetActive(false);
                rejectBtn.gameObject.SetActive(false);
                this.acceptBtn.onClick.RemoveAllListeners();
                this.rejectBtn.onClick.RemoveAllListeners();
            }
        }

        private string SetupRequestText(FriendData friendData) {
            if(friendData.sender.uid == Core.DataCenter.Instance.userData.uid) {
                return "Wait For " + friendData.userData.nickName + " Response";
            } else {
                return friendData.sender.nickName + " send you friend request";
            }
        }

    }
}

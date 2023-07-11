using System;
using System.Collections;
using System.Collections.Generic;
using ChiChien.DB;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class AddFriendItem : MonoBehaviour
    {
        public Text itemName;
        public Button AddBtn;

        public UserData userData;

        public void Initalize(UserData userData, Action<AddFriendItem, UserData> addFriendBtnEvent) {
            this.userData = userData;
            this.itemName.text = userData.nickName;
            this.AddBtn.gameObject.SetActive(true);
            this.AddBtn.onClick.AddListener(()=> { addFriendBtnEvent.Invoke(this, this.userData); });
        }

    }
}


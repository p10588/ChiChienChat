using System;
using System.Collections;
using System.Collections.Generic;
using ChiChien.Core;
using ChiChien.DB;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class FriendItem : MonoBehaviour
    {
        public Button friendBtn;
        public Text friendName;
        public EventHandler<Firebase.Database.ChildChangedEventArgs> listenerEventHandler { get; private set;}

        public FriendData friendData { get; private set; }

        public void Initalize(FriendData friendData, Action<DB.FriendData> action ) {
            this.friendData = friendData;
            this.friendName.text = this.friendData.userData.nickName;
            this.friendBtn.onClick.AddListener(() => { action.Invoke(this.friendData); });
            this.listenerEventHandler = TriggerNotification;
        }

        public void TriggerNotification(object o, Firebase.Database.ChildChangedEventArgs args) {
            if (args.DatabaseError != null) Debug.LogError(args.DatabaseError.Message);
            else {
                DB.MessageData message = JsonConvert.DeserializeObject<DB.MessageData>(args.Snapshot.GetRawJsonValue());
                if (message.sender != Core.DataCenter.Instance.userData.nickName) {
                    Core.TriggerSystem.Instance.Dispatch(TriggerCmd.TRIGGER_NOTIFICATION, message);
                }
            }
        }
    }
}


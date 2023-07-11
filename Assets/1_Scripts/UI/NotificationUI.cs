using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class NotificationUI : UIBase
    {
        public bool autoClose;
        public Text notificationText;

        public override void Initalize() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            Core.TriggerSystem.Instance.Register(Core.TriggerCmd.TRIGGER_NOTIFICATION, TriggerNotification);
        }

        public override void ShowUI() {
            if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            Invoke("CloseUI", 3);
        }
        public override void CloseUI() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
        }

        public void TriggerNotification( Core.TriggerEvent @event) {
            if (@event.Args.Length <= 0 || @event == null) return;

            DB.MessageData message = (DB.MessageData)@event.Args[0];

            if (message == null) return;

            Debug.Log(message.sender + "Open!!!!!!!!!");
            if (message.sender != Core.DataCenter.Instance.userData.nickName) {
                SetNotificationContent(message);
                ShowUI();
            }
        }

        public void SetNotificationContent(DB.MessageData message) {
            notificationText.text = message.sender + " send you a message";
        }

    }

}

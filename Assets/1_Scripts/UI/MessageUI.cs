using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI {
    public class MessageUI : UIBase
    {
        [SerializeField] private MessageItem messageItem;
        [SerializeField] private Transform itemRoot;
        [SerializeField] private Button sendBtn;
        [SerializeField] private Button backBtn;
        [SerializeField] private InputField inputField;
        [SerializeField] private Text friendName;

        private List<MessageItem> items = new List<MessageItem>();

        public override void Initalize() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            
        }

        public override void ShowUI() {
            InitalizeMessage(Core.DataCenter.Instance.curMessageFriend.messageId);
            if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            backBtn.onClick.AddListener(BackToFriendScene);
            sendBtn.onClick.AddListener(OnSendBtnClick);
            Debug.Log("Message UI Registor Message Listener");
            Core.DataCenter.Instance.RegisitorMessageListener(Core.DataCenter.Instance.curMessageFriend.messageId,
                                                              OnMessageUpdate);

        }
        public override void CloseUI() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            backBtn.onClick.RemoveListener(BackToFriendScene);
            sendBtn.onClick.RemoveListener(OnSendBtnClick);
            ClearAllItem();
            Debug.Log("Message UI Unregistor Message Listener");
            Core.DataCenter.Instance.UnregisitorMessageListener(Core.DataCenter.Instance.curMessageFriend.messageId,
                                                                OnMessageUpdate);

        }

        private void BackToFriendScene() {
            Core.GameManager.Instance.ChangeProc(Core.ProcStatus.Friend);
        }

        private async void InitalizeMessage(string messageId) {
            Dictionary<string, DB.MessageData> messages = await Core.DataCenter.Instance.GetMessages(messageId);
            //GenerateMessageItem(messages);
        }

        private void OnSendBtnClick() {
            DB.FriendDataDB friend = Core.DataCenter.Instance.curMessageFriend;
            DB.MessageData message = new DB.MessageData(Core.DataCenter.Instance.userData.nickName,
                                                            friend.userData.nickName, this.inputField.text);
            Core.DataCenter.Instance.AddMessageDataToDB(message);
            this.inputField.text = string.Empty;
        }

        private void OnMessageUpdate(object o, Firebase.Database.ChildChangedEventArgs args) {
            if (args.DatabaseError != null) Debug.LogError(args.DatabaseError.Message);
            else AddMessageItem(JsonConvert.DeserializeObject<DB.MessageData>(args.Snapshot.GetRawJsonValue()));
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }

        private void AddMessageItem(DB.MessageData message) {
            MessageItem item = messageItem.Spawn(itemRoot).GetComponent<MessageItem>();
            item.Initalize(message);
            items.Add(item);
        }

        private void GenerateMessageItem(Dictionary<string, DB.MessageData> messages) {
            ClearAllItem();
            if (messages.Count <= 0) return;
            foreach(KeyValuePair<string, DB.MessageData> m in messages) {
                AddMessageItem(m.Value);
            }
        }

        private void ClearAllItem() {
            for (int i = 0; i < items.Count; i++) {
                items[i].gameObject.Recycle();
            }
            items.Clear();
        }
    }
}

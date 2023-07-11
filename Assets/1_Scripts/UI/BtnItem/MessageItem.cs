using System.Collections;
using System.Collections.Generic;
using ChiChien.Core;
using ChiChien.DB;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class MessageItem : MonoBehaviour
    {
        [SerializeField]private Text Content;
        [SerializeField]private HorizontalLayoutGroup horizontalLayoutGroup;

        public void Initalize(MessageData messageData) {
            this.Content.text = messageData.content;
            if(messageData.sender == DataCenter.Instance.userData.nickName) {
                this.Content.text = messageData.content;
                horizontalLayoutGroup.childAlignment = TextAnchor.MiddleRight;
            } else {
                this.Content.text = messageData.sender + " : " + messageData.content;
                horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        }
    }
}

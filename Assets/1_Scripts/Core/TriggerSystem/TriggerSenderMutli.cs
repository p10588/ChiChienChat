using System;
using System.Collections.Generic;
using UnityEngine;
using ChiChien.Core;

namespace VO.LivePlus.Common
{
    public class TriggerSenderMutli : MonoBehaviour
    {
        [SerializeField] private SenderInfo[] _onEnableCmd;
        [SerializeField] private SenderInfo[] _onDisableCmd;
        private void OnEnable()
        {
            this.excute_sender(this._onEnableCmd);
        }

        private void OnDisable()
        {
            this.excute_sender(this._onDisableCmd);
        }

        private void excute_sender(SenderInfo[] senders)
        {
            if (senders == null || senders.Length == 0)
                return;

            for (int i = 0; i < senders.Length; i++)
            {
                var id = senders[i].CommandID;
                if (id == TriggerCmd.NONE)
                    continue;

                var param = senders[i].getParam();
                if (param == null)
                    TriggerSystem.Instance?.Dispatch(id, this);
                else
                    TriggerSystem.Instance?.Dispatch(id, param);
            }
        }
    }

    [System.Serializable]
    public class SenderInfo
    {
        [SerializeField] private TriggerCmd _commandID = TriggerCmd.NONE;
        [SerializeField] private string _strParam;
        [SerializeField] private Component _componentParam;

        public TriggerCmd CommandID
        {
            get { return this._commandID; }
        }
        public object getParam()
        {
            if (!string.IsNullOrEmpty(this._strParam))
                return this._strParam;
            else if (this._strParam != null)
                return this._componentParam;
            else
                return null;
        }
    }
}

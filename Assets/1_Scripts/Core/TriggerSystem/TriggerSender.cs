using System;
using System.Collections.Generic;
using UnityEngine;
using ChiChien.Core;

public class TriggerSender : MonoBehaviour
{
    [SerializeField] public string StrParam;
    [SerializeField] private TriggerCmd _onEnableCmd = TriggerCmd.NONE;
    [SerializeField] private TriggerCmd _onDisableCmd = TriggerCmd.NONE;
    [SerializeField] private TriggerCmd _sendTriggerCmd = TriggerCmd.NONE;
    private void OnEnable()
    {
        //Debug.Log(Camera.current);
        if (this._onEnableCmd == TriggerCmd.NONE)
            return;
        
        TriggerSystem.Instance?.Dispatch(this._onEnableCmd, this);
    }

    private void OnDisable()
    {
        if (this._onDisableCmd == TriggerCmd.NONE)
            return;
        //Debug.Log($"TriggerSender OnDisable : {this._onDisableCmd.ToString()}");
        TriggerSystem.Instance?.Dispatch(this._onDisableCmd, this);
    }

    public void SendTrigger(object args)
    {
        if (this._sendTriggerCmd == TriggerCmd.NONE)
            return;
        TriggerSystem.Instance?.Dispatch(this._sendTriggerCmd, args);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace ChiChien.Core
{
    public interface IProc {
        void Entry();
        void Leave();
    }

    public enum ProcStatus
    {
        None,
        Login,
        Friend,
        Message,
    }

    public class GameManager : Singleton<GameManager>
    {

        public IProc curProc { get; private set; }
        public ProcStatus CurProcStatus => curProcStatus;

        [SerializeField][ReadOnly]
        private ProcStatus curProcStatus;

        private Dictionary<ProcStatus, IProc> allProc;

        protected override void Start() {
            InitalAllProc();
        }

        public void ChangeProc(ProcStatus procStatus) {
            if (this.allProc.TryGetValue(procStatus, out IProc proc)) {
                if (this.curProc != null) curProc.Leave();
                this.curProcStatus = procStatus;
                this.curProc = proc;
                this.curProc.Entry();
            } else {
                Debug.LogError("Proc" + procStatus + "isn't been initalized");
            }
        }

        private void InitalAllProc() {
            this.allProc = new Dictionary<ProcStatus, IProc>();
            this.allProc.Add(ProcStatus.Login, new LoginProc());
            this.allProc.Add(ProcStatus.Friend, new FriendProc());
            this.allProc.Add(ProcStatus.Message, new MessageProc());
            ChangeProc(ProcStatus.Login);
        }

    }



    public class LoginProc : IProc
    {
        public void Entry() {
            Debug.Log("LoginProc Entry");
            UIManager.Instance.ShowUI(typeof(UI.LoginUI));
        }

        public void Leave() {
            Debug.Log("LoginProc Leave");
            UIManager.Instance.CloseUI(typeof(UI.LoginUI));
        }
    }

    public class FriendProc : IProc
    {
        public void Entry() {
            Debug.Log("FriendProc Entry");
            UIManager.Instance.ShowUI(typeof(UI.FriendUI));
        }

        public void Leave() {
            Debug.Log("FriendProc Leave");
            UIManager.Instance.CloseUI(typeof(UI.FriendUI));
        }
    }
    public class MessageProc : IProc
    {
        public void Entry() {
            Debug.Log("MessageProc Entry");
            UIManager.Instance.ShowUI(typeof(UI.MessageUI));

        }

        public void Leave() {
            Debug.Log("MessageProc Leave");
            UIManager.Instance.CloseUI(typeof(UI.MessageUI));
        }
    }


}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChiChien.Core;
using ChiChien.DB;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class FriendUI : UIBase {
        
        public TabGroup tabGroup;

        private FriendTabView _friendTabView;
        private InvitationTabView _invatationTabView;
        private SearchTabView _searchTabView;

        

        public override void Initalize() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            this.tabGroup.Initalize();
            this._friendTabView = tabGroup.tabViews[0].GetComponent<FriendTabView>();
            this._invatationTabView = tabGroup.tabViews[1].GetComponent<InvitationTabView>();
            this._searchTabView = tabGroup.tabViews[2].GetComponent<SearchTabView>();
        }

        public override void ShowUI() {
            if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            DataCenter.Instance.RegisitorFriendDBListener(FriendListCallback, FriendListFallback);
            Core.DataCenter.Instance.UpdateSelfOnlineState(true);
        }
        
        public void FriendListCallback(FriendList a, string b) {
            UpdateAllTabView();
        }

        public void FriendListFallback(AggregateException e) {
            Debug.LogError("FriendListFallback " + e);
        }
        
        public override void CloseUI() {
            if (this.gameObject.activeSelf) this.gameObject.SetActive(false);

        }

        private async void UpdateAllTabView() {
            Debug.Log("Update All Tab!!!!!!!!");
            List<FriendDataDB> friends = new List<FriendDataDB>();
            List<FriendDataDB> friendRequest = new List<FriendDataDB>();

            FriendList friendList = await DataCenter.Instance.SyncFriendListFromDB();
            foreach (KeyValuePair<string, FriendDataDB> friend in friendList.friendList) {
                if (friend.Value.isAccept) friends.Add(friend.Value);
                else friendRequest.Add(friend.Value);
            }
            _friendTabView.OnTabViewContentUpdate(friends);
            _invatationTabView.OnTabViewContentUpdate(friendRequest);
        }
    }

    
}

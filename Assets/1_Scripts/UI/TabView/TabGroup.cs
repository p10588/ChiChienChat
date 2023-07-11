using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChiChien.UI
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabBtns;
        public List<GameObject> tabViews;
        public Color idleColor;
        public Color hoverColor;
        public Color selectedColor;

        private TabButton selectedTabBtn;

        public void Initalize() {
            this.GetComponentsInChildren<TabButton>(tabBtns);
            for(int i = 0; i < tabBtns.Count; i++) {
                tabBtns[i].Initalize();
            }
            OnTabSelected(tabBtns[0]);
        }

        public void OnTabEnter(TabButton tabBtn)
        {
            ResetTab();
            if (selectedTabBtn == null || tabBtn != selectedTabBtn)
            {
                tabBtn.background.color = hoverColor;
            }
        }
        public void OnTabExit(TabButton tabBtn)
        {
            ResetTab();
        }
        public void OnTabSelected(TabButton tabBtn)
        {
            this.selectedTabBtn = tabBtn;
            ResetTab();
            tabBtn.background.color = selectedColor;
            int index = tabBtn.transform.GetSiblingIndex();
            for (int i=0; i<tabViews.Count; i++) {
                if (i == index) {
                    tabViews[i].SetActive(true);
                } else {
                    tabViews[i].SetActive(false);
                }
            }

        }

        public void ResetTab()
        {
            foreach (TabButton btn in tabBtns)
            {
                if (this.selectedTabBtn != null && btn == this.selectedTabBtn) { continue; }
                btn.background.color = idleColor;
            }

        }

    }

}


using System;
using System.Collections;
using System.Collections.Generic;
using ChiChien.UI;
using UnityEngine;

namespace ChiChien.Core
{

    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<Type, UIBase> allUI;


        protected override void Awake() {
            base.Awake();
            InitalizeAllUI();
        }


        protected override void Start() {}


        public void ShowUI(Type uiType) {
            if(allUI.TryGetValue(uiType, out UIBase ui)) {
                ui.ShowUI();
            } else {
                Debug.LogError("Cant find" + uiType + "On Show");
            }
        }

        public void CloseUI(Type uiType) {
            if (allUI.TryGetValue(uiType, out UIBase ui)) {
                ui.CloseUI();
            } else {
                Debug.LogError("Cant find" + uiType + "On Close" );
            }
        }

        private void InitalizeAllUI() {
            this.allUI = new Dictionary<Type, UIBase>();
            UIBase[] uiComponents = this.GetComponentsInChildren<UIBase>(true);
            for(int i=0; i < uiComponents.Length; i++) {
                allUI.Add(uiComponents[i].GetType(), uiComponents[i]);
                uiComponents[i].Initalize();
            }
        }
    }

    
}


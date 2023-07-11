using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChiChien.UI
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerExitHandler, IPointerClickHandler, IPointerEnterHandler
    {
        public TabGroup tabGroup;
        public Image background;

        public void Initalize() {
            this.background = GetComponent<Image>();
            if(this.tabGroup == null) this.tabGroup = GetComponentInParent<TabGroup>();
        }

        public void OnPointerClick(PointerEventData eventData) {
            tabGroup.OnTabSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData) {
            tabGroup.OnTabExit(this);
        }

        
    }

}

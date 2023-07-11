using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChiChien.UI {

    public abstract class UIBase : MonoBehaviour
    {
        public abstract void Initalize();
        public abstract void ShowUI();
        public abstract void CloseUI();
    }
}


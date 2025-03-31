using UnityEngine;

namespace CodeBase.UI.Services
{
    public class UIProvider : IUIProvider
    {
        public RectTransform MainUI { get; private set; }
        
        public void SetUIParent(RectTransform uiParent)
        {
            MainUI = uiParent;
        }
    }
}
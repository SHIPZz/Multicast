using UnityEngine;

namespace CodeBase.UI.Services
{
    public interface IUIProvider
    {
        RectTransform MainUI { get;  }

        void SetUIParent(RectTransform uiParent);
    }
}
using CodeBase.UI.AbstractWindow;
using TMPro;
using UnityEngine;

namespace CodeBase.UI.NoInternet
{
    public class NoInternetWindow : AbstractWindowBase
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
using CodeBase.UI.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Infrastructure.Installers
{
    public class UIInitializable : MonoBehaviour, IInitializable
    {
        [SerializeField] private RectTransform _uiParent;
        
        private IUIProvider _uiProvider;

        [Inject]
        private void Construct(IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
        }

        public void Initialize()
        {
            _uiProvider.SetUIParent(_uiParent);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Game
{
    public class WordSlot : MonoBehaviour, IDropHandler
    {
        [SerializeField] private Transform _placeHolder;
        [SerializeField] private TextMeshProUGUI _letterText;
        [SerializeField] private Image _background;
        
        private bool _isOccupied;
        private string _currentLetter;

        public bool IsOccupied => _isOccupied;
        public string CurrentLetter => _currentLetter;

        private void Awake()
        {
            _background.raycastTarget = true;
        }

        public void SetLetter(string letter)
        {
            _currentLetter = letter;
            _letterText.text = letter;
            _isOccupied = true;
        }

        public void Clear()
        {
            _currentLetter = string.Empty;
            _letterText.text = string.Empty;
            _isOccupied = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            // This method is required for IDropHandler but we handle the drop in ClusterItem
        }
    }
} 
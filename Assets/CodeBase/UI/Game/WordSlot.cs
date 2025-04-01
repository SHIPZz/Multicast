using TMPro;
using UnityEngine;

namespace CodeBase.UI.Game
{
    public class WordSlot : MonoBehaviour
    {
        [SerializeField] private Transform _placeHolder;
        [SerializeField] private TextMeshProUGUI _letterText;
        
        private bool _isOccupied;
        private string _currentLetter;

        public bool IsOccupied => _isOccupied;

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
    }
} 
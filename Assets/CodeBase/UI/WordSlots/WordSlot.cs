using TMPro;
using UnityEngine;

namespace CodeBase.UI.WordSlots
{
    public class WordSlot : MonoBehaviour
    {
        [SerializeField] private Transform _placeHolder;
        [SerializeField] private TextMeshProUGUI _letterText;

        [field: SerializeField] private bool _isOccupied;

        private char _currentLetter;

        public bool IsOccupied => _isOccupied;
        public char CurrentLetter => _currentLetter;

        public void SetLetter(char letter)
        {
            _currentLetter = letter;
            _letterText.text = letter.ToString();

            if (_currentLetter != '\0')
                _isOccupied = true;
        }

        public void Clear()
        {
            _currentLetter = '\0';
            _letterText.text = string.Empty;
            _isOccupied = false;
        }
    }
}
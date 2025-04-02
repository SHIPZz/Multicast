using CodeBase.UI.Cluster;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.WordSlots
{
    public class WordSlot : MonoBehaviour
    {
        [SerializeField] private Transform _placeHolder;
        [SerializeField] private TextMeshProUGUI _letterText;
        
        private bool _isOccupied;
        private string _currentLetter;

        public bool IsOccupied => _isOccupied;
        public string CurrentLetter => _currentLetter;
        
        //todo refactor remove
        public ClusterItem ClusterItem { get; set; }

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
    

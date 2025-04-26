using TMPro;
using UnityEngine;

namespace CodeBase.UI.WordSlots
{
    public class WordSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _letterText;

        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool IsOccupied { get; private set; }
        public char Letter { get; private set; } = '\0';

        public void Initialize(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void SetText(char letter)
        {
            if(letter == '\0')
                return;
            
            Letter = letter;
            _letterText.text = letter.ToString();
            IsOccupied = true;
        }

        public void Clear()
        {
            Letter = '\0';
            _letterText.text = string.Empty;
            IsOccupied = false;
        }
    }
}
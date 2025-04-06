using CodeBase.UI.AbstractWindow;
using TMPro;
using UnityEngine;

namespace CodeBase.UI.Levels
{
    public class LevelWindow : AbstractWindowBase
    {
        [SerializeField] private TMP_Text _levelText;

        public void SetLevelNumber(int levelNumber)
        {
            _levelText.text = $"Level {levelNumber}";
        }
    }
}
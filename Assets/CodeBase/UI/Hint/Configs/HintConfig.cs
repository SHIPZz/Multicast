using UnityEngine;

namespace CodeBase.UI.Hint.Configs
{
    [CreateAssetMenu(fileName = "HintConfig", menuName = "Gameplay/Data/HintConfig")]
    public class HintConfig : ScriptableObject
    {
        public int MaxHintsPerLevel = 3;
        
        [field: SerializeField] public int LettersToShow { get; private set; } = 2;

        [field: SerializeField] public string Text { get; private set; }
    }
}
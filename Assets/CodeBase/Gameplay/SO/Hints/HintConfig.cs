using UnityEngine;

namespace CodeBase.Gameplay.SO.Hints
{
        [CreateAssetMenu(fileName = "HintConfig", menuName = "Gameplay/Data/HintConfig")]
    public class HintConfig : ScriptableObject
    {
        [field: SerializeField] public int LettersToShow { get; private set; } = 2;
        
        [field: SerializeField] public string Text { get; private set; }
    }
}
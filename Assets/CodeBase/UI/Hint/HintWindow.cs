using System;
using CodeBase.UI.AbstractWindow;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Hint
{
    public class HintWindow : AbstractWindowBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _showHintButton;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private TextMeshProUGUI _remainingHintsText;

        public IObservable<Unit> OnCloseClicked => _closeButton.OnClickAsObservable();
        public IObservable<Unit> OnShowWordLengthClicked => _showHintButton.OnClickAsObservable();

        public void SetHintText(string text)
        {
            _hintText.text = text;
        }

        public void SetRemainingHints(int count)
        {
            _remainingHintsText.text = $"Remaining hints: {count}";
        }

        public void SetButtonsInteractable(bool interactable)
        {
            _showHintButton.interactable = interactable;
        }
    }
} 
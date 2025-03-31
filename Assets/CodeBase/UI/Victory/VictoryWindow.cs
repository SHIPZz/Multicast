using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CodeBase.UI.AbstractWindow;
using UniRx;

namespace CodeBase.UI.Victory
{
    public class VictoryWindow : AbstractWindowBase
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TextMeshProUGUI _solvedWordsText;

        public IObservable<Unit> OnNextLevelClicked => _nextLevelButton.OnClickAsObservable();
        public IObservable<Unit> OnMainMenuClicked => _mainMenuButton.OnClickAsObservable();
    }
} 
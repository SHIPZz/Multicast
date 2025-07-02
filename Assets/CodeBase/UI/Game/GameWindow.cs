using System;
using System.Collections.Generic;
using CodeBase.Data;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Cluster.Services.Factory;
using CodeBase.UI.WordCells;
using CodeBase.UI.WordCells.Services.Factory;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Game
{
    public class GameWindow : AbstractWindowBase
    {
        [SerializeField] private Transform wordCellsHolderParent;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Transform _levelTextLayout;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private Transform _clusterItemHolderParent;
        [SerializeField] private Button _validateButton;
        [SerializeField] private Button _menuButton;

        private WordCellsHolder _wordCellsHolder;
        private ClusterItemHolder _clusterItemHolder;

        public Transform WordCellsHolderParent => wordCellsHolderParent;

        public Transform ClusterItemHolderParent => _clusterItemHolderParent;

        public IObservable<Unit> OnValidateClicked => _validateButton?.OnClickAsObservable();
        public IObservable<Unit> OnHintClicked => _hintButton?.OnClickAsObservable();
        public IObservable<Unit> OnRestartClicked => _restartButton?.OnClickAsObservable();
        
        public IObservable<Unit> OnMenuClicked => _menuButton?.OnClickAsObservable();

        public void SetLevelNumber(int levelNumber)
        {
            _levelTextLayout.gameObject.SetActive(true);
            _levelText.text = $"LEVEL {levelNumber}";
        }
        
        public void HideLevelText() => _levelTextLayout.gameObject.SetActive(false);

        public void SetWordCellsHolder(WordCellsHolder wordCellsHolder)
        {
            _wordCellsHolder = wordCellsHolder;
            
            _wordCellsHolder.CreateWordSlots();
        }

        public void SetClusterItemHolder(ClusterItemHolder holder) => _clusterItemHolder = holder;

        public void CreateClusterItems(IEnumerable<ClusterModel> clusters) =>
            _clusterItemHolder.CreateClusterItems(clusters, _canvas);

        public void Cleanup()
        {
            ClearWordSlots();
            ClearClusters();
        }

        public void SetInteractableItemsActive(bool isActive)
        {
            SetClustersActive(isActive);
            SetValidateButtonActive(isActive);
            SetHintButtonActive(isActive);
            SetRestartButtonActive(isActive);
            SetMenuButtonActive(isActive);
        }

        private void ClearWordSlots() => _wordCellsHolder?.ClearSlots();

        private void ClearClusters() => _clusterItemHolder?.Clear();
        
        private void SetValidateButtonActive(bool isActive) => _validateButton?.gameObject.SetActive(isActive);
        
        private void SetRestartButtonActive(bool isActive) => _restartButton.gameObject.SetActive(isActive);

        private void SetClustersActive(bool isActive) => _clusterItemHolder?.gameObject.SetActive(isActive);
        
        public void SetHintButtonActive(bool isActive) => _hintButton?.gameObject.SetActive(isActive);
        
        private void SetMenuButtonActive(bool isActive) => _menuButton.gameObject.SetActive(isActive);
    }
}
using System;
using System.Collections.Generic;
using CodeBase.StaticData;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.WordSlots;
using CodeBase.UI.WordSlots;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Game
{
    public class GameWindow : AbstractWindowBase
    {
        [SerializeField] private Transform _wordSlotHolderParent;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _restartButton;

        [SerializeField] private Transform _clusterItemHolderParent;
        [SerializeField] private Button _validateButton;

        private WordSlotHolder _wordSlotHolder;
        private IClusterUIFactory _clusterUIFactory;
        private ClusterItemHolder _clusterItemHolder;
        private IWordSlotUIFactory _wordSlotUIFactory;

        public IObservable<Unit> OnValidateClicked => _validateButton.OnClickAsObservable();
        public IObservable<Unit> OnHintClicked => _hintButton.OnClickAsObservable();
        
        public IObservable<Unit> OnRestartClicked => _restartButton.OnClickAsObservable();

        [Inject]
        private void Construct(
            IUIStaticDataService uiStaticDataService,
            IWordSlotUIFactory wordSlotUIFactory,
            IClusterUIFactory clusterUIFactory)
        {
            _wordSlotUIFactory = wordSlotUIFactory;
            _clusterUIFactory = clusterUIFactory;
        }

        public void FillWordSlots(int row, string text)
        {
            _wordSlotHolder.FillWordSlots(row, text);
        }
        
        public void FillWordSlotsWithPositions(int row, Dictionary<int, char> slotPositions)
        {
            _wordSlotHolder.FillWordSlotsWithPositions(row, slotPositions);
        }

        public void CreateWordSlotHolder()
        {
            if (_wordSlotHolder == null)
                _wordSlotHolder = _wordSlotUIFactory.CreateWordSlotHolder(_wordSlotHolderParent);
            
            _wordSlotHolder.CreateWordSlots();
        }

        public void CreateClusterItemHolder()
        {
            if (_clusterItemHolder == null)
                _clusterItemHolder = _clusterUIFactory.CreateClusterItemHolder(_clusterItemHolderParent);

        }

        public void CreateClusterItems(IEnumerable<string> clusters)
        {
            _clusterItemHolder.CreateClusterItems(clusters, _canvas);
        }

        public void ClearWordSlots() => _wordSlotHolder?.ClearSlots();

        public void ClearClusters() => _clusterItemHolder?.Clear();

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
        }

        public void SetClustersActive(bool isActive) => _clusterItemHolder?.gameObject.SetActive(isActive);

        public void SetValidateButtonActive(bool isActive) => _validateButton?.gameObject.SetActive(isActive);

        public void SetHintButtonActive(bool isActive) => _hintButton?.gameObject.SetActive(isActive);
    }
}
using System;
using System.Collections.Generic;
using CodeBase.StaticData;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Services;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.WordSlots;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Game
{
    public class GameWindow : AbstractWindowBase
    {
        [SerializeField] private Transform _wordSlotHolderParent;

        [SerializeField] private Transform _clusterItemHolderParent;
        [SerializeField] private Button _validateButton;

        private readonly ClusterUIService _clusterUIService = new();

        private WordSlotHolder _wordSlotHolder;
        private IClusterUIFactory _clusterUIFactory;
        private ClusterItemHolder _clusterItemHolder;
        private IWordSlotUIFactory _wordSlotUIFactory;

        public IObservable<Unit> OnValidateClicked => _validateButton.OnClickAsObservable();

        public IObservable<(string clusterText, WordSlot slot)> OnClusterPlaced => _clusterUIService.OnClusterPlaced;
        public IObservable<(string clusterText, WordSlot slot)> OnClusterRemoved => _clusterUIService.OnClusterRemoved;

        [Inject]
        private void Construct(
            IUIStaticDataService uiStaticDataService,
            IWordSlotUIFactory wordSlotUIFactory,
            IClusterUIFactory clusterUIFactory)
        {
            _wordSlotUIFactory = wordSlotUIFactory;
            _clusterUIFactory = clusterUIFactory;
        }

        public void CreateWordSlotHolder(IReadOnlyList<string> words)
        {
            _wordSlotHolder = _wordSlotUIFactory.CreateWordSlotHolder(_wordSlotHolderParent);
            _wordSlotHolder.CreateWordSlots(words);
        }

        public void CreateClusterItemHolder(IEnumerable<string> clusters)
        {
            _clusterItemHolder = _clusterUIFactory.CreateClusterItemHolder(_clusterItemHolderParent);
            _clusterItemHolder.CreateClusterItems(clusters,_wordSlotHolder);
        }

        public void CreateCluster(string clusterText)
        {
            _clusterItemHolder.CreateClusterItem(clusterText,_wordSlotHolder);
        }

        public void ClearWordSlots()
        {
            _wordSlotHolder?.ClearSlots();
        }

        public void ClearClusters()
        {
            _clusterItemHolder?.Clear();
        }
    }
}
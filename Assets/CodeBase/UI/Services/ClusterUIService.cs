using System;
using CodeBase.UI.Game;
using UniRx;

namespace CodeBase.UI.Services
{
    public class ClusterUIService
    {
        private readonly Subject<(string clusterText, WordSlot slot)> _onClusterPlaced = new();
        private readonly Subject<(string clusterText, WordSlot slot)> _onClusterRemoved = new();

        public IObservable<(string clusterText, WordSlot slot)> OnClusterPlaced => _onClusterPlaced;
        public IObservable<(string clusterText, WordSlot slot)> OnClusterRemoved => _onClusterRemoved;

        public void NotifyClusterPlaced(string clusterText, WordSlot slot)
        {
            _onClusterPlaced.OnNext((clusterText, slot));
        }

        public void NotifyClusterRemoved(string clusterText, WordSlot slot)
        {
            _onClusterRemoved.OnNext((clusterText, slot));
        }
    }
} 
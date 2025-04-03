using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace CodeBase.Gameplay.Cluster
{
    public class ClusterService : IClusterService
    {
        private readonly Subject<bool> _validationResult = new();
        
        private readonly List<string> _placedClusters = new();
        private readonly List<string> _availableClusters = new();

        public IReadOnlyList<string> GetAvailableClusters() => _availableClusters;

        public IReadOnlyList<string> GetPlacedClusters() => _placedClusters;

        public IObservable<bool> OnValidationResult => _validationResult;

        public void Init(IEnumerable<string> clusters)
        {
            Cleanup();

            _availableClusters.AddRange(clusters);
        }

        public void PlaceCluster(string cluster)
        {
            if (_availableClusters.Contains(cluster))
            {
                _availableClusters.Remove(cluster);
                _placedClusters.Add(cluster);
            }
        }

        public void RemoveCluster(string cluster)
        {
            if (_placedClusters.Contains(cluster))
            {
                _placedClusters.Remove(cluster);
                _availableClusters.Add(cluster);
            }
        }

        public bool ValidateClusters(IEnumerable<string> formedWords, IEnumerable<string> wordsToFind)
        {
            if (formedWords.Count() != wordsToFind.Count())
            {
                _validationResult.OnNext(false);
                return false;
            }

            foreach (var word in formedWords)
            {
                if (!wordsToFind.Contains(word, StringComparer.OrdinalIgnoreCase))
                {
                    _validationResult.OnNext(false);
                    return false;
                }
            }

            _validationResult.OnNext(true);
            return true;
        }

        private void Cleanup()
        {
            _availableClusters.Clear();
            _placedClusters.Clear();
        }
    }
}
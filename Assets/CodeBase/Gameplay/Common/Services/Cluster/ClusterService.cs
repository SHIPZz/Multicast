using System;
using System.Collections.Generic;
using CodeBase.Extensions;
using UniRx;

namespace CodeBase.Gameplay.Common.Services.Cluster
{
    public class ClusterService :  IClusterService
    {
        private readonly Subject<Unit> _onClusterPlaced = new();
        private readonly Subject<Unit> _onClusterRemoved = new();
        private readonly Subject<bool> _onValidationResult = new();

        public IObservable<Unit> OnClusterPlaced => _onClusterPlaced;
        public IObservable<Unit> OnClusterRemoved => _onClusterRemoved;
        public IObservable<bool> OnValidationResult => _onValidationResult;
        
        public IReadOnlyList<string> GetAvailableClusters() => _availableClusters;
        
        public IReadOnlyList<string> GetCurrentWords() => _currentWords;

        public IReadOnlyList<string> GetPlacedClusters() => _placedClusters;

        private readonly List<string> _placedClusters = new();
        private readonly List<string> _availableClusters = new();
        private readonly List<string> _currentWords = new();

        public void Init(IEnumerable<string> clusters, IEnumerable<string> words)
        {
            Cleanup();

            _availableClusters.AddRange(clusters);

            foreach (string word in words)
            {
                string randomWord = words.PickRandom();
                
                _currentWords.Add(randomWord);
            }
        }

        public void Init(string[] clusters)
        {
            _availableClusters.Clear();
            _placedClusters.Clear();
            _availableClusters.AddRange(clusters);
        }

        public void PlaceCluster(string cluster)
        {
            if (_availableClusters.Contains(cluster))
            {
                _availableClusters.Remove(cluster);
                _placedClusters.Add(cluster);
                _onClusterPlaced.OnNext(Unit.Default);
            }
        }

        public void RemoveCluster(string cluster)
        {
            if (_placedClusters.Contains(cluster))
            {
                _placedClusters.Remove(cluster);
                _availableClusters.Add(cluster);
                _onClusterRemoved.OnNext(Unit.Default);
            }
        }

        public bool ValidateClusters(string[] targetWords)
        {
            string combinedClusters = string.Join("", _placedClusters);
            bool isValid = true;

            foreach (string word in targetWords)
            {
                if (!combinedClusters.Contains(word))
                {
                    isValid = false;
                    break;
                }
            }

            _onValidationResult.OnNext(isValid);
            return isValid;
        }

        private void Cleanup()
        {
            _availableClusters.Clear();
            _currentWords.Clear();
            _placedClusters.Clear();
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace CodeBase.Gameplay.Cluster
{
    public class ClusterService :  IClusterService
    {
        private const int MaxWordCount = 4;
        
        private readonly Subject<Unit> _onClusterPlaced = new();
        private readonly Subject<Unit> _onClusterRemoved = new();
        private readonly Subject<bool> _onValidationResult = new();

        public IObservable<Unit> OnClusterPlaced => _onClusterPlaced;
        public IObservable<Unit> OnClusterRemoved => _onClusterRemoved;
        public IObservable<bool> OnValidationResult => _onValidationResult;
        
        public IReadOnlyList<string> GetAvailableClusters() => _availableClusters;
        
        public IReadOnlyList<string> GetCurrentWords() => _currentWords;

        public IReadOnlyList<string> GetPlacedClusters() => _placedClusters;

        public IReadOnlyList<string> GetClustersForCurrentWords() => _clustersForCurrentWords;

        public int MaxLettersInWord
        {
            get
            {
                int maxLength = 0;
                foreach (var word in _currentWords)
                {
                    if (word.Length > maxLength)
                        maxLength = word.Length;
                }
                return maxLength;
            }
        }

        private readonly List<string> _placedClusters = new();
        private readonly List<string> _availableClusters = new();
        private readonly List<string> _currentWords = new();
        private readonly List<string> _clustersForCurrentWords = new();

        public void Init(IEnumerable<string> clusters, IEnumerable<string> words)
        {
            Cleanup();

            _availableClusters.AddRange(clusters);
            
            IEnumerable<string> shuffledWords = words.Take(MaxWordCount);
            _currentWords.AddRange(shuffledWords);

            FilterClustersForCurrentWords();
        }

        private void FilterClustersForCurrentWords()
        {
            _clustersForCurrentWords.Clear();
            
            foreach (var word in _currentWords)
            {
                var clustersForWord = _availableClusters
                    .Where(cluster => word.Contains(cluster));
                
                _clustersForCurrentWords.AddRange(clustersForWord);
            }
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

        public bool ValidateClusters()
        {
            string combinedClusters = string.Join("", _placedClusters);
            bool isValid = true;

            _currentWords.ForEach(x => Debug.Log($"word - {x}"));
            
            Debug.Log($"{combinedClusters}");
            
            _onValidationResult.OnNext(true);

            return isValid;
        }

        private void Cleanup()
        {
            _availableClusters.Clear();
            _currentWords.Clear();
            _placedClusters.Clear();
            _clustersForCurrentWords.Clear();
        }
    }
} 
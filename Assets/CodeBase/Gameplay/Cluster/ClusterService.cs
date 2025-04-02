using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace CodeBase.Gameplay.Cluster
{
    public class ClusterService : IClusterService
    {
        private const int MaxWordCount = 4;

        private readonly Subject<Unit> _onClusterPlaced = new();
        private readonly Subject<Unit> _onClusterRemoved = new();
        private readonly Subject<bool> _onValidationResult = new();

        private readonly List<string> _placedClusters = new();
        private readonly List<string> _availableClusters = new();
        private readonly List<string> _currentWords = new();
        private readonly List<string> _formedWords = new();
        private readonly List<string> _formedWordsClusters = new();
        private readonly HashSet<int> _usedClusterIndices = new();
        private readonly HashSet<string> _clustersUsedInWordFormation = new();

        public IObservable<Unit> OnClusterPlaced => _onClusterPlaced;
        public IObservable<Unit> OnClusterRemoved => _onClusterRemoved;
        public IObservable<bool> OnValidationResult => _onValidationResult;

        public IReadOnlyList<string> GetAvailableClusters() => _availableClusters;

        public IReadOnlyList<string> GetCurrentWords() => _currentWords;

        public IReadOnlyList<string> GetPlacedClusters() => _placedClusters;

        public IReadOnlyList<string> FormedWordsClusters => _formedWordsClusters;

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

        public void Init(IEnumerable<string> clusters, IEnumerable<string> words)
        {
            Cleanup();

            _availableClusters.AddRange(clusters);

            foreach (var word in words)
            {
                if (_currentWords.Count >= MaxWordCount)
                    break;

                _currentWords.Add(word);
            }
        }

        public void PlaceCluster(string cluster)
        {
            if (_availableClusters.Contains(cluster))
            {
                _availableClusters.Remove(cluster);
                _placedClusters.Add(cluster);
                CheckFormedWords();
                _onClusterPlaced.OnNext(Unit.Default);
            }
        }

        private void CheckFormedWords()
        {
            string combinedClusters = string.Join("", _placedClusters);
            _clustersUsedInWordFormation.Clear();
            _formedWords.Clear();

            foreach (var word in _currentWords)
            {
                if (HasAllLetters(word, combinedClusters))
                {
                    _formedWords.Add(word);
                    _clustersUsedInWordFormation.Add(word);
                    Debug.Log($"Слово '{word}' может быть составлено из кластеров: {combinedClusters}");
                }
            }
        }

        private bool HasAllLetters(string sourceWord, string targetWord)
        {
            var remainingLetters = new List<char>(targetWord);
            
            foreach (char letter in sourceWord)
            {
                int index = remainingLetters.IndexOf(letter);
                if (index == -1)
                    return false;
            
                remainingLetters.RemoveAt(index);
            }
            return true;
        }

        public bool IsClusterUsedInWordFormation(string cluster)
        {
            return _clustersUsedInWordFormation.Contains(cluster);
        }

        public void RemovePlacedCluster(string cluster)
        {
            _placedClusters.Remove(cluster);
        }

        public void RemoveCluster(string cluster)
        {
            if (_placedClusters.Contains(cluster))
            {
                _placedClusters.Remove(cluster);
                _availableClusters.Add(cluster);
                _formedWordsClusters.Clear();
                _onClusterRemoved.OnNext(Unit.Default);
            }
        }

        public bool ValidateClusters()
        {
            if (_formedWords.Count != _currentWords.Count)
            {
                _onValidationResult.OnNext(false);
                return false;
            }

            foreach (var word in _formedWords)
            {
                if (!_currentWords.Contains(word))
                {
                    _onValidationResult.OnNext(false);
                    return false;
                }
            }

            _onValidationResult.OnNext(true);
            return true;
        }

        private void Cleanup()
        {
            _availableClusters.Clear();
            _currentWords.Clear();
            _placedClusters.Clear();
            _formedWords.Clear();
            _usedClusterIndices.Clear();
            _clustersUsedInWordFormation.Clear();
        }

        public void ClearFormedWordsClusters()
        {
            _formedWordsClusters.Clear();
        }
    }
}
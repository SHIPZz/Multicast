using System;
using System.Collections.Generic;
using UniRx;

namespace CodeBase.Gameplay.Cluster
{
    public interface IClusterService
    {
        IObservable<Unit> OnClusterPlaced { get; }
        IObservable<Unit> OnClusterRemoved { get; }
        IObservable<bool> OnValidationResult { get; }
        int MaxLettersInWord { get; }
        void PlaceCluster(string cluster);
        void RemoveCluster(string cluster);
        bool ValidateClusters();
        IReadOnlyList<string> GetAvailableClusters();
        IReadOnlyList<string> GetPlacedClusters();
        void Init(IEnumerable<string> clusters, IEnumerable<string> words);
        IReadOnlyList<string> GetCurrentWords();
    }
}
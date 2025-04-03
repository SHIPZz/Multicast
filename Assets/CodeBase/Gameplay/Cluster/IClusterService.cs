using System;
using System.Collections.Generic;

namespace CodeBase.Gameplay.Cluster
{
    public interface IClusterService
    {
        IObservable<bool> OnValidationResult { get; }
        void PlaceCluster(string cluster);
        void RemoveCluster(string cluster);
        bool ValidateClusters(IEnumerable<string> formedWords, IEnumerable<string> wordsToFind);
        IReadOnlyList<string> GetAvailableClusters();
        void Init(IEnumerable<string> clusters);
    }
}
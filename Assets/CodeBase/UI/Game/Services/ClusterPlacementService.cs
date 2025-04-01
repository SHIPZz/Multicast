using System.Collections.Generic;
using CodeBase.Gameplay.Common.Services.Cluster;
using UnityEngine;

namespace CodeBase.UI.Game.Services
{
    public class ClusterPlacementService : IClusterPlacementService
    {
        private readonly IClusterService _clusterService;
        private readonly Dictionary<string, List<WordSlot>> _clustersOccupiedSlots = new();

        public ClusterPlacementService(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        public bool TryPlaceCluster(string clusterText, WordSlotHolder wordSlotHolder, int startIndex)
        {
            if (!IsPlacementAvailable(clusterText, wordSlotHolder, startIndex))
                return false;

            PlaceCluster(clusterText, wordSlotHolder, startIndex);
            return true;
        }

        public void ResetCluster(string clusterText)
        {
            if (_clustersOccupiedSlots.TryGetValue(clusterText, out var slots))
            {
                foreach (var slot in slots)
                {
                    slot.Clear();
                }
                
                _clustersOccupiedSlots.Remove(clusterText);
            }
        }

        private bool IsPlacementAvailable(string clusterText, WordSlotHolder wordSlotHolder, int startIndex)
        {
            if (startIndex == -1 || startIndex + clusterText.Length > wordSlotHolder.WordSlots.Count)
                return false;

            for (int i = 0; i < clusterText.Length; i++)
            {
                if (wordSlotHolder.WordSlots[startIndex + i].IsOccupied)
                    return false;
            }

            return true;
        }

        private void PlaceCluster(string clusterText, WordSlotHolder wordSlotHolder, int startIndex)
        {
            var occupiedSlots = new List<WordSlot>();

            for (int i = 0; i < clusterText.Length; i++)
            {
                WordSlot slot = wordSlotHolder.WordSlots[startIndex + i];
                slot.SetLetter(clusterText[i].ToString());
                occupiedSlots.Add(slot);
            }
            
            _clustersOccupiedSlots[clusterText] = occupiedSlots;
            _clusterService.PlaceCluster(clusterText);
        }
    }
} 
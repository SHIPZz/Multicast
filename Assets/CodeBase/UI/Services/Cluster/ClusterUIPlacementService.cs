using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Sound;
using CodeBase.Gameplay.Cluster;
using CodeBase.Gameplay.Sound;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;
using Unity.VisualScripting;
using UnityEngine;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterUIPlacementService : IClusterUIPlacementService
    {
        private readonly IClusterService _clusterService;
        private readonly Dictionary<string, List<WordSlot>> _clustersOccupiedSlots = new();
        private readonly IWordSlotService _wordSlotService;
        private readonly ISoundService _soundService;
        private ClusterItemHolder _clusterItemHolder;

        public ClusterUIPlacementService(IClusterService clusterService, IWordSlotService wordSlotService, ISoundService soundService)
        {
            _soundService = soundService;
            _wordSlotService = wordSlotService;
            _clusterService = clusterService;
        }

        public bool TryPlaceCluster(string clusterText, int startIndex)
        {
            if (!IsPlacementAvailable(clusterText, startIndex))
                return false;

            PlaceCluster(clusterText, startIndex);
            
            _soundService.Play(SoundTypeId.ClusterPlaced);
            
            return true;
        }

        public void SetClusterItemHolder(ClusterItemHolder clusterItemHolder) => _clusterItemHolder = clusterItemHolder;

        public void OnClusterSelected(ClusterItem clusterItem) => _soundService.Play(SoundTypeId.TakeCluster);

        public void ResetCluster(string clusterText)
        {
            if (_clustersOccupiedSlots.TryGetValue(clusterText, out List<WordSlot> slots))
            {
                foreach (var slot in slots)
                {
                    slot.Clear();
                }

                _clustersOccupiedSlots.Remove(clusterText);
                _clusterService.RemoveCluster(clusterText);
            }
        }

        private bool IsPlacementAvailable(string clusterText, int startIndex)
        {
            if (startIndex == -1 || startIndex + clusterText.Length > _wordSlotService.SlotCount)
                return false;

            for (int i = 0; i < clusterText.Length; i++)
            {
                if (_wordSlotService.GetTargetSlot(startIndex + i).IsOccupied)
                    return false;
            }

            return true;
        }

        private void PlaceCluster(string clusterText, int startIndex)
        {
            List<WordSlot> occupiedSlots = new List<WordSlot>();

            for (int i = 0; i < clusterText.Length; i++)
            {
                int targetIndex = startIndex + i;

                WordSlot slot = _wordSlotService.GetTargetSlot(targetIndex);

                slot.SetLetter(clusterText[i].ToString());

                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[clusterText] = occupiedSlots;
            _clusterService.PlaceCluster(clusterText);
        }

        public void CheckAndHideFilledClusters()
        {
            foreach (KeyValuePair<int, string> keyValuePair in _wordSlotService.GetFormedWordsFromRows())
            {
                string formedWord = keyValuePair.Value;
                
                if(!_wordSlotService.WordsToFind.Contains(formedWord))
                    continue;

                if (string.IsNullOrEmpty(formedWord))
                    continue;

                Debug.Log($"formedWord - {formedWord}");

                if (NoSuchTargetWordBy(formedWord))
                {
                    Debug.Log($"no target word like - {formedWord}");
                    continue;
                }

                int row = keyValuePair.Key;

                HideAllClustersOutlineIconInRow(row);
            }
        }

        private bool NoSuchTargetWordBy(string formedWord)
        {
            return !_wordSlotService
                .WordsToFind
                .Any(targetWord => targetWord.Equals(formedWord, StringComparison.OrdinalIgnoreCase));
        }

        private void HideAllClustersOutlineIconInRow(int row)
        {
            foreach (ClusterItem cluster in _clusterItemHolder.ClusterItems)
            {
                if (!cluster.IsPlaced)
                    continue;
                
                foreach (WordSlot slot in _wordSlotService.GetWordSlotsByRow(row))
                {
                    Debug.Log($"@@@: {cluster.Text} - {slot.CurrentLetter} - {cluster.Text.Contains(slot.CurrentLetter)}");
                    
                    if (slot.IsOccupied && slot.CurrentLetter != "" && cluster.Text.Contains(slot.CurrentLetter))
                    {
                        cluster.HideOutlineIcon();
                        cluster.SetBlocksRaycasts(false);
                    }
                }
            }
        }
    }
}
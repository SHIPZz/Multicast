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

        public ClusterUIPlacementService(IClusterService clusterService, IWordSlotService wordSlotService,
            ISoundService soundService)
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

                if (string.IsNullOrEmpty(formedWord))
                {
                    Debug.Log($"Row {keyValuePair.Key} is empty");
                    continue;
                }

                if (!_wordSlotService.WordsToFind.Contains(formedWord, StringComparer.OrdinalIgnoreCase))
                {
                    Debug.Log($"Row {keyValuePair.Key} - {keyValuePair.Value} is not found");
                    continue;
                }

                Debug.Log($"Found word: {formedWord} in row {keyValuePair.Key}");
                HideAllClustersOutlineIconInRow(keyValuePair.Key, formedWord);
            }
        }

        private void HideAllClustersOutlineIconInRow(int row, string formedWord)
        {
            foreach (ClusterItem cluster in _clusterItemHolder.ClusterItems)
            {
                if (!cluster.IsPlaced)
                    continue;

                bool allLettersInWord = true;

                foreach (char letter in cluster.Text)
                {
                    if (formedWord.All(c => char.ToUpperInvariant(c) != char.ToUpperInvariant(letter)))
                    {
                        allLettersInWord = false;
                        break;
                    }
                }

                if (!allLettersInWord)
                    continue;
                
                Debug.Log($"Hiding cluster: {cluster.Text} for word: {formedWord}");
                cluster.HideOutlineIcon();
                cluster.SetBlocksRaycasts(false);
            }
        }
    }
}
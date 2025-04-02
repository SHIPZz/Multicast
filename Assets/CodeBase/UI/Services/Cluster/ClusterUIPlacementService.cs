using System.Collections.Generic;
using CodeBase.Gameplay.Cluster;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;
using UniRx;
using UnityEngine;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterUIPlacementService : IClusterPlacementService
    {
        private readonly IClusterService _clusterService;
        private readonly Dictionary<string, List<WordSlot>> _clustersOccupiedSlots = new();
        private readonly Dictionary<string, ClusterItem> _clusterItems = new();
        private readonly CompositeDisposable _disposables = new();

        public ClusterUIPlacementService(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        public void RegisterClusterItem(string clusterText, ClusterItem clusterItem)
        {
            _clusterItems[clusterText] = clusterItem;
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
            List<WordSlot> occupiedSlots = new List<WordSlot>();

            for (int i = 0; i < clusterText.Length; i++)
            {
                int targetIndex = startIndex + i;
                WordSlot slot = wordSlotHolder.WordSlots[targetIndex];
                slot.SetLetter(clusterText[i].ToString());
                slot.ClusterItem = _clusterItems[clusterText];
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[clusterText] = occupiedSlots;
            _clusterService.PlaceCluster(clusterText);
        }

        public void HideClusterInSlot(WordSlot slot)
        {
            string targetText = slot.CurrentLetter.ToLower();

            foreach (ClusterItem clusterItem in _clusterItems.Values)
            {
                if (!clusterItem.IsPlaced)
                    continue;

                string text = clusterItem.Text.ToLower();

                Debug.Log($"cluster - {text}");
                Debug.Log($"target  - {targetText}");

                foreach (char letter in targetText)
                {
                    if (text.Contains(letter))
                    {
                        clusterItem.HideOutlineIcon();
                    }
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
    }
}
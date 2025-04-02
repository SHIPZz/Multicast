using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Cluster;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots
{
    public class WordSlotHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new();
        private readonly Dictionary<int, List<WordSlot>> _rowSlots = new();

        private IWordSlotUIFactory _wordSlotUIFactory;
        private IClusterService _clusterService;
        private IClusterPlacementService _placementService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;

        public int IndexOf(WordSlot wordSlot) => _wordSlots.IndexOf(wordSlot);

        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory, IClusterService clusterService, IClusterPlacementService placementService)
        {
            _clusterService = clusterService;
            _wordSlotUIFactory = wordSlotUIFactory;
            _placementService = placementService;
        }

        public void CreateWordSlots()
        {
            ClearSlots();

            int targetCount = _clusterService.GetCurrentWords().Count * _clusterService.MaxLettersInWord;
            int lettersInWord = _clusterService.MaxLettersInWord;
            int wordCount = _clusterService.GetCurrentWords().Count;

            for (int row = 0; row < wordCount; row++)
            {
                _rowSlots[row] = new List<WordSlot>();
                
                for (int i = 0; i < lettersInWord; i++)
                {
                    int slotIndex = row * lettersInWord + i;
                    WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);
                    _wordSlots.Add(slot);
                    _rowSlots[row].Add(slot);
                }
            }
        }

        public void ClearSlots()
        {
            foreach (var slot in _wordSlots)
            {
                Destroy(slot.gameObject);
            }

            _wordSlots.Clear();
            _rowSlots.Clear();
        }

        [ContextMenu("check")]
        public void CheckClustersByRows()
        {
            foreach (var row in _rowSlots)
            {
                string rowClusters = "";
                foreach (var slot in row.Value)
                {
                    rowClusters += slot.CurrentLetter;
                }
                Debug.Log($"Строка {row.Key + 1}: {rowClusters}");
            }
        }

        //todo refactor move to service
        [ContextMenu("check 2")]
        public void CheckAndHideFilledClusters()
        {
            foreach (var row in _rowSlots)
            {
                string formedWord = "";
                foreach (var slot in row.Value)
                {
                    if (slot.IsOccupied)
                        formedWord += slot.CurrentLetter;
                }
                
                Debug.Log($"Строка {row.Key + 1}: {formedWord}");
                
                if (_clusterService.GetCurrentWords().Any(word => word.Equals(formedWord, StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var slot in row.Value)
                    {
                        if (slot.IsOccupied && slot.ClusterItem != null)
                            slot.ClusterItem.HideOutlineIcon();
                    }
                }
            }
        }
    }
}
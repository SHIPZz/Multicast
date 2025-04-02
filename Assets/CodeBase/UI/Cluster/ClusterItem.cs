using CodeBase.UI.Common;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.WordSlots;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Cluster
{
    public class ClusterItem : DraggableItem, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _outlineIcon;

        private string _clusterText;
        private WordSlotHolder _wordSlotHolder;
        private IClusterPlacementService _placementService;

        [Inject]
        private void Construct(IClusterPlacementService placementService)
        {
            _placementService = placementService;
        }

        public void Initialize(string text, WordSlotHolder wordSlotHolder, Transform parent, Canvas parentCanvas)
        {
            base.Initialize(parent, parentCanvas);
            
            _clusterText = text;
            _text.text = text;
            _wordSlotHolder = wordSlotHolder;
            _outlineIcon.enabled = true;
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            GameObject raycastObject = eventData.pointerCurrentRaycast.gameObject;
            var wordSlot = raycastObject?.GetComponent<WordSlot>();
            
            if (wordSlot == null || wordSlot.IsOccupied)
            {
                ReturnToStartPosition();
                return;
            }
            
            int startIndex = _wordSlotHolder.IndexOf(wordSlot);

            if (_placementService.TryPlaceCluster(_clusterText, _wordSlotHolder, startIndex))
            {
                MarkPlaced(startIndex);
                return;
            }

            ReturnToStartPosition();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsPlaced) 
                OnReset();
        }

        protected override void OnReset()
        {
            base.OnReset();
            _text.enabled = true;
            ShowOutlineIcon();
            _placementService.ResetCluster(_clusterText);
        }

        private void MarkPlaced(int startIndex)
        {
            IsPlaced = true;
            _text.enabled = false;
            MoveToCenter(_wordSlotHolder.WordSlots[startIndex].transform);
            CanvasGroup.blocksRaycasts = true;
        }

        public void HideOutlineIcon()
        {
            _outlineIcon.enabled = false;
        }

        public void ShowOutlineIcon()
        {
            _outlineIcon.enabled = true;
        }
    }
}
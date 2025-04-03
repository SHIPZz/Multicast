using CodeBase.Gameplay.WordSlots;
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
        private IClusterUIPlacementService _clusterUIPlacementService;
        private IWordSlotService _wordSlotService;

        public string Text => _clusterText;

        [Inject]
        private void Construct(IClusterUIPlacementService uiPlacementService, IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
            _clusterUIPlacementService = uiPlacementService;
        }

        public void Initialize(string text, Transform parent, Canvas parentCanvas)
        {
            base.Initialize(parent, parentCanvas);
            
            _clusterText = text;
            _text.text = text;
            _outlineIcon.enabled = true;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            _clusterUIPlacementService.OnClusterSelected(this);
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
            
            int startIndex = _wordSlotService.IndexOf(wordSlot);

            if (_clusterUIPlacementService.TryPlaceCluster(_clusterText, startIndex))
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
            _clusterUIPlacementService.ResetCluster(_clusterText);
        }

        private void MarkPlaced(int startIndex)
        {
            IsPlaced = true;
            _text.enabled = false;
            MoveToCenter(_wordSlotService.GetTargetSlot(startIndex).transform);
            CanvasGroup.blocksRaycasts = true;
        }

        public void HideOutlineIcon()
        {
            _outlineIcon.enabled = false;
        }
        
        public void SetBlocksRaycasts(bool value)
        {
            CanvasGroup.blocksRaycasts = value;
        }

        public void ShowOutlineIcon()
        {
            _outlineIcon.enabled = true;
        }
    }
}
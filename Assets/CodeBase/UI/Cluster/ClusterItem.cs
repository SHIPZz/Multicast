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
        private IClusterService _clusterService;

        public string Text => _clusterText;

        [Inject]
        private void Construct(IClusterService service)
        {
            _clusterService = service;
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

            _clusterService.OnClusterSelected(this);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            GameObject raycastObject = eventData.pointerCurrentRaycast.gameObject;
            var wordSlot = raycastObject?.GetComponent<WordSlot>();
            
            PlaceToSlot(wordSlot);
        }

        public void PlaceToSlot(WordSlot wordSlot)
        {
            if (wordSlot == null || wordSlot.IsOccupied)
            {
                ReturnToStartPosition();
                return;
            }
            
            if (_clusterService.TryPlaceCluster(this, wordSlot))
            {
                MarkPlacedTo(wordSlot);
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
            SetOutlineIconActive(true);
            _clusterService.ResetCluster(this);
        }

        public void MarkPlacedTo(WordSlot wordSlot)
        {
            IsPlaced = true;
            _text.enabled = false;
            MoveToCenter(wordSlot.transform);
            CanvasGroup.blocksRaycasts = true;
        }

        public void SetOutlineIconActive(bool isActive)
        {
            _outlineIcon.enabled = isActive;
        }
        
        public void SetBlocksRaycasts(bool value)
        {
            CanvasGroup.blocksRaycasts = value;
        }
    }
}
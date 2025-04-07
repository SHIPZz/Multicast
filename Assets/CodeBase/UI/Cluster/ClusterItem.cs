using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Draggable;
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
        private Transform _originalParent;

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
            _originalParent = parent;
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
                OnReset();
                return;
            }
            
            if (_clusterService.TryPlaceCluster(this, wordSlot))
            {
                MarkPlacedTo(wordSlot);
                return;
            }

            OnReset();
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
            ReturnToOriginalPosition();
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

        private void ReturnToOriginalPosition()
        {
            transform.SetParent(_originalParent);
            transform.localPosition = Vector3.zero;
        }
    }
}
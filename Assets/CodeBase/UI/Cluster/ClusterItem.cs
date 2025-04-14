using System;
using CodeBase.Data;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Draggable;
using CodeBase.UI.WordSlots;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Cluster
{
    public class ClusterItem : DraggableItem, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Sprite _outlineIcon;
        [SerializeField] private Sprite _baseIcon;
        [SerializeField] private Image _image;

        private readonly Subject<Unit> _disabledEvent = new Subject<Unit>();

        private string _clusterText;
        private IClusterService _clusterService;
        private Transform _originalParent;

        public IObservable<Unit> DisabledEvent => _disabledEvent;

        public string Text => _clusterText;

        public string Id { get; private set; }

        public int Row { get; private set; }

        public int Column { get; private set; }

        [Inject]
        private void Construct(IClusterService service)
        {
            _clusterService = service;
        }

        public void Initialize(ClusterModel model, Transform parent, Canvas parentCanvas)
        {
            base.Initialize(parent, parentCanvas);

            _clusterText = model.Text;
            _text.text = model.Text;
            _originalParent = parent;
            Id = model.Id;
            Row = model.Row;
            Column = model.Column;
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
            _clusterService.ResetCluster(this);
            UpdatePosition(-1, -1);
            SetOutlineIconActive(false);

            ReturnToOriginalPosition();
        }

        public void MarkPlacedTo(WordSlot wordSlot)
        {
            IsPlaced = true;
            _text.enabled = false;

            MoveToCenter(wordSlot.transform);
            UpdatePosition(wordSlot.Row, wordSlot.Column);
            SetOutlineIconActive(true);
            SetBlocksRaycasts(true);
        }

        private void UpdatePosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public void SetOutlineIconActive(bool isActive)
        {
            _image.sprite = isActive ? _outlineIcon : _baseIcon;
        }

        public void MarkDisabled()
        {
            SetBlocksRaycasts(false);

            _image.enabled = false;

            _disabledEvent.OnNext(Unit.Default);
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
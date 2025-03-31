using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Game
{
    //todo refactor by diving
    public class ClusterItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;

        private string _clusterText;
        private Vector3 _startPosition;
        private Transform _startParent;
        private WordSlotHolder _wordSlotHolder;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public void Initialize(string text, WordSlotHolder wordSlotHolder)
        {
            _clusterText = text;
            _text.text = text;
            _wordSlotHolder = wordSlotHolder;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = transform.position;
            _startParent = transform.parent;
            
            _canvasGroup.blocksRaycasts = false;
            
            transform.SetParent(_canvas.transform);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // todo refactor 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out Vector2 localPoint);

            transform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == null)
            {
                ReturnToStartPosition();
                return;
            }

            var wordSlot = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<WordSlot>();
            
            if (wordSlot != null && !wordSlot.IsOccupied)
            {
                var startIndex = _wordSlotHolder.IndexOf(wordSlot);
                
                if (startIndex != -1 && startIndex + _clusterText.Length <= _wordSlotHolder.WordSlots.Count)
                {
                    bool canPlace = true;
                    for (int i = 0; i < _clusterText.Length; i++)
                    {
                        if (_wordSlotHolder.WordSlots[startIndex + i].IsOccupied)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        for (int i = 0; i < _clusterText.Length; i++)
                        {
                            _wordSlotHolder.WordSlots[startIndex + i].SetLetter(_clusterText[i].ToString());
                        }
                    }
                }
            }
        }

        private void ReturnToStartPosition()
        {
            transform.SetParent(_startParent);
            transform.position = _startPosition;
            _canvasGroup.blocksRaycasts = true;
        }

        public string GetClusterText() => _clusterText;
    }
} 

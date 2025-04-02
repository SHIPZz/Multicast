using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Common
{
    public abstract class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected CanvasGroup CanvasGroup;
        
        protected Canvas ParentCanvas;
        protected Vector3 StartPosition;
        protected Transform ParentTransform;
        
        public bool IsPlaced { get; protected set; }

        public virtual void Initialize(Transform parent, Canvas parentCanvas)
        {
            ParentTransform = parent;
            ParentCanvas = parentCanvas;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (IsPlaced)
            {
                OnReset();
            }
            
            StartPosition = transform.position;
            CanvasGroup.blocksRaycasts = false;
            transform.SetParent(ParentCanvas.transform);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                ParentCanvas.transform as RectTransform,
                eventData.position,
                ParentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : ParentCanvas.worldCamera,
                out Vector2 localPoint);

            transform.localPosition = localPoint;
        }

        public abstract void OnEndDrag(PointerEventData eventData);

        protected virtual void ReturnToStartPosition()
        {
            transform.SetParent(ParentTransform);
            transform.position = StartPosition;
            CanvasGroup.blocksRaycasts = true;
        }

        protected virtual void OnReset()
        {
            IsPlaced = false;
            ReturnToStartPosition();
        }

        protected virtual void MoveToCenter(Transform parent)
        {
            var rectTransform = transform as RectTransform;
            rectTransform.SetParent(parent);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
} 
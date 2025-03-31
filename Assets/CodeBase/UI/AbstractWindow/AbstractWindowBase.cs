using System;
using UniRx;
using UnityEngine;

namespace CodeBase.UI.AbstractWindow
{
    public abstract class AbstractWindowBase : MonoBehaviour
    {
        private readonly Subject<Unit> _onCloseEvent = new();
        private readonly Subject<Unit> _onOpenEvent = new();

        public IObservable<Unit> OnOpenEvent => _onOpenEvent;
        public IObservable<Unit> OnCloseEvent => _onCloseEvent;
        
        public void Open()
        {
            _onOpenEvent.OnNext(Unit.Default);

            OnOpen();
        }

        public void Close()
        {
            _onCloseEvent.OnNext(Unit.Default);
            
            OnClose();
            
            Destroy(gameObject);
        }
        
        public void ReportCloseRequest() => _onCloseEvent.OnNext(Unit.Default);

        public virtual void OnClose()
        {
            
        }

        public virtual void OnOpen()
        {
            
        }
    }
}
using UnityEngine;

namespace CodeBase.UI.AbstractWindow
{
    public abstract class AbstractWindowBase : MonoBehaviour
    {
        public void Open()
        {
            OnOpen();
        }

        public void Close()
        {
            OnClose();
            
            Destroy(gameObject);
        }

        public virtual void OnClose()
        {
            
        }

        public virtual void OnOpen()
        {
            
        }
    }
}
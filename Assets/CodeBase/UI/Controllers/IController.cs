using System;
using CodeBase.UI.AbstractWindow;
using Zenject;

namespace CodeBase.UI.Controllers
{
    public interface IController : IInitializable, IDisposable
    {
    }

    public interface IController<in TWindow> : IController where TWindow : AbstractWindowBase
    {
        void BindView(TWindow value);
    }
    
    public interface IModelBindable
    {
        void BindModel(AbstractWindowModel model);
    }
    
    public interface IModelBindable<TModel> : IModelBindable
        where TModel : AbstractWindowModel
    {
        void IModelBindable.BindModel(AbstractWindowModel model)
        {
           BindModel((TModel)model);
        }

        void BindModel(TModel model);
    }
}
using System;
using System.Collections.Generic;
using CodeBase.StaticData;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Controllers;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.Window
{
    public class WindowService : IWindowService
    {
        private const int BaseSortingOrder = 100;
        private const int TopSortingOrder = 1000;
        
        private readonly IInstantiator _instantiator;
        private readonly IStaticDataService _staticDataService;
        private readonly IUIProvider _uiProvider;

        private readonly Dictionary<Type, WindowBindingInfo> _windowBindings = new();
        private readonly Dictionary<Type, (AbstractWindowBase Window, IController Controller)> _activeWindows = new();
        private int _currentSortingOrder = BaseSortingOrder;

        public WindowService(IInstantiator instantiator,
            IStaticDataService staticDataService,
            IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }

        public void Bind<TWindow, TController>()
            where TWindow : AbstractWindowBase
            where TController : IController<TWindow>
        {
            var windowType = typeof(TWindow);

            if (_windowBindings.ContainsKey(windowType))
            {
                Debug.LogWarning($"Window type {windowType.Name} is already bound.");
                return;
            }

            _windowBindings[windowType] = new WindowBindingInfo
            {
                WindowType = windowType,
                ControllerType = typeof(TController),
                ModelType = null,
                Prefab = _staticDataService.GetWindow<TWindow>()
            };
        }

        public void Bind<TWindow, TController, TModel>()
            where TWindow : AbstractWindowBase
            where TModel : AbstractWindowModel
            where TController : IModelBindable
        {
            var windowType = typeof(TWindow);

            if (_windowBindings.ContainsKey(windowType))
                throw new InvalidOperationException($"Window type {windowType.Name} is already bound.");

            _windowBindings[windowType] = new WindowBindingInfo
            {
                WindowType = windowType,
                ControllerType = typeof(TController),
                ModelType = typeof(TModel),
                Prefab = _staticDataService.GetWindow<TWindow>()
            };
        }

        public TWindow OpenWindow<TWindow>(bool onTop = false) where TWindow : AbstractWindowBase
        {
            Type windowType = typeof(TWindow);

            if (!_windowBindings.TryGetValue(windowType, out var bindingInfo))
                throw new InvalidOperationException($"No binding found for window type {windowType.Name}");

            if (_activeWindows.ContainsKey(typeof(TWindow)))
            {
                var window = (TWindow)_activeWindows[windowType].Window;
                SetWindowSortingOrder(window, onTop);
                return window;
            }

            TWindow createdWindow = _instantiator.InstantiatePrefabForComponent<TWindow>(bindingInfo.Prefab, _uiProvider.MainUI);

            IController<TWindow> controller = (IController<TWindow>)_instantiator.Instantiate(bindingInfo.ControllerType);

            BindModelIfHas(bindingInfo, controller);

            InitWindow(controller, createdWindow);

            _activeWindows[windowType] = (createdWindow, (controller));

            SetWindowSortingOrder(createdWindow, onTop);

            return createdWindow;
        }

        private void SetWindowSortingOrder(AbstractWindowBase window, bool onTop)
        {
            if (window.TryGetComponent<Canvas>(out var canvas))
            {
                canvas.sortingOrder = onTop ? TopSortingOrder : _currentSortingOrder++;
            }
        }

        public void Close<TWindow>() where TWindow : AbstractWindowBase
        {
            var windowType = typeof(TWindow);

            if (!_activeWindows.TryGetValue(windowType, out (AbstractWindowBase Window, IController Controller) windowData))
                return;

            windowData.Window.Close();

            if (windowData.Controller is IDisposable disposable)
                disposable.Dispose();

            _activeWindows.Remove(windowType);
        }

        public void CloseAll()
        {
            foreach ((AbstractWindowBase Window, IController Controller) windowData in _activeWindows.Values)
            {
                if (windowData.Window != null && windowData.Window.gameObject != null)
                {
                    windowData.Window.Close();
                }

                if (windowData.Controller is IDisposable disposable)
                    disposable.Dispose();
            }
            
            _activeWindows.Clear();
            _currentSortingOrder = BaseSortingOrder;
        }

        private void BindModelIfHas(WindowBindingInfo bindingInfo, IController controller)
        {
            if (bindingInfo.ModelType != null)
            {
                var model = (AbstractWindowModel)_instantiator.Instantiate(bindingInfo.ModelType);

                if (controller is IModelBindable controllerWithModel)
                    controllerWithModel.BindModel(model);
            }
        }

        private static void InitWindow<TWindow>(IController<TWindow> controller, TWindow window)
            where TWindow : AbstractWindowBase
        {
            controller.BindView(window);
            controller.Initialize();
            window.Open();
        }
    }
}
using System;

namespace CodeBase.UI.Services.Window
{
    public class WindowBindingInfo
    {
        public Type WindowType { get; set; }
        public Type ControllerType { get; set; }
        public Type ModelType { get; set; }
        public UnityEngine.Object Prefab { get; set; }
    }
}
using CodeBase.Common.Services.Persistent;

namespace CodeBase.UI.WordSlots.Services
{
    public interface IWordSlotFacade : IProgressWatcher
    {
        void Cleanup();
    }
}
using CodeBase.Common.Services.Persistent;

namespace CodeBase.UI.WordCells.Services
{
    public interface IWordSlotFacade : IProgressWatcher
    {
        void Cleanup();
    }
}
using UnityEngine;

namespace CodeBase.UI.WordSlots.Services.Factory
{
    public interface IWordSlotUIFactory
    {
        WordSlotHolder CreateWordSlotHolder(Transform parent);
        WordSlot CreateWordSlotPrefab(Transform parent);
    }
} 
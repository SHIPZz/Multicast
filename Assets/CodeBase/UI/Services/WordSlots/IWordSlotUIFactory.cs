using CodeBase.UI.Game;
using UnityEngine;

namespace CodeBase.UI.Services.WordSlots
{
    public interface IWordSlotUIFactory
    {
        WordSlotHolder CreateWordSlotHolder(Transform parent);
        WordSlot CreateWordSlotPrefab(Transform parent);
    }
} 
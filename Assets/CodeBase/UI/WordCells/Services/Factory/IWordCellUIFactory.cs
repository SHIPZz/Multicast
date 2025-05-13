using UnityEngine;

namespace CodeBase.UI.WordCells.Services.Factory
{
    public interface IWordCellUIFactory
    {
        WordCellsHolder CreateWordSlotHolder(Transform parent);
        WordCellView CreateWordSlotPrefab(Transform parent);
    }
} 
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.UI.Tooltips;
using UnityEngine;

namespace RPG.UI.Inventories
{
    /// <summary>
    /// To be placed on a UI slot to spawn and show the correct item tooltip.
    /// </summary>
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            InventoryItem item = GetComponent<InventorySlotUI>().GetItem();
            if (!item) return;
            tooltip.GetComponent<ItemTooltipUI>().Setup(item.GetDisplayName(), item.GetDescription());
        }

        public override bool CanCreateTooltip()
        {
            InventoryItem item = GetComponent<InventorySlotUI>().GetItem();
            return item;
        }
    }
}

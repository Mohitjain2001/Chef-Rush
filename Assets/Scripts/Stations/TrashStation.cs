using UnityEngine;
using YesChef.Items;
using YesChef.Player;

namespace YesChef.Stations
{
    public class TrashStation : BaseStation
    {
        public override void Interact(PlayerInteraction player)
        {
            if (player != null && player.HasItemHeld())
            {
                KitchenItem item = player.DropItem();
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        public override string GetInteractPrompt(PlayerInteraction player)
        {
            if (player != null && player.HasItemHeld())
            {
                KitchenItem item = player.GetHeldItem();
                string name = item != null && item.ingredientData != null ? item.ingredientData.ingredientName : "Item";
                return $"Throw away {name}";
            }
            return "Trash Can (Empty Hands)";
        }
    }
}

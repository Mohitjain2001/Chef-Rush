using UnityEngine;
using YesChef.Data;
using YesChef.Items;
using YesChef.Player;

namespace YesChef.Stations
{
    public class FridgeStation : BaseStation
    {
        [Header("Fridge Settings")]
        public IngredientSO ingredientToDispense;

        public override void Interact(PlayerInteraction player)
        {
            if (player == null || player.HasItemHeld())
            {
                // Player is already holding an item
                return;
            }

            if (ingredientToDispense == null) return;

            // Instantiate raw ingredient
            GameObject itemObj = new GameObject($"Item_{ingredientToDispense.ingredientName}");
            KitchenItem kitchenItem = itemObj.AddComponent<KitchenItem>();
            kitchenItem.Init(ingredientToDispense, prepared: ingredientToDispense.requiredStation == PrepStationType.None);

            player.PickUpItem(kitchenItem);
        }

        public override string GetInteractPrompt(PlayerInteraction player)
        {
            if (player.HasItemHeld())
            {
                return "Hands Full";
            }
            return ingredientToDispense != null ? $"Get {ingredientToDispense.ingredientName}" : "Get Ingredient";
        }
    }
}

using UnityEngine;
using YesChef.Data;
using YesChef.Items;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Stations
{
    public class CuttingTableStation : BaseStation
    {
        public KitchenItem currentItem;
        public float chopProgress = 0f;
        public float chopDuration = 2.0f;
        public bool isChopping = false;

        public ProgressBarUI progressBar;

        private void Update()
        {
            if (isChopping && currentItem != null)
            {
                chopProgress += Time.deltaTime;

                if (progressBar != null)
                {
                    progressBar.SetProgress(chopProgress / chopDuration);
                    progressBar.Show(true);
                }

                if (chopProgress >= chopDuration)
                {
                    isChopping = false;
                    chopProgress = chopDuration;
                    currentItem.SetPrepared(true);

                    if (progressBar != null)
                    {
                        progressBar.Show(false);
                    }
                }
            }
        }

        public override void Interact(PlayerInteraction player)
        {
            if (player == null) return;

            // Case 1: Table is empty, player places raw Vegetable
            if (currentItem == null && player.HasItemHeld())
            {
                KitchenItem held = player.GetHeldItem();
                if (held != null && held.ingredientData != null && 
                    held.ingredientData.ingredientType == IngredientType.Vegetable && 
                    !held.isPrepared && !held.isBeingPrepared)
                {
                    currentItem = player.DropItem();
                    currentItem.transform.SetParent(itemHoldPoint);
                    currentItem.transform.localPosition = Vector3.zero;
                    currentItem.transform.localRotation = Quaternion.identity;

                    chopProgress = 0f;
                    isChopping = true;
                    currentItem.isBeingPrepared = true;
                }
                return;
            }

            // Case 2: Table has completed chopped Vegetable
            if (currentItem != null && currentItem.isPrepared)
            {
                if (!player.HasItemHeld())
                {
                    // Empty hands: pick up chopped veg
                    KitchenItem itemToPick = currentItem;
                    currentItem = null;
                    if (progressBar != null) progressBar.Show(false);
                    player.PickUpItem(itemToPick);
                    return;
                }
                else
                {
                    // Hands full: Swap if held item is raw Vegetable!
                    KitchenItem held = player.GetHeldItem();
                    if (held != null && held.ingredientData != null && 
                        held.ingredientData.ingredientType == IngredientType.Vegetable && 
                        !held.isPrepared && !held.isBeingPrepared)
                    {
                        KitchenItem preparedItem = currentItem;
                        KitchenItem newRawItem = player.DropItem();

                        // Put new raw item on table
                        currentItem = newRawItem;
                        currentItem.transform.SetParent(itemHoldPoint);
                        currentItem.transform.localPosition = Vector3.zero;
                        currentItem.transform.localRotation = Quaternion.identity;

                        chopProgress = 0f;
                        isChopping = true;
                        currentItem.isBeingPrepared = true;

                        // Pick up prepared item into hands
                        if (progressBar != null) progressBar.Show(false);
                        player.PickUpItem(preparedItem);
                        return;
                    }
                }
            }
        }

        public override string GetInteractPrompt(PlayerInteraction player)
        {
            if (currentItem == null)
            {
                if (player.HasItemHeld())
                {
                    KitchenItem held = player.GetHeldItem();
                    if (held?.ingredientData?.ingredientType == IngredientType.Vegetable && !held.isPrepared)
                    {
                        return "[ E ] Place Vegetable to Chop";
                    }
                    return "Cutting Table (Needs Raw Vegetable)";
                }
                return "Cutting Table (Empty)";
            }

            if (isChopping)
            {
                float remaining = Mathf.Max(0, chopDuration - chopProgress);
                return $"Chopping Vegetable... ({remaining:F1}s)";
            }

            if (currentItem.isPrepared)
            {
                if (player.HasItemHeld())
                {
                    KitchenItem held = player.GetHeldItem();
                    if (held?.ingredientData?.ingredientType == IngredientType.Vegetable && !held.isPrepared)
                    {
                        return "[ E ] Swap with New Raw Veg (Take Chopped Veg)";
                    }
                    return "Hands Full! Deliver item to Window or Trash first!";
                }
                return "[ E ] Take Chopped Vegetable";
            }

            return "Cutting Table";
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using YesChef.Data;
using YesChef.Items;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Stations
{
    [System.Serializable]
    public class StoveSlot
    {
        public int slotIndex;
        public Transform holdPoint;
        public KitchenItem currentItem;
        public float cookProgress = 0f;
        public float cookDuration = 6.0f;
        public bool isCooking = false;
        public ProgressBarUI progressBar;
    }

    public class StoveStation : BaseStation
    {
        [Header("Stove Slots")]
        public StoveSlot[] slots = new StoveSlot[2];
        public Transform slot1HoldPoint;
        public Transform slot2HoldPoint;

        public ProgressBarUI slot1ProgressBar;
        public ProgressBarUI slot2ProgressBar;

        protected override void Awake()
        {
            base.Awake();

            if (slots == null || slots.Length != 2)
            {
                slots = new StoveSlot[2];
            }

            for (int i = 0; i < 2; i++)
            {
                if (slots[i] == null) slots[i] = new StoveSlot();
                slots[i].slotIndex = i;
            }

            if (slot1HoldPoint != null) slots[0].holdPoint = slot1HoldPoint;
            if (slot2HoldPoint != null) slots[1].holdPoint = slot2HoldPoint;
            if (slot1ProgressBar != null) slots[0].progressBar = slot1ProgressBar;
            if (slot2ProgressBar != null) slots[1].progressBar = slot2ProgressBar;
        }

        private void Update()
        {
            foreach (var slot in slots)
            {
                if (slot != null && slot.isCooking && slot.currentItem != null)
                {
                    slot.cookProgress += Time.deltaTime;

                    if (slot.progressBar != null)
                    {
                        slot.progressBar.SetProgress(slot.cookProgress / slot.cookDuration);
                        slot.progressBar.Show(true);
                    }

                    if (slot.cookProgress >= slot.cookDuration)
                    {
                        slot.isCooking = false;
                        slot.cookProgress = slot.cookDuration;
                        slot.currentItem.SetPrepared(true);

                        if (slot.progressBar != null)
                        {
                            slot.progressBar.Show(false);
                        }
                    }
                }
            }
        }

        public override void Interact(PlayerInteraction player)
        {
            if (player == null) return;

            // 1. If player has raw meat, try to place in an empty slot or swap with a cooked slot
            if (player.HasItemHeld())
            {
                KitchenItem held = player.GetHeldItem();
                if (held != null && held.ingredientData != null && 
                    held.ingredientData.ingredientType == IngredientType.Meat && 
                    !held.isPrepared && !held.isBeingPrepared)
                {
                    StoveSlot emptySlot = GetEmptySlot();
                    if (emptySlot != null)
                    {
                        KitchenItem item = player.DropItem();
                        emptySlot.currentItem = item;
                        if (emptySlot.holdPoint != null)
                        {
                            item.transform.SetParent(emptySlot.holdPoint);
                            item.transform.localPosition = Vector3.zero;
                            item.transform.localRotation = Quaternion.identity;
                        }

                        emptySlot.cookProgress = 0f;
                        emptySlot.isCooking = true;
                        item.isBeingPrepared = true;
                        return;
                    }
                    else
                    {
                        // Slots are full: Swap with a cooked slot if available!
                        StoveSlot cookedSlot = GetCookedSlot();
                        if (cookedSlot != null)
                        {
                            KitchenItem cookedItem = cookedSlot.currentItem;
                            KitchenItem newRawMeat = player.DropItem();

                            cookedSlot.currentItem = newRawMeat;
                            if (cookedSlot.holdPoint != null)
                            {
                                newRawMeat.transform.SetParent(cookedSlot.holdPoint);
                                newRawMeat.transform.localPosition = Vector3.zero;
                                newRawMeat.transform.localRotation = Quaternion.identity;
                            }
                            cookedSlot.cookProgress = 0f;
                            cookedSlot.isCooking = true;
                            newRawMeat.isBeingPrepared = true;

                            if (cookedSlot.progressBar != null) cookedSlot.progressBar.Show(false);
                            player.PickUpItem(cookedItem);
                            return;
                        }
                    }
                }
            }

            // 2. If player has empty hands, pick up cooked meat from a finished slot
            if (!player.HasItemHeld())
            {
                StoveSlot cookedSlot = GetCookedSlot();
                if (cookedSlot != null)
                {
                    KitchenItem itemToPick = cookedSlot.currentItem;
                    cookedSlot.currentItem = null;
                    cookedSlot.isCooking = false;
                    if (cookedSlot.progressBar != null) cookedSlot.progressBar.Show(false);
                    player.PickUpItem(itemToPick);
                    return;
                }
            }
        }

        private StoveSlot GetEmptySlot()
        {
            foreach (var slot in slots)
            {
                if (slot.currentItem == null) return slot;
            }
            return null;
        }

        private StoveSlot GetCookedSlot()
        {
            foreach (var slot in slots)
            {
                if (slot.currentItem != null && slot.currentItem.isPrepared) return slot;
            }
            return null;
        }

        public override string GetInteractPrompt(PlayerInteraction player)
        {
            StoveSlot cookedSlot = GetCookedSlot();
            if (cookedSlot != null)
            {
                if (player.HasItemHeld())
                {
                    KitchenItem held = player.GetHeldItem();
                    if (held?.ingredientData?.ingredientType == IngredientType.Meat && !held.isPrepared)
                    {
                        return "[ E ] Swap with New Raw Meat (Take Cooked Meat)";
                    }
                    return "Hands Full! Deliver item to Window or Trash first!";
                }
                return "[ E ] Take Cooked Meat";
            }

            StoveSlot emptySlot = GetEmptySlot();
            if (emptySlot != null && player.HasItemHeld() && player.GetHeldItem()?.ingredientData?.ingredientType == IngredientType.Meat && !player.GetHeldItem().isPrepared)
            {
                return "[ E ] Place Meat on Stove Slot";
            }

            int cookingCount = 0;
            foreach (var slot in slots)
            {
                if (slot.isCooking) cookingCount++;
            }

            if (cookingCount > 0)
            {
                return $"Stove Cooking ({cookingCount}/2 slots)...";
            }

            return "Stove (2 Slots Empty)";
        }
    }
}

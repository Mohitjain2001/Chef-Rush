using UnityEngine;
using YesChef.Data;
using YesChef.Items;
using YesChef.Managers;
using YesChef.Player;
using YesChef.UI;

namespace YesChef.Stations
{
    public class CustomerWindowStation : BaseStation
    {
        public int windowIndex;
        public OrderData currentOrder;

        public bool isRespawning = false;
        public float respawnTimer = 0f;
        public const float RESPAWN_DELAY = 5.0f;

        public CustomerWindowUI windowUI;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            {
                return;
            }

            if (currentOrder != null && !currentOrder.isCompleted)
            {
                currentOrder.timeActive += Time.deltaTime;
                if (windowUI != null)
                {
                    windowUI.UpdateOrderDisplay(currentOrder);
                }
            }
            else if (isRespawning)
            {
                respawnTimer += Time.deltaTime;
                if (windowUI != null)
                {
                    windowUI.ShowRespawning(RESPAWI_DELAY_REMAINING(respawnTimer));
                }

                if (respawnTimer >= RESPAWN_DELAY)
                {
                    isRespawning = false;
                    respawnTimer = 0f;
                    SpawnNewOrder();
                }
            }
        }

        private float RESPAWI_DELAY_REMAINING(float timer) => Mathf.Max(0, RESPAWN_DELAY - timer);

        public void AssignOrder(OrderData order)
        {
            currentOrder = order;
            isRespawning = false;
            respawnTimer = 0f;

            if (windowUI != null)
            {
                windowUI.UpdateOrderDisplay(currentOrder);
                windowUI.Show(true);
            }
        }

        public void SpawnNewOrder()
        {
            if (OrderManager.Instance != null)
            {
                OrderData newOrder = OrderManager.Instance.GenerateRandomOrder();
                AssignOrder(newOrder);
            }
        }

        public override void Interact(PlayerInteraction player)
        {
            if (player == null || !player.HasItemHeld()) return;
            if (currentOrder == null || currentOrder.isCompleted || isRespawning) return;

            KitchenItem held = player.GetHeldItem();
            if (held == null || held.ingredientData == null) return;

            // Check if held item is prepared (or Cheese which requires no prep)
            bool isPrepared = held.isPrepared || held.ingredientData.requiredStation == PrepStationType.None;

            if (!isPrepared)
            {
                // Unprepared ingredient cannot be placed
                return;
            }

            IngredientType type = held.ingredientData.ingredientType;

            if (currentOrder.NeedsIngredient(type))
            {
                // Deliver ingredient: consume held item
                KitchenItem consumed = player.DropItem();
                Destroy(consumed.gameObject);

                currentOrder.DeliverIngredient(type);

                if (windowUI != null)
                {
                    windowUI.UpdateOrderDisplay(currentOrder);
                }

                if (currentOrder.isCompleted)
                {
                    CompleteOrder();
                }
            }
            // If item is not required, it remains in hand!
        }

        private void CompleteOrder()
        {
            int score = currentOrder.CalculateFinalScore(OrderManager.Instance != null ? OrderManager.Instance.ingredientDefs : null);

            // Add score to GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(score);
            }

            // Floating text score popup near window
            if (windowUI != null)
            {
                windowUI.ShowScorePopup(score);
            }

            currentOrder = null;
            isRespawning = true;
            respawnTimer = 0f;
        }

        public override string GetInteractPrompt(PlayerInteraction player)
        {
            if (currentOrder == null || isRespawning)
            {
                return $"Window {windowIndex + 1} (Empty)";
            }

            if (player.HasItemHeld())
            {
                KitchenItem item = player.GetHeldItem();
                bool isPrepared = item != null && (item.isPrepared || item.ingredientData.requiredStation == PrepStationType.None);
                if (isPrepared && currentOrder.NeedsIngredient(item.ingredientData.ingredientType))
                {
                    return $"Deliver {item.ingredientData.ingredientName} to Window {windowIndex + 1}";
                }
                return $"Window {windowIndex + 1} (Item not needed)";
            }

            return $"Customer Window {windowIndex + 1}";
        }
    }
}

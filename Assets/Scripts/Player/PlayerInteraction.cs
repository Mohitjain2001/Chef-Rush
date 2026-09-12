using UnityEngine;
using YesChef.Items;
using YesChef.Managers;
using YesChef.Stations;
using YesChef.UI;

namespace YesChef.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float interactionRadius = 1.6f;
        public LayerMask stationLayer = ~0;

        [Header("Hold Settings")]
        public Transform holdPoint;
        private KitchenItem heldItem;

        private BaseStation selectedStation;

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying())
            {
                ClearSelection();
                return;
            }

            DetectClosestStation();

            // Interact key: E, Space, or Left Mouse Click
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (selectedStation != null)
                {
                    selectedStation.Interact(this);
                }
            }
        }

        private void DetectClosestStation()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius, stationLayer);
            BaseStation closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                BaseStation station = hit.GetComponent<BaseStation>() ?? hit.GetComponentInParent<BaseStation>();
                if (station != null)
                {
                    float dist = Vector3.Distance(transform.position, station.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = station;
                    }
                }
            }

            if (selectedStation != closest)
            {
                if (selectedStation != null) selectedStation.SetHighlight(false);
                selectedStation = closest;
                if (selectedStation != null) selectedStation.SetHighlight(true);
            }

            // Update interaction prompt UI
            if (UIManager.Instance != null)
            {
                if (selectedStation != null)
                {
                    UIManager.Instance.ShowInteractionPrompt(selectedStation.GetInteractPrompt(this));
                }
                else
                {
                    UIManager.Instance.HideInteractionPrompt();
                }
            }
        }

        private void ClearSelection()
        {
            if (selectedStation != null)
            {
                selectedStation.SetHighlight(false);
                selectedStation = null;
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideInteractionPrompt();
            }
        }

        public bool HasItemHeld() => heldItem != null;

        public KitchenItem GetHeldItem() => heldItem;

        public void PickUpItem(KitchenItem item)
        {
            if (item == null) return;
            heldItem = item;
            heldItem.transform.SetParent(holdPoint != null ? holdPoint : transform);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }

        public KitchenItem DropItem()
        {
            KitchenItem item = heldItem;
            heldItem = null;
            if (item != null)
            {
                item.transform.SetParent(null);
            }
            return item;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}

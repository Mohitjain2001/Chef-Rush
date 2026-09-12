using UnityEngine;
using YesChef.Player;

namespace YesChef.Stations
{
    public abstract class BaseStation : MonoBehaviour, IInteractable
    {
        [Header("Station Setup")]
        public string stationName = "Station";
        public Transform itemHoldPoint;

        protected MeshRenderer stationRenderer;
        protected Color originalColor;
        protected bool isHighlighted = false;

        protected virtual void Awake()
        {
            stationRenderer = GetComponent<MeshRenderer>();
            if (stationRenderer != null && stationRenderer.sharedMaterial != null)
            {
                originalColor = stationRenderer.sharedMaterial.color;
            }

            if (itemHoldPoint == null)
            {
                GameObject hp = new GameObject("ItemHoldPoint");
                hp.transform.SetParent(transform);
                hp.transform.localPosition = new Vector3(0, 0.6f, 0);
                itemHoldPoint = hp.transform;
            }
        }

        public abstract void Interact(PlayerInteraction player);
        public abstract string GetInteractPrompt(PlayerInteraction player);

        public virtual void SetHighlight(bool highlight)
        {
            isHighlighted = highlight;
            if (stationRenderer != null && stationRenderer.material != null)
            {
                stationRenderer.material.color = highlight ? Color.yellow : originalColor;
            }
        }
    }
}

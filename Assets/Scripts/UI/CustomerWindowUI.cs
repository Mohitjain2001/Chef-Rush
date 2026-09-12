using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Data;

namespace YesChef.UI
{
    public class CustomerWindowUI : MonoBehaviour
    {
        [Header("UI Components")]
        public Text orderTitleText;
        public Text timerText;
        public Text ingredientsText;
        public GameObject containerPanel;

        [Header("Floating Text Prefab / Anchor")]
        public Transform popupAnchor;

        public void Show(bool visible)
        {
            if (containerPanel != null) containerPanel.SetActive(visible);
            else gameObject.SetActive(visible);
        }

        public void UpdateOrderDisplay(OrderData order)
        {
            Show(true);

            if (orderTitleText != null)
            {
                orderTitleText.text = $"ORDER #{order.orderId}";
            }

            if (timerText != null)
            {
                int seconds = Mathf.FloorToInt(order.timeActive);
                timerText.text = $"Wait Time: {seconds}s";
                timerText.color = seconds > 15 ? new Color(1.0f, 0.35f, 0.35f) : new Color(0.3f, 0.95f, 1.0f);
            }

            if (ingredientsText != null)
            {
                // Format required vs fulfilled ingredients clearly
                Dictionary<IngredientType, int> reqCounts = new Dictionary<IngredientType, int>();
                foreach (var req in order.requiredIngredients)
                {
                    if (!reqCounts.ContainsKey(req)) reqCounts[req] = 0;
                    reqCounts[req]++;
                }

                Dictionary<IngredientType, int> fulCounts = new Dictionary<IngredientType, int>();
                foreach (var ful in order.fulfilledIngredients)
                {
                    if (!fulCounts.ContainsKey(ful)) fulCounts[ful] = 0;
                    fulCounts[ful]++;
                }

                string info = "";
                foreach (var kvp in reqCounts)
                {
                    int done = fulCounts.ContainsKey(kvp.Key) ? fulCounts[kvp.Key] : 0;
                    bool isDone = done >= kvp.Value;
                    string statusIcon = isDone ? "✔ " : "• ";
                    string nameStr = kvp.Key.ToString().ToUpper();
                    info += $"{statusIcon}{nameStr} : {done}/{kvp.Value}\n";
                }

                ingredientsText.text = info.TrimEnd();
            }
        }

        public void ShowRespawning(float remainingTime)
        {
            Show(true);
            if (orderTitleText != null) orderTitleText.text = "NEW ORDER";
            if (timerText != null) timerText.text = $"Spawning: {remainingTime:F1}s";
            if (ingredientsText != null) ingredientsText.text = "Waiting for customer...";
        }

        public void ShowScorePopup(int score)
        {
            GameObject popupObj = new GameObject("ScorePopupCanvas");
            popupObj.transform.position = (popupAnchor != null ? popupAnchor.position : transform.position) + Vector3.up * 1.5f;

            Canvas popupCanvas = popupObj.AddComponent<Canvas>();
            popupCanvas.renderMode = RenderMode.WorldSpace;
            popupCanvas.transform.localScale = Vector3.one * 0.015f;

            GameObject textObj = new GameObject("ScoreText");
            textObj.transform.SetParent(popupObj.transform, false);

            Text textComp = textObj.AddComponent<Text>();
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", 24);
            textComp.fontSize = 32;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.rectTransform.sizeDelta = new Vector2(300, 100);

            FloatingTextUI floating = popupObj.AddComponent<FloatingTextUI>();
            floating.textComponent = textComp;

            string textStr = (score >= 0) ? $"+{score}" : $"{score}";
            Color textCol = (score >= 0) ? new Color(0.2f, 0.95f, 0.4f) : new Color(1.0f, 0.3f, 0.3f);

            floating.Setup(textStr, textCol);
        }
    }
}

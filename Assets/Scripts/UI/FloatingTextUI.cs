using UnityEngine;
using UnityEngine.UI;

namespace YesChef.UI
{
    public class FloatingTextUI : MonoBehaviour
    {
        public Text textComponent;
        public float floatSpeed = 1.0f;
        public float fadeDuration = 1.5f;

        private float elapsed = 0f;
        private Color initialColor;

        public void Setup(string message, Color color)
        {
            if (textComponent == null) textComponent = GetComponentInChildren<Text>();
            if (textComponent != null)
            {
                textComponent.text = message;
                textComponent.color = color;
                initialColor = color;
            }
            elapsed = 0f;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            if (textComponent != null)
            {
                float alpha = Mathf.Clamp01(1.0f - (elapsed / fadeDuration));
                Color col = initialColor;
                col.a = alpha;
                textComponent.color = col;
            }

            if (elapsed >= fadeDuration)
            {
                Destroy(gameObject);
            }
        }
    }
}

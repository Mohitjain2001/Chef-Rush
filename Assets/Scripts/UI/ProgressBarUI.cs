using UnityEngine;
using UnityEngine.UI;

namespace YesChef.UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        public Image fillImage;
        public Canvas canvas;

        private void Awake()
        {
            if (canvas == null) canvas = GetComponentInChildren<Canvas>();
        }

        public void SetProgress(float progress)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(progress);
            }
        }

        public void Show(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}

using UnityEngine;
using YesChef.Data;

namespace YesChef.Items
{
    public class KitchenItem : MonoBehaviour
    {
        public IngredientSO ingredientData;
        public bool isPrepared = false;
        public bool isBeingPrepared = false;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public void Init(IngredientSO data, bool prepared = false)
        {
            ingredientData = data;
            isPrepared = prepared;

            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

            UpdateVisuals();
        }

        public void SetPrepared(bool prepared)
        {
            isPrepared = prepared;
            isBeingPrepared = false;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (ingredientData == null) return;

            PrimitiveType shape = isPrepared ? ingredientData.preparedPrimitiveShape : ingredientData.rawPrimitiveShape;
            Color col = isPrepared ? ingredientData.preparedColor : ingredientData.rawColor;
            Vector3 scale = isPrepared ? ingredientData.preparedScale : ingredientData.rawScale;

            GameObject tempPrimitive = GameObject.CreatePrimitive(shape);
            if (meshFilter != null) meshFilter.sharedMesh = tempPrimitive.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(tempPrimitive);

            if (meshRenderer != null)
            {
                Material mat = new Material(Shader.Find("Standard") ?? Shader.Find("Sprites/Default"));
                mat.color = col;
                meshRenderer.sharedMaterial = mat;
            }

            transform.localScale = scale;
        }
    }
}

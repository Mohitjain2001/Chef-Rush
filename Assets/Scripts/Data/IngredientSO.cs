using UnityEngine;

namespace YesChef.Data
{
    public enum IngredientType
    {
        Vegetable,
        Cheese,
        Meat
    }

    public enum PrepStationType
    {
        None,
        CuttingTable,
        Stove
    }

    [CreateAssetMenu(fileName = "NewIngredient", menuName = "YesChef/Ingredient Data")]
    public class IngredientSO : ScriptableObject
    {
        [Header("Basic Info")]
        public IngredientType ingredientType;
        public string ingredientName;
        public int scoreValue;

        [Header("Preparation")]
        public PrepStationType requiredStation;
        public float prepTime = 0f; // Seconds required to prepare

        [Header("Visual Colors & Models")]
        public Color rawColor = Color.white;
        public Color preparedColor = Color.white;
        public PrimitiveType rawPrimitiveShape = PrimitiveType.Cube;
        public PrimitiveType preparedPrimitiveShape = PrimitiveType.Cube;
        public Vector3 rawScale = Vector3.one * 0.4f;
        public Vector3 preparedScale = Vector3.one * 0.4f;
    }
}

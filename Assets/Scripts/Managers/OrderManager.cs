using System.Collections.Generic;
using UnityEngine;
using YesChef.Data;
using YesChef.Stations;

namespace YesChef.Managers
{
    public class OrderManager : MonoBehaviour
    {
        public static OrderManager Instance { get; private set; }

        [Header("Customer Windows")]
        public CustomerWindowStation[] customerWindows = new CustomerWindowStation[4];

        [Header("Ingredient Definitions")]
        public IngredientSO vegetableSO;
        public IngredientSO cheeseSO;
        public IngredientSO meatSO;

        public Dictionary<IngredientType, IngredientSO> ingredientDefs = new Dictionary<IngredientType, IngredientSO>();

        private int nextOrderId = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitIngredientDefs();
        }

        private void InitIngredientDefs()
        {
            ingredientDefs.Clear();

            if (vegetableSO == null) vegetableSO = CreateDefaultSO(IngredientType.Vegetable, "Vegetable", 20, PrepStationType.CuttingTable, 2.0f, Color.green, new Color(0.4f, 0.9f, 0.4f), PrimitiveType.Sphere, PrimitiveType.Cube, new Vector3(0.35f, 0.35f, 0.35f), new Vector3(0.35f, 0.15f, 0.35f));
            if (cheeseSO == null) cheeseSO = CreateDefaultSO(IngredientType.Cheese, "Cheese", 10, PrepStationType.None, 0.0f, Color.yellow, Color.yellow, PrimitiveType.Cube, PrimitiveType.Cube, new Vector3(0.35f, 0.35f, 0.35f), new Vector3(0.35f, 0.35f, 0.35f));
            if (meatSO == null) meatSO = CreateDefaultSO(IngredientType.Meat, "Meat", 30, PrepStationType.Stove, 6.0f, new Color(0.8f, 0.2f, 0.2f), new Color(0.4f, 0.2f, 0.1f), PrimitiveType.Cube, PrimitiveType.Cube, new Vector3(0.4f, 0.25f, 0.4f), new Vector3(0.4f, 0.25f, 0.4f));

            ingredientDefs[IngredientType.Vegetable] = vegetableSO;
            ingredientDefs[IngredientType.Cheese] = cheeseSO;
            ingredientDefs[IngredientType.Meat] = meatSO;
        }

        private IngredientSO CreateDefaultSO(IngredientType type, string name, int score, PrepStationType station, float prepTime, Color rawCol, Color prepCol, PrimitiveType rawShape, PrimitiveType prepShape, Vector3 rawScale, Vector3 prepScale)
        {
            IngredientSO so = ScriptableObject.CreateInstance<IngredientSO>();
            so.ingredientType = type;
            so.ingredientName = name;
            so.scoreValue = score;
            so.requiredStation = station;
            so.prepTime = prepTime;
            so.rawColor = rawCol;
            so.preparedColor = prepCol;
            so.rawPrimitiveShape = rawShape;
            so.preparedPrimitiveShape = prepShape;
            so.rawScale = rawScale;
            so.preparedScale = prepScale;
            return so;
        }

        public void InitializeOrders()
        {
            InitIngredientDefs();

            for (int i = 0; i < customerWindows.Length; i++)
            {
                if (customerWindows[i] != null)
                {
                    customerWindows[i].windowIndex = i;
                    OrderData newOrder = GenerateRandomOrder();
                    customerWindows[i].AssignOrder(newOrder);
                }
            }
        }

        public OrderData GenerateRandomOrder()
        {
            // 50% chance 2 ingredients, 50% chance 3 ingredients
            int itemCount = (Random.value < 0.5f) ? 2 : 3;
            List<IngredientType> ingredients = new List<IngredientType>();

            IngredientType[] availableTypes = new IngredientType[] { IngredientType.Vegetable, IngredientType.Cheese, IngredientType.Meat };

            for (int i = 0; i < itemCount; i++)
            {
                IngredientType randomType = availableTypes[Random.Range(0, availableTypes.Length)];
                ingredients.Add(randomType);
            }

            return new OrderData(nextOrderId++, ingredients);
        }
    }
}

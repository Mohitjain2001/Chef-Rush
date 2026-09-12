using System.Collections.Generic;
using UnityEngine;

namespace YesChef.Data
{
    [System.Serializable]
    public class OrderData
    {
        public int orderId;
        public List<IngredientType> requiredIngredients = new List<IngredientType>();
        public List<IngredientType> fulfilledIngredients = new List<IngredientType>();
        public float timeActive = 0f;
        public bool isCompleted = false;

        public OrderData(int id, List<IngredientType> ingredients)
        {
            orderId = id;
            requiredIngredients = new List<IngredientType>(ingredients);
            fulfilledIngredients = new List<IngredientType>();
            timeActive = 0f;
            isCompleted = false;
        }

        public bool NeedsIngredient(IngredientType type)
        {
            if (isCompleted) return false;

            // Count how many of this type are required vs already fulfilled
            int requiredCount = 0;
            foreach (var req in requiredIngredients)
            {
                if (req == type) requiredCount++;
            }

            int fulfilledCount = 0;
            foreach (var ful in fulfilledIngredients)
            {
                if (ful == type) fulfilledCount++;
            }

            return fulfilledCount < requiredCount;
        }

        public bool DeliverIngredient(IngredientType type)
        {
            if (NeedsIngredient(type))
            {
                fulfilledIngredients.Add(type);
                if (fulfilledIngredients.Count >= requiredIngredients.Count)
                {
                    isCompleted = true;
                }
                return true;
            }
            return false;
        }

        public int CalculateFinalScore(Dictionary<IngredientType, IngredientSO> ingredientDefs)
        {
            int baseScore = 0;
            foreach (var req in requiredIngredients)
            {
                if (ingredientDefs.TryGetValue(req, out var def))
                {
                    baseScore += def.scoreValue;
                }
                else
                {
                    // Fallback score values according to spec
                    switch (req)
                    {
                        case IngredientType.Vegetable: baseScore += 20; break;
                        case IngredientType.Cheese: baseScore += 10; break;
                        case IngredientType.Meat: baseScore += 30; break;
                    }
                }
            }

            int secondsElapsed = Mathf.FloorToInt(timeActive);
            return baseScore - secondsElapsed;
        }
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FateGames.ArcadeIdle
{
    public abstract class Machine : MonoBehaviour
    {
        [SerializeField] private float productionPeriod = 1f;
        [SerializeField] protected Ingredient[] ingredients;
        [SerializeField] protected ItemStack outcomeStack;
        protected Dictionary<string, Ingredient> ingredientDictionary = new Dictionary<string, Ingredient>();
        private float lastProductionTime = -100;
        protected UnityEvent<Stackable> onProduct = new UnityEvent<Stackable>();

        [System.Serializable]
        public class Ingredient
        {
            [SerializeField] private string tag;
            [SerializeField] private ItemStack stack;
            [SerializeField] private int requiredQuantity;

            public string Tag { get => tag; }
            public ItemStack Stack { get => stack; }
            public int RequiredQuantity { get => requiredQuantity; }
        }

        private void InitializeDictionary()
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                Ingredient ingredient = ingredients[i];
                ingredientDictionary.Add(ingredient.Tag, ingredient);
            }
        }

        protected void Awake()
        {
            InitializeDictionary();
            void CheckAndProduce(Stackable item)
            {
                if (CanProduce())
                {
                    Stackable product = Produce();
                    lastProductionTime = Time.time;
                    onProduct.Invoke(product);
                    DOVirtual.DelayedCall(productionPeriod, () => { CheckAndProduce(null); });
                }
            }
            foreach (Ingredient ingredient in ingredientDictionary.Values)
                ingredient.Stack.OnAdd.AddListener(CheckAndProduce);
        }

        private bool CanProduce()
        {
            if (!outcomeStack.CanPush || Time.time < lastProductionTime + productionPeriod) return false;
            bool result = true;
            foreach (Ingredient ingredient in ingredientDictionary.Values)
            {
                result = ingredient.Stack.Size >= ingredient.RequiredQuantity;
                if (!result)
                    break;
            }
            return result;
        }

        public void TransferIngredient(string tag, ItemStack itemStack, bool audio = false, bool overrideWave = false)
        {
            itemStack.Transfer(ingredientDictionary[tag].Stack, audio, overrideWave);
        }

        protected abstract Stackable Produce();


    }
}

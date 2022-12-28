using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FateGames;

namespace FateGames
{
    public class UIMoney : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private RectTransform moneyImageTransform;

        public RectTransform MoneyImageTransform { get => moneyImageTransform; }

        private void Awake()
        {
            PlayerProgression.OnMoneyChanged.AddListener(UpdateMoneyText);
        }

        private void UpdateMoneyText(int money)
        {
            string text;
            if (money >= 1000000 && money - (money / 1000000) * 1000000 >= 10000) text = (money / 1000000f).ToString("0.00").Replace(',', '.') + "M";
            else if (money >= 1000000) text = (money / 1000000f).ToString("0").Replace(',', '.') + "M";
            else if (money >= 10000 && money - (money / 10000) * 10000 >= 100) text = (money / 1000f).ToString("0.0").Replace(',', '.') + "K";
            else if (money >= 10000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
            else if (money >= 1000 && money - (money / 1000) * 1000 >= 10) text = (money / 1000f).ToString("0.00").Replace(',', '.') + "K";
            else if (money >= 1000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
            else text = money.ToString();
            moneyText.text = text;
        }
    }

}
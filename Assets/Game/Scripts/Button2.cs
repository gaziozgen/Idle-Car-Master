using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using TMPro;

public class Button2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText = null;
    [SerializeField] private Image miniArea = null;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color passiveColor;

    private int money = 0;

    public void SetMoney(int money)
    {
        this.money = money;
        if (money != 0)
            moneyText.text = IntToString(money);
        else
            moneyText.text = "Free";
    }

    public void UpdateAffordability()
    {
        if (money <= PlayerProgression.MONEY)
        {
            miniArea.color = new Color(activeColor.r, activeColor.g, activeColor.b);
        }
        else
        {
            miniArea.color = new Color(passiveColor.r, passiveColor.g, passiveColor.b);
        }
    }

    static public string IntToString(int money)
    {
        string text;
        if (money >= 1000000 && money - (money / 1000000) * 1000000 >= 10000) text = (money / 1000000f).ToString("0.00").Replace(',', '.') + "M";
        else if (money >= 1000000) text = (money / 1000000f).ToString("0").Replace(',', '.') + "M";
        else if (money >= 10000 && money - (money / 10000) * 10000 >= 100) text = (money / 1000f).ToString("0.0").Replace(',', '.') + "K";
        else if (money >= 10000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
        else if (money >= 1000 && money - (money / 1000) * 1000 >= 10) text = (money / 1000f).ToString("0.00").Replace(',', '.') + "K";
        else if (money >= 1000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
        else text = money.ToString();
        return text;
    }
}

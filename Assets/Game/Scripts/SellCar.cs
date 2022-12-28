using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using TMPro;

public class SellCar : MonoBehaviour
{
    [SerializeField] private Transform otherCars = null;
    [SerializeField] private TextMeshProUGUI statusText = null;
    [SerializeField] private TextMeshProUGUI marketMoneyText = null;
    [SerializeField] private TextMeshProUGUI lossText = null;
    [SerializeField] private Button2 sellButton = null;

    private int offer = 0;

    public void Sell()
    {
        HapticManager.DoHaptic();
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        Car car = slot.Car;

        GarageManager garage = GarageManager.Instance;

        garage.Income(offer, slot.Transform);

        car.CleanForRest();
        slot.RemoveCar();

        car.AlignForPlace(otherCars);

        garage.SetLayerRecursively(car.Transform, 0);
        garage.ConfirmSell();
        garage.Home();
        garage.CheckEmptySlots();
    }

    public void CalculateSell()
    {
        HapticManager.DoHaptic();
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();

        float ratio = slot.GetStatus();

        string status = "";
        float loss = -1;

        if (ratio >= 1)
        {
            status = "Old";
            loss = 30;
        }
        else if (ratio >= 0.1f)
        {
            status = "Used";
            loss = ratio * 30f;
        }
        else
        {
            status = "New";
            loss = 0;
        }

        statusText.text = status;
        marketMoneyText.text = slot.Car.GetPrice().ToString();
        lossText.text = "%" + loss.ToString("F1");

        offer = (int)(slot.Car.GetPrice() * (100 - loss)/100);
        sellButton.SetMoney(offer);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using TMPro;

public class BuyCar : MonoBehaviour
{
    public static BuyCar Instance { get; private set; }

    [SerializeField] private float turnSpeed = 10;
    [SerializeField] private Transform platform = null;
    [SerializeField] private Transform carPoint = null;
    [SerializeField] private Transform otherCars = null;
    //[SerializeField] private TextMeshProUGUI price = null;
    [SerializeField] private TextMeshProUGUI speed = null;
    [SerializeField] private TextMeshProUGUI income = null;
    [SerializeField] private Button2 buyButton = null;
    [SerializeField] private GameObject tutorialHand2 = null;
    [SerializeField] private GameObject tutorialHand0 = null;

    private List<Car> cars = null;
    private Car selectedCar = null;
 
    void Awake()
    {
        Instance = this;

        cars = new List<Car>();

        for (int i = 0; i < otherCars.childCount; i++)
        {
            cars.Add(otherCars.GetChild(i).GetComponent<Car>());
        }
    }


    void Update()
    {
        platform.localRotation = Quaternion.Euler(platform.localRotation.eulerAngles + Time.deltaTime * Vector3.up * turnSpeed);
    }

    public Car GetCarById(int id)
    {
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].CarId == id)
            {
                Car car = cars[i];
                cars.Remove(car);
                return car;
            }
        }
        return null;
    }

    public int GetSelectedCarPrice()
    {
        return selectedCar.GetPrice();
    }


    public void Buy()
    {
        if (PlayerProgression.MONEY >= selectedCar.GetPrice() || PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
        {
            HapticManager.DoHaptic();

            if (PlayerProgression.PlayerData.BestAchivedCarLevel != 0)
                PlayerProgression.MONEY -= selectedCar.GetPrice();
            else
            {
                tutorialHand2.SetActive(false);
                tutorialHand0.SetActive(true);
            }

            if (selectedCar.GetCarType() > PlayerProgression.PlayerData.BestAchivedCarLevel)
                PlayerProgression.PlayerData.BestAchivedCarLevel = selectedCar.GetCarType();

            GarageManager garage = GarageManager.Instance;
            garage.PlaceBuyedCar(selectedCar);
            selectedCar = null;
            garage.ClosePanel();
            garage.CheckEmptySlots();
        }
    }

    public void Refresh(bool reset)
    {
        HapticManager.DoHaptic();

        int previousCarType = -1;
        int previousCarIndex = -1;

        if (selectedCar)
        {
            selectedCar.AlignForPlace(otherCars);
            previousCarIndex = cars.IndexOf(selectedCar);
            previousCarType = selectedCar.GetCarType();
        }

        if (reset)
        {
            previousCarIndex = cars.Count;
            previousCarType = -1;
        }

        for (int i = 1; i <= cars.Count; i++)
        {
            Car car = cars[(previousCarIndex + cars.Count - i) % cars.Count];
            if ((!car.IsOwned() && car.GetCarType() != previousCarType && car.GetCarType() <= PlayerProgression.PlayerData.BestAchivedCarLevel + 1))
            {
                selectedCar = car;
                break;
            }
        }

        selectedCar.AlignForPlace(carPoint);
        UpdateCarInfo();
    }

    private void UpdateCarInfo()
    {
        speed.text = ((int)selectedCar.GetFinalSpeed()).ToString() + " MPH";
        income.text = selectedCar.GetIncome().ToString() + "$";

        if (PlayerProgression.PlayerData.BestAchivedCarLevel != 0)
            buyButton.SetMoney(selectedCar.GetPrice());
        else
            buyButton.SetMoney(0);

        buyButton.UpdateAffordability();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using TMPro;

public class UpgradeCar : MonoBehaviour
{
    public static UpgradeCar Instance { get; private set; }

    public bool locked { get; private set; } = false;

    //[SerializeField] private TextMeshProUGUI price = null;
    [SerializeField] private TextMeshProUGUI incomeText = null;
    [SerializeField] private TextMeshProUGUI baseSpeedText = null;
    [SerializeField] private TextMeshProUGUI maxSpeedText = null;
    [SerializeField] private TextMeshProUGUI plusIncomeText = null;
    [SerializeField] private TextMeshProUGUI plusBaseSpeedText = null;
    [SerializeField] private TextMeshProUGUI plusMaxSpeedText = null;
    [SerializeField] private Button2 confirmUpgradeButton = null;

    private int income = 0;
    private int baseSpeed = 0;
    private int maxSpeed = 0;
    private int plusIncome = 0;
    private int plusBaseSpeed = 0;
    private int plusMaxSpeed = 0;

    private int cost = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void Upgrade()
    {
        if (!locked)
        {
            if (PlayerProgression.MONEY >= cost)
            {
                CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
                Car car = slot.Car;

                HapticManager.DoHaptic();
                PlayerProgression.MONEY -= cost;
                CameraFocusController.Instance.Focus.GetComponent<CarSlot>().Car.UpgradeMesh();
                confirmUpgradeButton.gameObject.SetActive(false);
                locked = true;

                slot.ConfirmUpgrade();

                DOTween.To((val) =>
                {
                    if (locked)
                        incomeText.text = val.ToString("F0");
                }, income, income + plusIncome, 1);

                DOTween.To((val) =>
                {
                    if (locked)
                        baseSpeedText.text = val.ToString("F0");
                }, baseSpeed, baseSpeed + plusBaseSpeed, 1);

                DOTween.To((val) =>
                {
                    if (locked)
                        maxSpeedText.text = val.ToString("F0");
                }, maxSpeed, maxSpeed + plusMaxSpeed, 1);

                DOTween.To((val) =>
                {
                    if (locked)
                        plusIncomeText.text = "+ " + val.ToString("F0");
                }, plusIncome, 0, 1);

                DOTween.To((val) =>
                {
                    if (locked)
                        plusBaseSpeedText.text = "+ " + val.ToString("F0");
                }, plusBaseSpeed, 0, 1);

                DOTween.To((val) =>
                {
                    if (locked)
                        plusMaxSpeedText.text = "+ " + val.ToString("F0");
                }, plusMaxSpeed, 0, 1);

                DOVirtual.DelayedCall(1.1f, () =>
                {
                    //GarageManager.Instance.ConfirmUpgrade();
                    confirmUpgradeButton.gameObject.SetActive(true);
                    locked = false;

                    if (car.GetUpgradeLevel() < car.GetMaxUpgradeLevel())
                        PrepareUpgrade();
                    else
                    {
                        GarageManager.Instance.BackFromUpgradeCar();
                    }
                });
                

            }
            else
                print("not enough money");
        }
        else
            print("locked");
        
    }

    public void PrepareUpgrade()
    {
        HapticManager.DoHaptic();
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        slot.UpdateUILevel(true);
        Car car = slot.Car;
        //price.text = car.GetUpgradeCost().ToString() + "$";

        income = car.GetIncome();
        baseSpeed = car.GetBaseSpeed();
        maxSpeed = car.GetMaxSpeed();
        plusIncome = car.GetUpgradeIncomeChange();
        plusBaseSpeed = car.GetUpgradeBaseSpeedChange();
        plusMaxSpeed = car.GetUpgradeMaxSpeedChange();

        incomeText.text = income.ToString();
        baseSpeedText.text = baseSpeed.ToString();
        maxSpeedText.text = maxSpeed.ToString();
        plusIncomeText.text = "+ " + plusIncome;
        plusBaseSpeedText.text = "+ " + plusBaseSpeed;
        plusMaxSpeedText.text = "+ " + plusMaxSpeed;

        cost = car.GetUpgradeCost();
        confirmUpgradeButton.SetMoney(cost);
        confirmUpgradeButton.UpdateAffordability();
    }

}

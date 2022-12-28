using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Car : MonoBehaviour
{
    public Transform Transform { get; private set; }
    public int CarId { get; private set; } = -1;

    [SerializeField] private int id = 0;
    [SerializeField] private int typeNo = 0;
    [SerializeField] private int basePrice = 200;
    [SerializeField] private float milePerHour  = 100;
    [SerializeField] private int income = 10;

    private CarSlot slot = null;
    private float currentSpeed = 1;
    //private bool continueBaseCarMove = true;
    private float[] speedBonusTimes = { -10, -10, -10, -10, -10 };
    [SerializeField] private Animator[] animationControllers = null;

    #region upgrade variables
    [SerializeField] private Transform[] upgrade1In = null;
    [SerializeField] private Transform[] upgrade1Out = null;
    [SerializeField] private Transform[] upgrade2In = null;
    [SerializeField] private Transform[] upgrade2Out = null;
    [SerializeField] private Transform[] upgrade3In = null;
    [SerializeField] private Transform[] upgrade3Out = null;
    [SerializeField] private Transform[] upgrade4In = null;
    [SerializeField] private Transform[] upgrade4Out = null;
    [SerializeField] private Transform[] upgrade5In = null;
    [SerializeField] private Transform[] upgrade5Out = null;
    private List<Transform[]> upgrades = null;
    #endregion

    #region static variables
    static private int maxUpgradeCount = 5;
    static private float acceleration = 5;
    static private float bonusDuration = 1f;
    static private float speedToAnimationSpeedMultiplier = 0.02f;
    static private float upgradeSpeedBonusMultiplier = 0.1f;
    static private float tapSpeedBonusMultiplier = 0.1f;
    static private float upgradeValueMultiplyRatio = 1.5f;
    static private float baseUpgradeValueRatio = 0.2f;
    static private float incomeBonusPerUpgradeRatio = 0.1f;
    #endregion

    void Awake()
    {
        CarId = id;
        Transform = transform;
        if (!slot)
            SetAnimationSpeed(0);

        #region assign upgrade parts arrays
        upgrades = new List<Transform[]>();
        upgrades.Add(upgrade1In);
        upgrades.Add(upgrade1Out);
        upgrades.Add(upgrade2In);
        upgrades.Add(upgrade2Out);
        upgrades.Add(upgrade3In);
        upgrades.Add(upgrade3Out);
        upgrades.Add(upgrade4In);
        upgrades.Add(upgrade4Out);
        upgrades.Add(upgrade5In);
        upgrades.Add(upgrade5Out);
        #endregion
    }


    #region Speed
    public void SpeedUp()
    {
        HapticManager.DoHaptic();
        float min = float.MaxValue;
        int index = 0;
        for (int i = 0; i < speedBonusTimes.Length; i++)
        {
            if (speedBonusTimes[i] < min)
            {
                min = speedBonusTimes[i];
                index = i;
            }
        }
        EnvironmentRoadController.Instance.Wind();
        speedBonusTimes[index] = Time.time + bonusDuration;
    }

    private float GetSpeed()
    {
        int totalBonusCount = 0;
        for (int i = 0; i < speedBonusTimes.Length; i++)
        {
            if (speedBonusTimes[i] >= Time.time)
                totalBonusCount += 1;
        }

        return milePerHour * (tapSpeedBonusMultiplier * totalBonusCount + 1) * (upgradeSpeedBonusMultiplier * slot.slotData.UpgradeLevel + 1);
    }

    public void CalculateSpeedAndInformToSlot()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, GetSpeed(), Time.deltaTime * acceleration);
        SetAnimationSpeed(currentSpeed * speedToAnimationSpeedMultiplier); 
        slot.UpdateCarSpeed(currentSpeed);
    }
    #endregion





    #region Upgrade
    public int GetUpgradeLevel()
    {
        return slot.slotData.UpgradeLevel + 1;
    }

    public int GetMaxUpgradeLevel()
    {
        return maxUpgradeCount + 1;
    }

    public int GetUpgradeCost()
    {
        return (int)(basePrice * baseUpgradeValueRatio * Mathf.Pow(upgradeValueMultiplyRatio, slot.slotData.UpgradeLevel));
    }

    public int GetMaxSpeed()
    {
        return (int)(milePerHour * (tapSpeedBonusMultiplier * speedBonusTimes.Length + 1) * (upgradeSpeedBonusMultiplier * slot.slotData.UpgradeLevel + 1));
    }

    public int GetBaseSpeed()
    {
        if (!slot)
            return (int)milePerHour;

        return (int)(milePerHour * (upgradeSpeedBonusMultiplier * slot.slotData.UpgradeLevel + 1));
    }

    public int GetUpgradeBaseSpeedChange()
    {
        return (int)(upgradeSpeedBonusMultiplier * milePerHour);
    }

    public int GetUpgradeMaxSpeedChange()
    {
        return (int)(upgradeSpeedBonusMultiplier * milePerHour * (tapSpeedBonusMultiplier * speedBonusTimes.Length + 1));
    }

    public int GetUpgradeIncomeChange()
    {
        return (int)(income * incomeBonusPerUpgradeRatio);
    }

    public void UpgradeMesh()
    {
        /*continueBaseCarMove = false;
        animationControllers[0].speed = 0;*/

        UpgradeParts(upgrades[slot.slotData.UpgradeLevel * 2], upgrades[slot.slotData.UpgradeLevel * 2 + 1]);
    }

    public void ReverseMesh()
    {
        /*continueBaseCarMove = true;*/

        RevertUpgradeParts(upgrades[slot.slotData.UpgradeLevel * 2], upgrades[slot.slotData.UpgradeLevel * 2 + 1]);
    }

    public void FastUpgrade()
    {
        for (int i = 0; i < slot.slotData.UpgradeLevel; i++)
        {
            for (int j = 0; j < upgrades[i*2].Length; j++)
            {
                upgrades[i * 2][j].gameObject.SetActive(true);
            }

            for (int j = 0; j < upgrades[i*2 +1].Length; j++)
            {
                upgrades[i * 2+1][j].gameObject.SetActive(false);
            }
        }
    }

    private void UpgradeParts(Transform[] ins, Transform[] outs)
    {
        for (int i = 0; i < outs.Length; i++)
        {
            outs[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < ins.Length; i++)
        {
            ins[i].gameObject.SetActive(true);
            ins[i].GetComponent<Animator>().SetTrigger("Bounce");
        }
    }

    private void RevertUpgradeParts(Transform[] ins, Transform[] outs)
    {
        for (int i = 0; i < ins.Length; i++)
        {
            ins[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < outs.Length; i++)
        {
            outs[i].gameObject.SetActive(true);
        }
    }
    #endregion





    #region Sell & Buy
    public int GetPrice()
    {
        if (!slot)
            return basePrice;

        int total = basePrice;

        for (int i = 0; i < slot.slotData.UpgradeLevel; i++)
        {
            total += (int)(basePrice * baseUpgradeValueRatio * Mathf.Pow(upgradeValueMultiplyRatio, i));
        }

        return total;
        
    }

    public void SetSlot(CarSlot slot)
    {
        this.slot = slot;
    }

    public void CleanForRest()
    {
        currentSpeed = 1;
        //continueBaseCarMove = true;

        for (int i = 0; i < animationControllers.Length; i++)
        {
            animationControllers[i].enabled = false;
            animationControllers[i].enabled = true;
        }
        SetAnimationSpeed(0);

        for (int i = 0; i < speedBonusTimes.Length; i++)
        {
            speedBonusTimes[i] = -10;
        }

        for (int i = slot.slotData.UpgradeLevel-1; i >= 0; i--)
        {
            RevertUpgradeParts(upgrades[i * 2], upgrades[i * 2 + 1]);
        }

        slot = null;
    }

    #endregion





    #region Helper

    public int GetIncome()
    {
        if (!slot)
            return income;

        return (int)(income * (slot.slotData.UpgradeLevel * incomeBonusPerUpgradeRatio + 1));
    }

    public int GetCarType()
    {
        return typeNo;
    }

    public float GetCurrentMaxCarSpeed()
    {
        return milePerHour * (tapSpeedBonusMultiplier * speedBonusTimes.Length + 1) * (upgradeSpeedBonusMultiplier * slot.slotData.UpgradeLevel + 1);
    }

    public float GetFinalSpeed()
    {
        return milePerHour * (tapSpeedBonusMultiplier * speedBonusTimes.Length + 1) * (upgradeSpeedBonusMultiplier * maxUpgradeCount + 1);
    }

    private void SetAnimationSpeed(float speed)
    {
        for (int i = 0; i < animationControllers.Length; i++)
        {
            /*if (i == 0 && !continueBaseCarMove)
                continue;*/

            animationControllers[i].speed = speed;
        }
    }

    public void AlignForPlace(Transform place)
    {
        Transform.parent = place;
        Transform.localPosition = Vector3.zero;
        Transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public bool IsOwned()
    {
        return slot;
    }

    #endregion

}

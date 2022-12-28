using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;
using TMPro;
using ToonyColorsPro;
using DG.Tweening;

public class CarSlot : MonoBehaviour
{
    public Transform Transform { get; private set; }
    public int Price { get; private set; }
    public Car Car { get; private set; }  = null;

    public PlayerData.SlotData slotData { get; private set; }

    [SerializeField] private int id = 0;
    [SerializeField] private int price = 0;
    [SerializeField] private bool free = false;


    #region UI
    [SerializeField] private GameObject lockTexture = null;
    [SerializeField] private TextMeshProUGUI priceText = null;
    [SerializeField] private MeshRenderer speedBandTexture = null;
    [SerializeField] private GameObject mesh = null;
    [SerializeField] private GameObject emptySprite = null;
    [SerializeField] private Transform carPoint = null;
    [SerializeField] private GameObject detailCarUI = null;
    [SerializeField] private GameObject carUI = null;
    [SerializeField] private TextMeshProUGUI speed = null;
    [SerializeField] private RectTransform speedCursor = null;
    [SerializeField] private TextMeshProUGUI totalRoad = null;
    [SerializeField] private TextMeshProUGUI income = null;
    [SerializeField] private TextMeshProUGUI upgradeLevel = null;
    [SerializeField] private RectTransform level = null;
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private GameObject tutorialHand0 = null;
    #endregion


    private float currentMaxCarSpeed = 1;
    private float finalCarSpeed = 1;
    private float speedBandOffset = 0;

    static private float incomeRange = 1;
    static private float timeMultiplier = 10;
    static private float slotSpeedMultiplier = 0.01f;
    static private float normalLevelPosition = 250;
    static private float focusedLevelPosition = 450f;

    void Awake()
    {
        Transform = transform;
        Price = price;
        priceText.text = price.ToString();
        Initialize();

        bool opened = IsOpened();

        SetMeshVisible(opened);
        setLocked(!opened);

        if (tutorialHand0)
            tutorialHand0.SetActive(PlayerProgression.PlayerData.BestAchivedCarLevel == 0);
    }

    public void Initialize()
    {
        for (int i = 0; i < PlayerProgression.PlayerData.SlotsData.Count; i++)
        {
            if (PlayerProgression.PlayerData.SlotsData[i].SlotId == id)
            {
                slotData = PlayerProgression.PlayerData.SlotsData[i];
                print("found");

                if (slotData.CarId != -1)
                {
                    Car car = BuyCar.Instance.GetCarById(slotData.CarId);
                    if (car)
                    {
                        car.SetSlot(this);
                        PlaceCar(car);
                        car.FastUpgrade(); 
                    }
                    else
                        print("car not found");
                }

                return;
            }
        }
        print("not found");

        slotData = new(id);
        PlayerProgression.PlayerData.SlotsData.Add(slotData);
    }


    private void Update()
    {
        if (Car)
            Car.CalculateSpeedAndInformToSlot();
    }


    #region Open & Place & Remove
    public void Open()
    {
        slotData.Opened = true;
        setLocked(false);
        SetMeshVisible(true);
        emptySprite.SetActive(false);
        mesh.GetComponent<Animator>().SetTrigger("Bounce");
        HapticManager.DoHaptic();

        if (tutorialHand0 && tutorialHand0.activeInHierarchy && PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
            tutorialHand0.SetActive(false);
    }

    public bool IsOpened()
    {
        return slotData.Opened;
    }

    public void PlaceCar(Car car)
    {
        slotData.CarId = car.CarId;

        Car = car;
        car.SetSlot(this);
        car.AlignForPlace(carPoint);
        carUI.SetActive(true);
        UpdateUILevel(false);
        UpdateIncome();
        finalCarSpeed = Car.GetFinalSpeed();
        currentMaxCarSpeed = Car.GetCurrentMaxCarSpeed();
        RepositionLevel(false);
    }

    public void RemoveCar()
    {
        Car = null;
        slotData.CarId = -1;
        slotData.UpgradeLevel = 0;
        slotData.TotalMile = 0;
        carUI.SetActive(false);
    }

    private void setLocked(bool locked)
    {
        emptySprite.SetActive(locked);
        lockTexture.SetActive(locked);

        if (locked && free)
            priceText.text = "Free";

    }
    #endregion





    #region Upgrade
    public void ConfirmUpgrade()
    {
        slotData.UpgradeLevel += 1;
        currentMaxCarSpeed = Car.GetCurrentMaxCarSpeed();
        UpdateIncome();
        UpdateUILevel(false);
        GarageManager.Instance.CheckMaxUpgradeAndAvailability();
    }

    public void UpdateIncome()
    {
        income.text = Button2.IntToString(Car.GetIncome());
    }

    public void UpdateUILevel(bool plus)
    {
        string extra = "";
        if (plus)
            extra = "+1";

        upgradeLevel.text = "Level " + Car.GetUpgradeLevel().ToString() + extra;
    }
    #endregion





    #region Sell

    public float GetStatus()
    {
        return slotData.TotalMile / (currentMaxCarSpeed * timeMultiplier) / 0.5f;
    }

    #endregion





    #region Update UI with current car info
    public void UpdateCarSpeed(float currentSpeed)
    {
        float oldRoad = slotData.TotalMile;
        slotData.TotalMile += currentSpeed * timeMultiplier * Time.deltaTime / 3600;

        if ((int)(oldRoad / incomeRange) < (int)(slotData.TotalMile / incomeRange))
        {
            GarageManager.Instance.Income(Car.GetIncome(), Transform);
        }

        speed.text = currentSpeed.ToString("F0");
        totalRoad.text = Button2.IntToString((int) slotData.TotalMile);
        progressBar.value = (slotData.TotalMile % incomeRange) / incomeRange;

        UpdateSpeedCursor(currentSpeed);

        if (CameraFocusController.Instance.Focus == Transform)
            InfromSpeedToEnvironment(currentSpeed);

        speedBandOffset += Time.deltaTime * slotSpeedMultiplier * currentSpeed;
        speedBandTexture.materials[1].mainTextureOffset = Vector2.down * speedBandOffset;
    }

    private void UpdateSpeedCursor(float currentSpeed)
    {
        float ratio = (currentSpeed / finalCarSpeed) * -230;
        speedCursor.localRotation = Quaternion.Euler(Vector3.forward * ratio);
    }

    private void InfromSpeedToEnvironment(float speed)
    {
        EnvironmentRoadController.Instance.Speed = speed;
    }

    
    #endregion





    #region Garage Manager
    public void SpeedUp()
    {
        Car.SpeedUp();
    }

    public void SetDetailCarUI(bool x)
    {
        detailCarUI.SetActive(x);
    }

    public void SetProgressBarVisible(bool x)
    {
        progressBar.gameObject.SetActive(x);
    }

    public void SetMeshVisible(bool x)
    {
        mesh.SetActive(x);
    }

    public void RepositionLevel(bool up)
    {
        float target = normalLevelPosition;

        if (up)
            target = focusedLevelPosition;

        level.DOLocalMoveY(target + 6, 0.5f);
    }
    #endregion

}

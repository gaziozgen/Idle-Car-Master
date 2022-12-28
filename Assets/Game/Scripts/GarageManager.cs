using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using FateGames;

public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    [SerializeField] private CarSlot[] carSlots = null;

    [SerializeField] private RawImage environmentRoad = null;
    [SerializeField] private GameObject buyCarPlatform = null;

    [SerializeField] private Button1 buyCarButton = null;
    [SerializeField] private Button1 upgradeButton = null;
    [SerializeField] private Button2 confirmBuyButton = null;
    [SerializeField] private Button2 confirmUpgradeButton = null;

    [SerializeField] private GameObject carUI = null;
    [SerializeField] private GameObject panel = null;
    [SerializeField] private GameObject buyCarButtons = null;
    [SerializeField] private GameObject sellButtons = null;
    [SerializeField] private GameObject upgradeCarButtons = null;
    [SerializeField] private GameObject tapToSpeedUp = null;
    [SerializeField] private GameObject tutorialHand0 = null;
    [SerializeField] private GameObject tutorialHand1 = null;
    [SerializeField] private GameObject tutorialHand2 = null;


    private float lastSpeedUpTime = -1;
    private float tapAnimationComeBackTime = 3;

    void Awake()
    {
        Instance = this;

        environmentRoad.gameObject.SetActive(true);
        environmentRoad.DOFade(0, 0);
    }

    private void Start()
    {
        CheckEmptySlots();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.currentSelectedGameObject && !panel.activeSelf)
        {
            if (CameraFocusController.Instance.Focus)
            {
                CameraFocusController.Instance.Focus.GetComponent<CarSlot>().SpeedUp();
                lastSpeedUpTime = Time.time;

                if (tapToSpeedUp.activeInHierarchy)
                    tapToSpeedUp.SetActive(false);
            }

            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    CarSlot slot = hit.collider.gameObject.GetComponent<CarSlot>();

                    if (slot.Car)
                        Focus(slot);

                    else if (slot.IsOpened())
                        OpenBuyCar();

                    else if (PlayerProgression.MONEY >= slot.Price)
                    {
                        if (PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
                            tutorialHand1.SetActive(true);
                        OpenSlot(slot);
                    }
                }
            }
        }

        if (Time.time > lastSpeedUpTime + tapAnimationComeBackTime)
            tapToSpeedUp.SetActive(true);

    }

    public void Income(int income, Transform transform)
    {
        PlayerProgression.MONEY += income;
        print(income);

        UpdateBuyAndUpgradeAffordability();

        Transform focus = CameraFocusController.Instance.Focus;
        if ((!focus || focus == transform) && !panel.activeInHierarchy)
        {
            GameObject text = ObjectPooler.Instance.SpawnFromPool("LevitatingText", transform.position + new Vector3(0, 5, -5), CameraFocusController.Instance.Transform.rotation);
            text.GetComponent<LevitatingText>().SetText(income.ToString() + "$");
        }
    }


    #region validate buttons

    public void UpdateBuyAndUpgradeAffordability()
    {
        if (confirmBuyButton.gameObject.activeInHierarchy)
            confirmBuyButton.UpdateAffordability();

        else if (confirmUpgradeButton.gameObject.activeInHierarchy)
            confirmUpgradeButton.UpdateAffordability();

        else if (upgradeButton.gameObject.activeInHierarchy)
            CheckMaxUpgradeAndAvailability();
    }

    public void CheckMaxUpgradeAndAvailability()
    {
        Car car = CameraFocusController.Instance.Focus.GetComponent<CarSlot>().Car;
        if (car.GetUpgradeLevel() < car.GetMaxUpgradeLevel())
        {
            upgradeButton.UpdateLock(false);

            if (car.GetUpgradeCost() <= PlayerProgression.MONEY)
                upgradeButton.UpdateInfo(true);
            else
                upgradeButton.UpdateInfo(false);
        }
        else
        {
            upgradeButton.UpdateLock(true);
            upgradeButton.UpdateInfo(false);
        }
    }

    public void CheckEmptySlots()
    {
        if (OpenSlotExist())
            buyCarButton.UpdateLock(false);

        else
            buyCarButton.UpdateLock(true);
    }

    #endregion





    #region slot

    public void PlaceBuyedCar(Car car)
    {
        for (int i = 0; i < carSlots.Length; i++)
        {
            if (carSlots[i].IsOpened() && !carSlots[i].Car)
            {
                carSlots[i].PlaceCar(car);
                break;
            }
        }
    }

    public void OpenSlot(CarSlot slot)
    {
        if (CheckNotDeadGame(slot.Price))
        {
            PlayerProgression.MONEY -= slot.Price;
            slot.Open();
            CheckEmptySlots();
        }
        else
        {
            print("dead game control");
        }
        
    }

    private bool OpenSlotExist()
    {
        bool res = false;
        for (int i = 0; i < carSlots.Length; i++)
        {
            if (carSlots[i].IsOpened() && !carSlots[i].Car)
            {
                res = true;
            }
        }
        return res;
    }

    private bool CheckNotDeadGame(int slotCost)
    {
        if (PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
            return true;

        for (int i = 0; i < carSlots.Length; i++)
        {
            if (carSlots[i].Car)
                return true;
        }

        if (PlayerProgression.MONEY >= slotCost + 200)
            return true;

        return false;
    }

    #endregion





    #region UI open & close

    public void OpenBuyCar()
    {
        if (!OpenSlotExist())
        {
            print("there is no avaliable slot");
            return;
        }

        if (PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
        {
            tutorialHand1.SetActive(false);
            tutorialHand2.SetActive(true);
        }

        // UI
        buyCarButton.gameObject.SetActive(false);

        panel.SetActive(true);
        buyCarButtons.SetActive(true);
        buyCarPlatform.SetActive(true);

        // REST
        BuyCar.Instance.Refresh(true);
    }

    public void OpenSellCar()
    {
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        // UI
        carUI.SetActive(false);
        slot.SetDetailCarUI(false);
        slot.SetProgressBarVisible(false);

        panel.SetActive(true);
        sellButtons.SetActive(true);

        // REST
        GetComponent<SellCar>().CalculateSell();
    }

    public void OpenUpgradeCar()
    {
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        Car car = slot.Car;

        if (car.GetUpgradeLevel() < car.GetMaxUpgradeLevel())
        {
            // UI
            carUI.SetActive(false);
            slot.SetDetailCarUI(false);
            slot.SetProgressBarVisible(false);

            panel.SetActive(true);
            upgradeCarButtons.SetActive(true);

            // REST
            GetComponent<UpgradeCar>().PrepareUpgrade();
        }
        else
            print("alredy max upgrade");
    }

    public void ConfirmUpgrade()
    {
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        // UI
        upgradeCarButtons.SetActive(false);
        panel.SetActive(false);

        slot.SetDetailCarUI(true);
        slot.SetProgressBarVisible(true);
        carUI.SetActive(true);
    }

    public void ConfirmSell()
    {
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        // UI
        sellButtons.SetActive(false);
        panel.SetActive(false);

        slot.SetDetailCarUI(true);
        slot.SetProgressBarVisible(true);
        carUI.SetActive(true);
    }

    public void ClosePanel()
    {
        if (sellButtons.activeSelf)
        {
            BackFromSellCar();
            CheckMaxUpgradeAndAvailability();
        }
        else if (upgradeCarButtons.activeSelf)
        {
            BackFromUpgradeCar();
            CheckMaxUpgradeAndAvailability();
        }

        else if (buyCarButtons.activeSelf)
            BackFromBuyCar();

        else
            print("panel error");
    }

    public void Home()
    {
        CancelFocus();
    }

    public void BackFromUpgradeCar()
    {
        if (!UpgradeCar.Instance.locked)
        {
            CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();

            // UI
            upgradeCarButtons.SetActive(false);
            panel.SetActive(false);

            slot.SetDetailCarUI(true);
            slot.SetProgressBarVisible(true);
            slot.UpdateUILevel(false);
            carUI.SetActive(true);
        }
        else
            print("locked");
    }

    private void BackFromBuyCar()
    {
        if (PlayerProgression.PlayerData.BestAchivedCarLevel == 0)
        {
            tutorialHand2.SetActive(false);
            tutorialHand1.SetActive(true);
        }
            

        // UI
        buyCarPlatform.SetActive(false);
        buyCarButtons.SetActive(false);
        panel.SetActive(false);

        buyCarButton.gameObject.SetActive(true);
    }

    private void BackFromSellCar()
    {
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();
        // UI
        sellButtons.SetActive(false);
        panel.SetActive(false);

        slot.SetDetailCarUI(true);
        slot.SetProgressBarVisible(true);
        carUI.SetActive(true);

    }

    #endregion





    private void Focus(CarSlot slot)
    {
        HapticManager.DoHaptic();
        CameraFocusController.Instance.Focus = slot.Transform;

        if (tutorialHand0.activeInHierarchy)
        {
            tutorialHand0.SetActive(false);
        }

        // ENVIRONMENT
        SetLayerRecursively(slot.Transform, 6);
        slot.SetMeshVisible(false);
        environmentRoad.DOFade(1, 0.5f);
        EnvironmentRoadController.Instance.RandomBiome();

        // UI
        buyCarButton.gameObject.SetActive(false);

        carUI.SetActive(true);
        slot.SetDetailCarUI(true);
        CheckMaxUpgradeAndAvailability();

        slot.RepositionLevel(true);
    }

    private void CancelFocus()
    {
        HapticManager.DoHaptic();
        CarSlot slot = CameraFocusController.Instance.Focus.GetComponent<CarSlot>();

        // UI
        slot.SetDetailCarUI(false);
        carUI.SetActive(false);

        buyCarButton.gameObject.SetActive(true);

        // ENVIRONMENT
        SetLayerRecursively(slot.Transform, 0);
        slot.SetMeshVisible(true);
        environmentRoad.DOFade(0, 0.5f);

        slot.RepositionLevel(false);

        CameraFocusController.Instance.Focus = null;
    }

    public void SetLayerRecursively(Transform obj, int newLayer)
    {
        obj.gameObject.layer = newLayer;
        for (int i = 0; i < obj.childCount; i++)
        {
            SetLayerRecursively(obj.GetChild(i), newLayer);
        }
    }

    
}

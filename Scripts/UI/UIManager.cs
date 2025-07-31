using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Bar")]
    public Image healthBar;
    public Image satietyBar;
    public Image hydrationBar;
    public GameObject scanBar;
    public Image scanGaugeBar;
    public GameObject jetpackBar;
    public Image jetpackGaugeBar;
    public Image hotGaugeBar;
    public Image breathingGaugeBar;
    public GameObject invisibilityBar;
    public Image invisibilityGaugeBar;

    [Header("Text")]
    public Text healthText;
    public Text satietyText;
    public Text hydrationText;
    public Text jetpackRateText;

    [Header("Slot")]
    public GameObject inventorySlotPrefab;
    public GameObject benchSlotPrefab;
    public GameObject quickSlotPrefab;
    public GameObject chargeSlotPrefab;

    [Header("UI Panels")]
    public GameObject inventoryPanel;   
    public GameObject inventorySlotPanel;
    public GameObject benchPanel;
    public GameObject benchSlotPanel;
    public GameObject quickSlotPanel;
    public GameObject chargeSlotPanel;
    public GameObject equipmentSlotPanel;
    public GameObject moduleSlotPanel;
    public GameObject menuPanel;

    [Header("Player")]
    public GameObject player;

    [Header("Game")]
    public GameObject GameManager;

    [Header("Scripts")]
    [SerializeField] public Inventory inventory;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UseItem useItem;
    [SerializeField] private PlayerItemHandler playerItemHandler;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerController playerController;


    private ScrollRect benchScrollRect;
    private KeyCode inventoryKey = KeyCode.E;
    private bool isInventoryOpen = false;
    public bool isBenchOpen = false;
    private EquipmentSlot[] equipmentSlots;
    [HideInInspector] public GameObject currentBench;
    private ItemInstance currentArmor;

    void Start()
    {
        if (benchPanel != null)
        {
            benchScrollRect = benchPanel.GetComponentInChildren<ScrollRect>();
        }
        if (player != null)
        {
            UpdateUI(playerState.stats);
            playerState.OnStatsChanged += UpdateUI;
        }
        if (inventoryPanel != null && inventorySlotPanel != null)
        {
            UpdateItemUI();
        }
        if (equipmentSlotPanel != null)
        {
            equipmentSlots = equipmentSlotPanel.GetComponentsInChildren<EquipmentSlot>();
            RefreshEquipmentItems(playerState.equipmentItems);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(inventoryKey) && !playerController.isPaused)
        {
            ToggleInventory();
        }
        if (playerState.isInvisible)
        {
            UpdateInvisibilityGaugeBar();
        }
    }

    public void OnInventoryOneClick(int slotIndex)
    {
        if (!currentArmor) return;
        ItemInstance item = inventory.items[slotIndex];
        if (!item)
        {
            currentArmor = null;
            moduleSlotPanel.SetActive(false);
            return;
        }
        if (item.itemData.itemType != ItemType.ArmorMoudle)
        {
            currentArmor = null;
            moduleSlotPanel.SetActive(false);
        }
    }
    public void OnInventorySlotDoubleClick(int slotIndex)
    {
        if (!inventory.items[slotIndex]) return;

        EquipmentType type = inventory.items[slotIndex].itemData.equipmentType;

        
        // ¿Â¬¯ æ∆¿Ã≈€
        if (type != EquipmentType.None)
        {
            playerState.AddEquipmentItem(inventory.items[slotIndex]);
            UpdateEquipmentUI(type, inventory.items[slotIndex]);
            inventory.RemoveItem(slotIndex);
            UpdateItemUI();
            return;
        }

        // π∞, Ωƒ∑Æ, »˙∆— æ∆¿Ã≈€
        if (inventory.items[slotIndex].itemData.itemType == ItemType.BioState && inventory.UseItem(slotIndex))
        {
            Destroy(inventory.items[slotIndex].gameObject);
            inventory.RemoveItem(slotIndex);
            UpdateItemUI();
            return;
        }

        if (inventory.items[slotIndex].itemData.itemType == ItemType.ArmorMoudle && moduleSlotPanel && currentArmor)
        {
            currentArmor.GetComponent<ArmorModuleManager>().AddModule(inventory, inventory.items[slotIndex], currentArmor);
            return;
        }

    }

    public void UpdateEquipmentUI(EquipmentType type, ItemInstance item)
    {
        switch(type)
        {
            case EquipmentType.Face:
                equipmentSlots[0].SetEquipmentSlot(item);
                break;
            case EquipmentType.Body:
                equipmentSlots[1].SetEquipmentSlot(item);
                break;
            case EquipmentType.Back:
                equipmentSlots[2].SetEquipmentSlot(item);
                playerItemHandler.SetBackSocket(item);
                break;
            default:
                break;
        }
    }
    private void ToggleInventory()
    {
        if (!inventoryPanel || isBenchOpen || useItem.chargeUIOpen) return;
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        moduleSlotPanel.SetActive(false);
        currentArmor = null;
        SetMouseSate();
    }

    public void ToggleBench(GameObject bench)
    {
        if (!benchPanel || useItem.chargeUIOpen) return;
        isBenchOpen = !isBenchOpen;
        if (bench) currentBench = bench;

        benchPanel.SetActive(isBenchOpen);

        if (isBenchOpen)
        {
            InitializeBenchUI();
        }
        if (isBenchOpen && benchScrollRect != null)
        {
            benchScrollRect.verticalNormalizedPosition = 1f;
        }
        SetMouseSate();
    }

    private void SetMouseSate()
    {
        if (isInventoryOpen || isBenchOpen || playerController.isPaused)
        {
            GameState.IsUIOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            GameState.IsUIOpen = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void UpdateUI(PlayerStats stats)
    {
        if (healthBar && satietyBar && hydrationBar)
        {
            healthBar.fillAmount = stats.health / GameConstants.MAX_HEALTH;
            satietyBar.fillAmount = stats.satiety / GameConstants.MAX_SATIETY;
            hydrationBar.fillAmount = stats.hydration / GameConstants.MAX_HYDRATION;
            hotGaugeBar.fillAmount = stats.hotGauge / GameConstants.MAX_HOT_GAUGE;
            breathingGaugeBar.fillAmount = stats.breathingGauge / GameConstants.MAX_BREATHING_GAUGE;
        }

        if (healthText && satietyText && hydrationText)
        {
            healthText.text = stats.health.ToString("F0");  
            satietyText.text = stats.satiety.ToString("F0");
            hydrationText.text = stats.hydration.ToString("F0");
        }
    }

    public void UpdateScanUI(float time)
    {
        scanBar.SetActive(true);
        if (scanBar && scanGaugeBar)
        {
            scanGaugeBar.fillAmount = time / 2f;
        }
    }
    public void UpdateItemUI()
    {
        if (!inventory || !inventorySlotPanel) return;
        for (int i = 0; i < inventory.items.Length; i++)
        {
            Transform transform = inventorySlotPanel.transform.GetChild(i);

            Image itemImage = transform.Find("ItemIcon").GetComponent<Image>();

            if (inventory.items[i] == null)
            {
                itemImage.sprite = null;
                itemImage.color = new Color(0, 0, 0, 0);
            }
            else
            {
                itemImage.sprite = inventory.items[i].itemData.icon;
                itemImage.color = Color.white;
            }
        }
    }
    public void UpdateJetpackUI(ItemInstance jetpack, float time, float foolTime)
    {
        if (!jetpackBar || !jetpackGaugeBar || !jetpackRateText || !jetpack) return;
        if (!playerItemHandler.backSocketItem) return;
        if (playerItemHandler.backSocketItem.itemData.itemName != "¡¶∆Æ∆—") return;

        jetpackBar.SetActive(true);
        jetpackGaugeBar.fillAmount = time / foolTime;
        
        ItemInstance fuel = jetpack.Get<ItemInstance>("Fuel");
        int displayRate = 0;

        if (fuel)
        {
            displayRate = Mathf.Max(0, (int)fuel.Get<float>("fuelUsageRate"));
            jetpackRateText.text = displayRate.ToString() + "%";
        }
        else
        {
            jetpackRateText.text = "None";
        }
    }

    public void OnBenchSlotClick(int slotIndex)
    {
        if (!currentBench) return;
        bool isCan = currentBench.GetComponent<Bench>().CanCraft(playerState, gameManager.allItems[slotIndex]);
        if (isCan)
        {
            currentBench.GetComponent<Bench>().CraftItem(player, playerState, gameManager.allItems[slotIndex]);
            UpdateItemUI();
        }
    }

    public void InitializeBenchUI()
    {
        if (!benchPanel || !benchSlotPanel) return;

        // ±‚¡∏ ¡¶¿€¥Î ΩΩ∑‘ √ ±‚»≠
        for (int i = benchSlotPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(benchSlotPanel.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < playerState.unlockedItems.Length; i++)
        {
            if (playerState.unlockedItems[i])
            {
                GameObject slot = Instantiate(benchSlotPrefab, benchSlotPanel.transform);
                BenchSlot slotData = slot.GetComponent<BenchSlot>();

                Image itemImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
                itemImage.sprite = gameManager.allItems[i].icon;
                itemImage.color = Color.white;

                slotData.SetSlotIndex(i);
            }
        }
    }

    public void ClearTooltip(GameObject Tooltip)
    {
        for (int i = Tooltip.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(Tooltip.transform.GetChild(i).gameObject);
        }
    }

    public void ShowChargeSlots(ItemInstance[] charges, ItemInstance currentCharge)
    {
        //chargeSlotPanel.SetActive(true);
        for (int i = 0; i < charges.Length; i++)
        {
            GameObject slot = Instantiate(chargeSlotPrefab, chargeSlotPanel.transform);
            Image itemImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI chargeRate = slot.transform.Find("chargeRate").GetComponent<TextMeshProUGUI>();

            if (!charges[i])
            {
                itemImage.color = Color.clear;
                chargeRate.text = "None";
                continue;
            };
            itemImage.sprite = charges[i].itemData.icon;
            
            int displayCharge = 0;
            if (charges[i].itemData.productType == ProductType.Battery)
            {
                displayCharge = Mathf.Max(0, (int)charges[i].Get<float>("batteryUsageRate"));
            }
            else if (charges[i].itemData.productType == ProductType.Fuel)
            {
                displayCharge = Mathf.Max(0, (int)charges[i].Get<float>("fuelUsageRate"));
            }
            chargeRate.text = displayCharge.ToString() + "%";
            if (currentCharge == charges[i])
            {
                itemImage.color = Color.white;
                continue;
            }

            itemImage.color = new Color (1, 1, 1, 0.5f);
        }
    }

    public void SetChargeText()
    {
        ItemInstance item = playerItemHandler.currentItem.GetComponent<ItemInstance>();
        ItemInstance charge = null;
        switch (item.itemData.productType)
        {
            case ProductType.BatteryUsing:
                charge = item.Get<ItemInstance>("Battery");
                break;
            case ProductType.FuelUsing:
                charge = item.Get<ItemInstance>("Fuel");
                break;
            default:
                return;
        }

        int displayCharge = Mathf.Max(0, (int)charge.Get<float>("batteryUsageRate"));

        if (!chargeSlotPanel) return;
        if (!chargeSlotPanel.GetComponentInChildren<TextMeshProUGUI>()) return;
        chargeSlotPanel.GetComponentInChildren<TextMeshProUGUI>().text = displayCharge.ToString() + "%";
    }
    public void ClearChargeSlots()
    {
        for (int i = chargeSlotPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(chargeSlotPanel.transform.GetChild(i).gameObject);
        }
    }

    public void SwitchChargeSlot(int index)
    {
        for (int i = 0; i < chargeSlotPanel.transform.childCount; i++)
        {
            GameObject slot = chargeSlotPanel.transform.GetChild(i).gameObject;
            Image itemImage = slot.transform.Find("ItemIcon").GetComponent<Image>();

            if (index == i)
            {
                itemImage.color = Color.white;
            }
            else
            {
                itemImage.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }

    public void OnInventorySlotRightClick(int slotIndex)
    {
        if (!inventory.items[slotIndex]) return;

        if (currentArmor == inventory.items[slotIndex])
        {
            currentArmor = null;
            moduleSlotPanel.SetActive(false);
            return;
        }

        currentArmor = inventory.items[slotIndex];

        ItemType type = currentArmor.itemData.itemType;
        if (type != ItemType.Armor) return;

        moduleSlotPanel.SetActive(true);

        ModuleSlot[] moduleSlots = moduleSlotPanel.GetComponentsInChildren<ModuleSlot>();
        ItemInstance[] armorModules = currentArmor.Get<ItemInstance[]>("armorModules");

        if (moduleSlots.Length <= 0 || armorModules.Length <= 0) return;
        for (int i = 0; i < moduleSlots.Length; i++)
        {
            moduleSlots[i].SetModuleSlots(armorModules[i], currentArmor);
        }
    }
    public void UpdateInvisibilityGaugeBar()
    {
        if (!playerController.currentSkill)
        {
            return;
        }
        invisibilityGaugeBar.fillAmount = 1 - playerController.currentSkill.skillDurationTimer / playerController.currentSkill.skillDuration;
    }

    public void SetEscMenu()
    {
        if (!menuPanel) return;
        menuPanel.SetActive(!menuPanel.activeSelf);
        if (menuPanel.activeSelf)
        {
            Time.timeScale = 0f;
            playerController.isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            playerController.isPaused = false;
        }
        SetMouseSate();
    }

    public void ClickContinueButton()
    {
        if (!menuPanel) return;
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        playerController.isPaused = false;
        SetMouseSate();
    }

    public void RefreshQuickSlotItems(int[] quickIndexs)
    {
        QuickPanel quickPanel = quickSlotPanel.GetComponent<QuickPanel>();
        int invenIndex = -1;    

        for (int i = 0; i < quickIndexs.Length; i++)
        {
            invenIndex = quickIndexs[i];
            if (invenIndex < 0 || !playerState.inventoryItems[invenIndex]) continue;

            quickPanel.AddQuickSlotItem(i, invenIndex, playerState.inventoryItems[invenIndex]);
        }
        quickPanel.HoldingItem();
    }

    public void RefreshEquipmentItems(ItemInstance[] items)
    {
        foreach (ItemInstance item in items)
        {
            if (item)
            {
                UpdateEquipmentUI(item.itemData.equipmentType, item);
            }
        }
    }
}
 
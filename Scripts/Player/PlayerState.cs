using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public struct PlayerStats
{
    public float health;
    public float satiety;
    public float hydration;
    public float hotGauge;
    public float breathingGauge;

    public PlayerStats(float maxHp, float maxSatiety, float maxHydration, float minHot, float maxBreathing)
    {
        this.health = maxHp;
        this.satiety = maxSatiety;
        this.hydration = maxHydration;
        this.hotGauge = minHot;
        this.breathingGauge = maxBreathing;
    }
}

public static class GameConstants
{
    public const int MAX_INVENTORY_SIZE = 21;
    public const float MAX_HEALTH = 100f;
    public const float MAX_SATIETY = 100f;
    public const float MAX_HYDRATION = 100f;
    public const float MAX_HOT_GAUGE = 100f;
    public const float MAX_BREATHING_GAUGE = 100f;
}

public class PlayerState : MonoBehaviour
{
    public PlayerStats stats;

    public bool[] unlockedItems;
    public ItemInstance[] inventoryItems;
    public ItemInstance[] equipmentItems;
    public int[] quickSlotIndexs;

    private bool isGameOver = false; // ���� ���� ����

    private float satietyTime = 8f;
    private float hydrationTime = 5f;
    private float hotGaugeTime = 0.5f;
    private float breathingGaugeTime = 0.5f;

    private Inventory inventory;
    private UIManager uiManager;
    private PlayerController playerController;
    [HideInInspector] public PlayerItemHandler playerItemHandler;
    [HideInInspector] public bool isInHotZone;

    private bool isAutoHeal;
    private float autoHealAmount = 0f;
    private float autoHealTime = 5f;

    [HideInInspector] public bool isInvisible = false;

    public event System.Action<PlayerStats> OnStatsChanged;

    void Start()
    {
        stats = new PlayerStats(GameConstants.MAX_HEALTH, GameConstants.MAX_SATIETY, GameConstants.MAX_HYDRATION, 0f, GameConstants.MAX_BREATHING_GAUGE);  // stats �ʱ�ȭ
        inventory = GetComponent<Inventory>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        InvokeRepeating("DecreaseSatietyByTime", satietyTime, satietyTime); // ������ ���� �Լ� ȣ��
        InvokeRepeating("DecreaseHydrationByTime", hydrationTime, hydrationTime); // ���� ���� �Լ� ȣ��
        playerController = GetComponent<PlayerController>();
        playerItemHandler = GetComponent<PlayerItemHandler>();
    }

    private void Update()
    {
        if (isAutoHeal)
        {
            autoHealTime -= Time.deltaTime;
            if (autoHealTime < 0f)
            {
                autoHealTime = 5f;
                ModifyHealth(autoHealAmount);
            }
        }
    }
    public void EnterInHotZone()
    {
        CancelInvoke("DecreaseHotGaugeByTime");
        float resultTime = hotGaugeTime;
        if (equipmentItems[1] != null)
        {
            ItemInstance resistanceModule = equipmentItems[1].Get<ItemInstance[]>("armorModules")[1];
            if (resistanceModule)
            {
                ModuleEffect moduleEffect = resistanceModule.GetComponent<ModuleEffect>();
                if (moduleEffect && moduleEffect.environmentType == EnvironmentType.Desert)
                {
                    resultTime = moduleEffect.ApplyResistanceEffect(resultTime);
                }
            }
        }
        isInHotZone = true;
        InvokeRepeating("IncreaseHotGaugeByTime", resultTime, resultTime); 
    }

    public void ExitFromHotZone()
    {
        CancelInvoke("IncreaseHotGaugeByTime");
        CancelInvoke("DecreaseHealthByHotGauge");
        InvokeRepeating("DecreaseHotGaugeByTime", hotGaugeTime + 1f, hotGaugeTime + 1f); // �µ� ������ ���� �Լ� ȣ��
        isInHotZone = false;
    }

    public void EnterInWaterZone()
    {
        CancelInvoke("IncreaseBreathingGaugeByTime");
        InvokeRepeating("DecreaseBreathingGaugeByTime", breathingGaugeTime, breathingGaugeTime); // ȣ�� ������ ���� �Լ� ȣ��
    }

    public void ExitFromWaterZone()
    {
        CancelInvoke("DecreaseBreathingGaugeByTime");
        CancelInvoke("DecreaseHealthByBreathingGauge");
        InvokeRepeating("IncreaseBreathingGaugeByTime", breathingGaugeTime + 1f, breathingGaugeTime + 1f); // ȣ�� ������ ���� �Լ� ȣ��
    }

    public void UpdateInventory(ItemInstance[] newInventoryItems)
    {
        inventoryItems = newInventoryItems;
    }    
    public void AddUnlockedItems(ItemData itemData)
    {
        unlockedItems[itemData.itemID] = true; 
    }

    public void AddEquipmentItem(ItemInstance item)
    {
        switch (item.itemData.equipmentType)
        {
            case EquipmentType.Face:
                SetEquipmentItems(0, item);
                break;
            case EquipmentType.Body:
                ResetModuleEffect();
                SetEquipmentItems(1, item);
                break;
            case EquipmentType.Back:
                SetEquipmentItems(2, item);
                break;
            default:
                break;
        }
    }

    private void SetEquipmentItems(int index, ItemInstance item)
    {
        if (equipmentItems[index])
        {
            inventory.AddItem(equipmentItems[index]);
            uiManager.UpdateItemUI();
        }
        equipmentItems[index] = item;

        SetSkillModule(item);
    }

    private void SetSkillModule(ItemInstance item)
    {
        ItemInstance skillModule = item.Get<ItemInstance[]>("armorModules")[2];
        if (!skillModule) return;
        skillModule.GetComponent<ArmorSkill>().ResetSkill();
        isAutoHeal = SetIsAutoHeal(); 
    }

    // ������ ���� �Լ�
    private void DecreaseSatietyByTime()
    {
        ModifySatiety(-1f);
        if (stats.satiety <= 0f)
        {
            InvokeRepeating("DecreaseHealthBySatiety", 1f, 1f);
            CancelInvoke("DecreaseSatietyByTime");
        }
    }

    // ���� ���� �Լ�
    private void DecreaseHydrationByTime()
    {
        ModifyHydration(-1f);
        if (stats.hydration <= 0f)
        {
            InvokeRepeating("DecreaseHealthByHydration", 1f, 1f);
            CancelInvoke("DecreaseHydrationByTime");
            return;
        }
    }

    // �µ� ������ ���� �Լ�
    private void IncreaseHotGaugeByTime()
    {
        ModifyHotGauge(1f);
        if (stats.hotGauge >= GameConstants.MAX_HOT_GAUGE)
        {
            InvokeRepeating("DecreaseHealthByHotGauge", 1f, 1f);
            CancelInvoke("IncreaseHotGaugeByTime");
        }
    }

    // �µ� ������ ���� �Լ�
    private void DecreaseHotGaugeByTime()
    {
        ModifyHotGauge(-1f);
        if (stats.hotGauge <= 0f)
        {
            CancelInvoke("DecreaseHealthByHotGauge");
            CancelInvoke("DecreaseHotGaugeByTime");
        }
    }

    // ȣ�� ������ ���� �Լ�
    private void IncreaseBreathingGaugeByTime()
    {
        ModifyBreathingGauge(1f);
        if (stats.breathingGauge >= GameConstants.MAX_BREATHING_GAUGE)
        {
            CancelInvoke("DecreaseBreathingGaugeByTime");
            CancelInvoke("DecreaseHealthByBreathingGauge");
        }
    }

    // ȣ�� ������ ���� �Լ�
    private void DecreaseBreathingGaugeByTime()
    {
        ModifyBreathingGauge(-1f);
        if (stats.breathingGauge <= 0f)
        {
            InvokeRepeating("DecreaseHealthByBreathingGauge", 1f, 1f);
            CancelInvoke("DecreaseBreathingGaugeByTime");
        }
    }

    // ������ ���ҷ� ���� ü�� ���� �Լ�
    private void DecreaseHealthBySatiety()
    {
        ModifyHealth(-1f);
        if (stats.satiety > 0f)
        {
            InvokeRepeating("DecreaseSatietyByTime", satietyTime, satietyTime);
            CancelInvoke("DecreaseHealthBySatiety");   
        }
    }

    // ���� ���ҷ� ���� ü�� ���� �Լ�
    private void DecreaseHealthByHydration()
    {
        CancelInvoke("DecreaseHydrationByTime");
        ModifyHealth(-1f);
        if (stats.hydration > 0f)
        {
            InvokeRepeating("DecreaseHydrationByTime", hydrationTime, hydrationTime);
            CancelInvoke("DecreaseHealthByHydration");
        }
    }

    // �µ� ������ ������ ���� ü�� ���� �Լ�
    private void DecreaseHealthByHotGauge()
    {
        CancelInvoke("DecreaseHotGaugeByTime");
        ModifyHealth(-1f);
        if (stats.hotGauge < GameConstants.MAX_HOT_GAUGE)
        {
            InvokeRepeating("DecreaseHotGaugeByTime", hotGaugeTime, hotGaugeTime);
            CancelInvoke("DecreaseHealthByHotGauge");
        }
    }

    // ȣ�� ������ ���ҷ� ���� ü�� ���� �Լ�
    private void DecreaseHealthByBreathingGauge()
    {
        CancelInvoke("DecreaseBreathingGaugeByTime");
        ModifyHealth(-1f);
        if (stats.breathingGauge > 0f)
        {
            InvokeRepeating("DecreaseBreathingGaugeByTime", breathingGaugeTime, breathingGaugeTime);
            CancelInvoke("DecreaseHealthByBreathingGauge");
        }
    }

    // ü�� ���� �Լ�
    public void ModifyHealth(float amount)
    {
        if (isGameOver) return; 

        if (amount < 0f && equipmentItems[1] != null)
        {
            ItemInstance defenseModule = equipmentItems[1].Get<ItemInstance[]>("armorModules")[0];
            if (defenseModule)
            {
                ModuleEffect moduleEffect = defenseModule.GetComponent<ModuleEffect>();
                if (moduleEffect)
                {
                    amount = moduleEffect.ApplyDefenseEffect(amount);
                }
            }
        }
        
        stats.health += amount;
        isAutoHeal = SetIsAutoHeal();

        // ü���� �ִ�ġ�� ���� �ʵ��� ����
        if (stats.health > GameConstants.MAX_HEALTH)
        {
            stats.health = GameConstants.MAX_HEALTH;
        }
        else if (stats.health < 0f)
        {
            stats.health = 0f;
        }

        OnStatsChanged?.Invoke(stats);

        if (stats.health <= 0f)
        {
            isGameOver = true;
            CancelInvoke();
            // ���� ó�� �Լ�
        }

    }

    // ������ ���� �Լ�
    public void ModifySatiety(float amount)
    {
        stats.satiety += amount;

        // �������� �ִ�ġ�� ���� �ʵ��� ����
        if (stats.satiety > GameConstants.MAX_SATIETY)
        {
            stats.satiety = GameConstants.MAX_SATIETY;
        }
        else if (stats.satiety < 0f)
        {
            stats.satiety = 0f;
        }

        OnStatsChanged?.Invoke(stats);
    }

    // ���� ���� �Լ�
    public void ModifyHydration(float amount)
    {
        stats.hydration += amount;

        // ������ �ִ�ġ�� ���� �ʵ��� ����
        if (stats.hydration > GameConstants.MAX_HYDRATION)
        {
            stats.hydration = GameConstants.MAX_HYDRATION;
        }
        else if (stats.hydration < 0f)
        {
            stats.hydration = 0f;
        }

        OnStatsChanged?.Invoke(stats);
    }

    // �µ� ������ ���� �Լ�
    public void ModifyHotGauge(float amount)
    {
        stats.hotGauge += amount;

        if (stats.hotGauge > GameConstants.MAX_HOT_GAUGE)
        {
            stats.hotGauge = GameConstants.MAX_HOT_GAUGE;
        }
        else if (stats.hotGauge < 0f)
        {
            stats.hotGauge = 0f;
        }

        OnStatsChanged?.Invoke(stats);
    }

    // ȣ�� ������ ���� �Լ�
    public void ModifyBreathingGauge(float amount)
    {
        stats.breathingGauge += amount;

        if (stats.breathingGauge > GameConstants.MAX_BREATHING_GAUGE)
        {
            stats.breathingGauge = GameConstants.MAX_BREATHING_GAUGE;
        }

        else if (stats.breathingGauge < 0f)
        {
            stats.breathingGauge = 0f;
        }

        OnStatsChanged?.Invoke(stats);
    }

    public void ResetModuleEffect()
    {
        if (isInHotZone)
        {
            ExitFromHotZone();
            EnterInHotZone();
        }
        playerController.SetMoveSpeed(5f); 
        if (playerItemHandler.currentItem)
        {
            if (playerItemHandler.currentItem.itemData.itemType == ItemType.Weapon)
            {
                playerItemHandler.ResetWeapon();
            }
        }
        isAutoHeal = false;
        autoHealTime = 5f;
}

    private bool SetIsAutoHeal()
    {
        if (!equipmentItems[1]) return false;

        ItemInstance skillModule = equipmentItems[1].Get<ItemInstance[]>("armorModules")[2];
        if (!skillModule) return false;

        ArmorSkill armorSkill = skillModule.GetComponent<ArmorSkill>();
        if (ArmorSkillType.AutoHeal != armorSkill.skillType) return false;

        autoHealAmount = armorSkill.healAmount;
        return stats.health + autoHealAmount < GameConstants.MAX_HEALTH * armorSkill.healthRatio;
    }

    public void RefreshUI()
    {
        OnStatsChanged?.Invoke(stats);
        
    }
}

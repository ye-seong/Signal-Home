using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public struct PlayerStats
{
    public float health;
    public float satiety;
    public float hydration;

    public PlayerStats(float maxHp, float maxSatiety, float maxHydration)
    {
        this.health = maxHp;
        this.satiety = maxSatiety;
        this.hydration = maxHydration;
    }
}

public static class GameConstants
{
    public const float MAX_HEALTH = 100f;
    public const float MAX_SATIETY = 100f;
    public const float MAX_HYDRATION = 100f;
}

public class PlayerState : MonoBehaviour
{
    public PlayerStats stats;

    public bool[] unlockedItems;
    public ItemInstance[] inventoryItems;

    private bool isGameOver = false; // ���� ���� ����

    private float satietyTime = 8f;
    private float hydrationTime = 5f;

    public event System.Action<PlayerStats> OnStatsChanged;

    void Start()
    {
        stats = new PlayerStats(GameConstants.MAX_HEALTH, GameConstants.MAX_SATIETY, GameConstants.MAX_HYDRATION);  // stats �ʱ�ȭ

        InvokeRepeating("DecreaseSatietyByTime", satietyTime, satietyTime); // ������ ���� �Լ� ȣ��
        InvokeRepeating("DecreaseHydrationByTime", hydrationTime, hydrationTime); // ���� ���� �Լ� ȣ��
    }

    void Update()
    {

    }

    public void UpdateInventory(ItemInstance[] newInventoryItems)
    {
        inventoryItems = newInventoryItems;
    }    
    public void AddUnlockedItems(ItemData itemData)
    {
        unlockedItems[itemData.itemID] = true; 
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

    // ü�� ���� �Լ�
    public void ModifyHealth(float amount)
    {
        if (isGameOver) return; 

        stats.health += amount;

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
}

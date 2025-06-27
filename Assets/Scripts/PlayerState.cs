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

    private bool isGameOver = false; // 게임 오버 여부

    private float satietyTime = 8f;
    private float hydrationTime = 5f;

    public event System.Action<PlayerStats> OnStatsChanged;

    void Start()
    {
        stats = new PlayerStats(GameConstants.MAX_HEALTH, GameConstants.MAX_SATIETY, GameConstants.MAX_HYDRATION);  // stats 초기화

        InvokeRepeating("DecreaseSatietyByTime", satietyTime, satietyTime); // 포만감 감소 함수 호출
        InvokeRepeating("DecreaseHydrationByTime", hydrationTime, hydrationTime); // 수분 감소 함수 호출
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

    // 포만감 감소 함수
    private void DecreaseSatietyByTime()
    {
        ModifySatiety(-1f);
        if (stats.satiety <= 0f)
        {
            InvokeRepeating("DecreaseHealthBySatiety", 1f, 1f);
            CancelInvoke("DecreaseSatietyByTime");
        }
    }

    // 수분 감소 함수
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

    // 포만감 감소로 인한 체력 감소 함수
    private void DecreaseHealthBySatiety()
    {
        ModifyHealth(-1f);
        if (stats.satiety > 0f)
        {
            InvokeRepeating("DecreaseSatietyByTime", satietyTime, satietyTime);
            CancelInvoke("DecreaseHealthBySatiety");   
        }
    }

    // 수분 감소로 인한 체력 감소 함수
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

    // 체력 수정 함수
    public void ModifyHealth(float amount)
    {
        if (isGameOver) return; 

        stats.health += amount;

        // 체력이 최대치를 넘지 않도록 제한
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
            // 죽음 처리 함수
        }

    }

    // 포만감 수정 함수
    public void ModifySatiety(float amount)
    {
        stats.satiety += amount;

        // 포만감이 최대치를 넘지 않도록 제한
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

    // 수분 수정 함수
    public void ModifyHydration(float amount)
    {
        stats.hydration += amount;

        // 수분이 최대치를 넘지 않도록 제한
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

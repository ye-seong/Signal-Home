using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Information")]
    public int itemID = -1;
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject itemPrefab;
    public ItemType itemType;

    [Header("ingredient list")]
    public ItemData[] ingredients;

    [ShowIf("itemType", ItemType.BioState)]
    [Header("BioState Properties")]
    public BioStateType bioStateType;
    [ShowIf("itemType", ItemType.BioState)]
    public float amount;

    [ShowIf("itemType", ItemType.Product)]
    [Header("Product Properties")]
    public ProductType productType;

    [ShowIf("itemType", ItemType.Weapon)]
    [Header("Weapon Properties")]
    public float attackPower;
    [ShowIf("itemType", ItemType.Weapon)]
    public float attackSpeed;

    [ShowIf("itemType", ItemType.Armor)]
    [Header("Armor Properties")]
    public float defensePower;
    [ShowIf("itemType", ItemType.Armor)]
    public float maxDurability;

    [ShowIf("productType", ProductType.Battery)]
    [Header("Battery Properties")]
    public float maxBatteryCharge;

    [ShowIf("productType", ProductType.Fuel)]
    [Header("Fuel Properties")]
    public float maxFuelAmount;



    public bool UseBioTypeItem(PlayerState playerState)
    {
        if (itemType != ItemType.BioState || !playerState) return false;
        
        switch (bioStateType)
        {
            case BioStateType.Healing:
                if (!UseHealItem(playerState)) return false;
                break;
            case BioStateType.Satiety:
                if (!UseSatietyItem(playerState)) return false;
                break;
            case BioStateType.Hydration:
                if (!UseHydrationItem(playerState)) return false;
                break;
            default:
                return false;
        }
        return true;
    }

    public bool UseHealItem(PlayerState playerState)
    {
        if (playerState.stats.health >= GameConstants.MAX_HEALTH && amount >= 0)
        {
            return false;
        }
        playerState.ModifyHealth(amount);
        return true;
    }

    public bool UseSatietyItem(PlayerState playerState)
    {
        if (playerState.stats.satiety >= GameConstants.MAX_SATIETY && amount >= 0)
        {
            return false;
        }
        playerState.ModifySatiety(amount);
        return true;
    }

    public bool UseHydrationItem(PlayerState playerState)
    {
        if (playerState.stats.hydration >= GameConstants.MAX_HYDRATION && amount >= 0)
        {
            return false;
        }
        playerState.ModifyHydration(amount);
        return true;
    }
}

public enum ItemType
{
    BioState,   // 구급키트, 식량, 물
    Resources, // 재료가 없는 순수 자원
    Product,  // 재료가 필요한 아이템
    Weapon,     // 무기
    Armor       // 수트
}

public enum BioStateType
{
    Healing,
    Satiety,
    Hydration
}   

public enum ProductType
{
    None,
    Battery,
    Fuel,
    BatteryUsing,
    FuelUsing
}


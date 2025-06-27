using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemInstance : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector] public Dictionary<string, object> properties = new Dictionary<string, object>();

    private bool isInitialized = false;

    private Outline outline;

    private void Start()
    {
        if (!isInitialized)
        {
            IntializeProperties();
        }
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            outline = GetComponent<Outline>();
        }
        if (!outline) return;
        outline.enabled = false;
    }
    public static ItemInstance Create(ItemData data)
    {
        GameObject obj = Instantiate(data.itemPrefab);
        ItemInstance instance = obj.AddComponent<ItemInstance>();
        instance.itemData = data;
        return instance;
    }

    void IntializeProperties()
    {
        switch(itemData.productType)
        {
            case ProductType.Battery:
                properties["batteryUsageRate"] = itemData.maxBatteryCharge;
                break;
            case ProductType.Fuel:
                properties["fuelUsageRate"] = itemData.maxFuelAmount;
                break;
            case ProductType.BatteryUsing:
                properties["Battery"] = null;
                break;
            case ProductType.FuelUsing:
                properties["Fuel"] = null;
                break;
            default:
                break;
        }

        switch(itemData.itemType)
        {
            case ItemType.Armor:
                properties["durability"] = 100f;
                break;
            default:
                break;
        }
    }

    public T Get<T>(string Key) where T : class
    {
        if (properties.TryGetValue(Key, out object value))
        {
            return value as T;
        }
        return null;
    }

    public void Set<T>(string Key, T Value) where T : class
    {
        if (properties.TryGetValue(Key, out object value))
        {
            properties[Key] = Value;
        }
        return;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public void OnPickup()
    {
        Destroy(gameObject); 
    }

    public void ShowOutLine()
    {
        outline.enabled = true;
    }

    public void HideOutLine()
    {
        outline.enabled = false;
    }
}

public static class ItemInstanceExtensions
{
    public static ItemData[] ToItemDatas(this ItemInstance[] itemInstances)
    {
        return itemInstances.Select(instance => instance?.itemData).ToArray();
    }
}
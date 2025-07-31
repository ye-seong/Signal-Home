using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector] public Dictionary<string, object> properties = new Dictionary<string, object>();

    [Header("Collider")]
    public GameObject itemCollider;

    [Header("Grip Settings")]
    public Transform gripPosition;
    public Transform leftHandGrip;
    public bool isTwoHanded = false;
    public bool isWorldItem;
    private bool isInitialized = false;

    private Outline outline;

    private void Start()
    {
        if (!isInitialized)
        {
            if (gripPosition == null)
            {
                GameObject grip = new GameObject("GripPosition");
                grip.transform.SetParent(transform);
                grip.transform.localPosition = Vector3.zero;
                gripPosition = grip.transform;
            }

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

        SetFlashLightOn(false);
    }

    private void OnDisable()
    {
        SetFlashLightOn(false);
    }
    public static ItemInstance Create(ItemData data, SavedItemInstance saveData = null)
    {
        GameObject obj = Instantiate(data.itemPrefab);
        ItemInstance instance = obj.GetComponent<ItemInstance>();
        instance.itemData = data;
        instance.IntializeProperties();

        if (saveData != null)
        {
            instance.LoadFromSaveData(saveData);
            instance.gameObject.SetActive(true);
        }

        return instance;
    }

    void IntializeProperties()
    {
        switch (itemData.productType)
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

        switch (itemData.itemType)
        {
            case ItemType.Armor:
                properties["durability"] = 100f;
                properties["armorModules"] = new ItemInstance[3];
                break;
            default:
                break;
        }

        if (itemData.itemName == "������")
        {
            properties["isFlashlightOn"] = false;
        }
    }



    public T Get<T>(string Key)
    {
        if (properties.TryGetValue(Key, out object value))
        {
            if (value is T result)
            {
                return result;
            }
        }
        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }

    public void Set<T>(string Key, T Value)
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

    public void SetFlashLightOn(bool isOn)
    {
        if (!itemData) return;
        if (itemData.itemName != "������") return;
        properties["isFlashlightOn"] = isOn;
        GetComponentInChildren<Light>().enabled = isOn;
    }

    // ����� ������ ���� (�迭 ���� ����)
    public SavedItemInstance ToSaveData()
    {
        SavedItemInstance saveData = new SavedItemInstance
        {
            itemName = itemData.itemName,
            position = transform.position,
            rotation = transform.rotation,
            isWorldItem = isWorldItem
        };

        // properties Dictionary�� ����ȭ ������ ���·� ��ȯ
        foreach (var kvp in properties)
        {
            // �迭�̳� ������ ��ü�� ���忡�� ����
            if (kvp.Value != null && kvp.Value.GetType().IsArray)
                continue;

            if (kvp.Key == "armorModules") // ItemInstance �迭 ����
                continue;

            saveData.propertyKeys.Add(kvp.Key);
            saveData.propertyValues.Add(JsonUtility.ToJson(kvp.Value));
            saveData.propertyTypes.Add(kvp.Value?.GetType().FullName ?? "null");
        }

        return saveData;
    }

    // ����� �����ͷκ��� ���� (���� ����)
    public void LoadFromSaveData(SavedItemInstance saveData)
    {
        transform.position = saveData.position;
        transform.rotation = saveData.rotation;

        // properties ���� (���� properties�� ������ �ʰ� �����)
        for (int i = 0; i < saveData.propertyKeys.Count; i++)
        {
            string key = saveData.propertyKeys[i];
            string valueJson = saveData.propertyValues[i];
            string typeName = saveData.propertyTypes[i];

            if (typeName == "null")
            {
                properties[key] = null;
                continue;
            }

            try
            {
                Type type = Type.GetType(typeName);
                if (type != null && !type.IsArray) // �迭 Ÿ���� ����
                {
                    object value = JsonUtility.FromJson(valueJson, type);
                    properties[key] = value;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to deserialize property {key}: {ex.Message}");
            }
        }
    }

}
public static class ItemInstanceExtensions
{
    public static ItemData[] ToItemDatas(this ItemInstance[] itemInstances)
    {
        return itemInstances.Select(instance => instance?.itemData).ToArray();
    }
}

[System.Serializable]
public class SavedItemInstance
{
    public string itemName;  // ItemData �ĺ���
    public Vector3 position;
    public Quaternion rotation;
    public Dictionary<string, object> properties;
    public bool isWorldItem;

    // Dictionary ����ȭ�� ���� ����
    public List<string> propertyKeys = new List<string>();
    public List<string> propertyValues = new List<string>();
    public List<string> propertyTypes = new List<string>();
}
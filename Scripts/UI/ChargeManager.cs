using System.Collections.Generic;
using UnityEngine;

public class ChargeManager : MonoBehaviour
{
    public ChargeType chargeType;

    private ItemInstance thisItem;
    [HideInInspector] public ItemInstance currentCharge;

    private int currentIndex;
    private UIManager uiManager;
    private Inventory inventory;

    private void Start()
    {
        thisItem = GetComponent<ItemInstance>();
        if (chargeType == ChargeType.Battery)
        {
            currentCharge = thisItem.Get<ItemInstance>("Battery");
        }
        else if (chargeType == ChargeType.Fuel)
        {
            currentCharge = thisItem.Get<ItemInstance>("Fuel");
        }
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
    }
    public void SetChargeSlots()
    {
        switch(chargeType)
        {
            case ChargeType.Battery:
                currentCharge = thisItem.Get<ItemInstance>("Battery");
                break;
            case ChargeType.Fuel:
                currentCharge = thisItem.Get<ItemInstance>("Fuel");
                break;
            default:
                break;
        }
        
        uiManager.chargeSlotPanel.SetActive(true);
        uiManager.ShowChargeSlots(ChargeArray(inventory.items), currentCharge);
        uiManager.SwitchChargeSlot(currentIndex);
    }

    public ItemInstance[] ChargeArray(ItemInstance[] items)
    {
        ProductType product = ProductType.None;
        switch(chargeType)
        {
            case ChargeType.Battery:
                product = ProductType.Battery;
                break;
            case ChargeType.Fuel:
                product = ProductType.Fuel;
                break;
            default:
                break;
        }

        List<ItemInstance> list = new List<ItemInstance>();
        if (currentCharge) list.Add(currentCharge);
        else list.Add(null);

        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i]) continue;
            if (items[i].itemData.productType == product)
            {
                list.Add(items[i]);
            }
        }
        if (currentCharge) list.Add(null);

        return list.ToArray();
    }
    public void SetcurrentCharge(ItemInstance[] items)
    {
        ItemInstance defaultCharge = currentCharge;
        currentCharge = ChargeArray(items)[currentIndex];
        if (chargeType == ChargeType.Battery)
        {
            thisItem.Set<ItemInstance>("Battery", currentCharge);
            if (defaultCharge != currentCharge)
            {
                thisItem.SetFlashLightOn(false);
            }
        }
        else if (chargeType == ChargeType.Fuel)
        {
            thisItem.Set<ItemInstance>("Fuel", currentCharge);
        }
        inventory.RemoveItemFromInstance(currentCharge);
        if (defaultCharge)
        {
            if (defaultCharge != currentCharge)
            {
                inventory.AddItem(defaultCharge);
                uiManager.UpdateItemUI();
            }
        }
        currentIndex = 0;
    }
    public void SetcurrentChargeIndex(string button)
    {
        switch(button)
        {
            case "left":
                if (currentIndex > 0) currentIndex--;
                break;
            case "right":
                if (currentIndex < ChargeArray(inventory.items).Length - 1) currentIndex++;
                break;
            default:
                break;
        }
        uiManager.SwitchChargeSlot(currentIndex);
    }
}

public enum ChargeType
{
    None,
    Battery,
    Fuel
}
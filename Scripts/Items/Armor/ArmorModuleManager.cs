using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorModuleManager : MonoBehaviour
{
    public static event System.Action<int, ItemInstance, ItemInstance> OnModuleChanged;
    public void AddModule(Inventory inventory, ItemInstance module, ItemInstance armor)
    {
        if (!inventory || !module) return;

        if (module.itemData.itemType != ItemType.ArmorMoudle) return;

        int index = -1;
        switch(module.itemData.armorModuleType)
        {
            case ArmorModuleType.Defense:
                index = 0;
                break;
            case ArmorModuleType.Resistance:
                index = 1;
                break;
            case ArmorModuleType.Skill:
                index = 2;
                break;
            default:
                return;
        }
        AddModuleInIndex(inventory, module, armor, index);
    }

    private void AddModuleInIndex(Inventory inventory, ItemInstance module, ItemInstance armor, int index)
    {
        OnModuleChanged?.Invoke(index, module, armor);
        ItemInstance[] armorModules = armor.Get<ItemInstance[]>("armorModules");
        if (!armorModules[index])
        {
            armorModules[index] = module;
            inventory.RemoveItemFromInstance(module);
        }
        else
        {
            inventory.AddItem(armorModules[index]);
            armorModules[index] = module;
            inventory.RemoveItemFromInstance(module);
        }
    }

}



using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bench : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;

    private ItemData[] items;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public bool CanCraft(PlayerState playerState, ItemData craftData)
    {
        ItemInstance[] invenDatas = playerState.inventoryItems;

        ItemData[] invenItemDatas = ItemInstanceExtensions.ToItemDatas(invenDatas);
        ItemData[] requiredItems = craftData.ingredients;

        for (int i = 0; i < invenDatas.Length; i++)
        {
            if (!invenDatas[i]) continue;
        }
        // 필요한 아이템들을 그룹화
        var requiredGroups = requiredItems.GroupBy(item => item.itemName);

        foreach (var group in requiredGroups)
        {
            
            string itemName = group.Key;
            int requiredCount = group.Count();
            int invenCount = GetItemCount(invenItemDatas, itemName);

            if (invenCount < requiredCount)
                return false;
        }

        return true;
    }

    public int GetItemCount(ItemData[] datas, string name)
    {
        int count = 0;
        for (int i = 0; i < datas.Length; i++)
        {
            if (!datas[i]) continue;
            if (datas[i].itemName == name)
            {
                count++;
            }
        }
        return count;
    }

    public void CraftItem(GameObject player, PlayerState playerState, ItemData craftData)
    {
        Inventory inventory = player.GetComponent<Inventory>();
        if (!inventory) return;

        foreach (var ingredient in craftData.ingredients)
        {
            inventory.RemoveItemFromName(ingredient.itemName);
        };
        ItemInstance instance = ItemInstance.Create(craftData, null);
        inventory.AddItem(instance);
    }
}

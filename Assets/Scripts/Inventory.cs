using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemInstance[] items;
    public int maxSlots = 21;
    public GameObject quickPanelObject;
    public GameObject UiManager;

    private PlayerState playerState;
    private QuickPanel quickPanel;
    [HideInInspector] public bool isHovering = false;
    private UIManager uiManager;

    [HideInInspector] public int draggedObjectIndex = -1;
    [HideInInspector] public int hoverIndex = -1;
    private void Start()
    {
        items = new ItemInstance[maxSlots];
        playerState = GetComponent<PlayerState>();
        if (quickPanelObject)
        {
            quickPanel = quickPanelObject.GetComponent<QuickPanel>();
        }
        if (UiManager)
        {
            uiManager = UiManager.GetComponent<UIManager>();
        }
    }

    private void Update()
    {
        
    }
    public bool AddItem(ItemInstance item)
    {
        if (!item) return false;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.gameObject.SetActive(false);
                
                playerState.UpdateInventory(items);
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(int index)
    {
        if (items.Length <= index) return false;
        items[index] = null;
        playerState.UpdateInventory(items);
        RemoveFromQuickSlot(index);
        return true;
    }

    public void RemoveFromQuickSlot(int index)
    {
        if (!quickPanel) return;
        for (int i = 0; i < quickPanel.quickSlotsIndex.Length; i++)
        {
            if (quickPanel.quickSlotsIndex[i] == index)
            {
                quickPanel.RemoveQuickSlotItem(i);
                return;
            }
        }
    }
    public void RemoveItemFromName(string itemName)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemData.itemName == itemName)
            {
                items[i] = null;
                RemoveFromQuickSlot(i);
                return;
            }
        }
    }

    public bool UseItem(int index)
    {
        if (index < 0 || index >= items.Length || !items[index] || !playerState) return false;
        return items[index].itemData.UseBioTypeItem(playerState);
    }   

    public void ThrowItem(int index)
    {
        ItemInstance item = items[index];

        if (!item) return;

        Vector3 dropPosition = transform.position + transform.forward * 0.5f + Vector3.up * 0.1f;

        item.transform.position = dropPosition;
        item.transform.rotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb)
        {
            Vector3 throwForce = transform.forward * 5f + Vector3.up * 2f;
            rb.AddForce(throwForce, ForceMode.Impulse);
            rb.isKinematic = false;
        }

        Collider[] colliders = item.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }

        item.transform.SetParent(null);
        item.gameObject.SetActive(true);
    }

    public void SetHoverIndex(int index)
    {
        if (index < 0)
        {
            hoverIndex = -1;
            isHovering = false;
            return;
        }
        hoverIndex = index;
        if (!items[index])
        {
            isHovering = false;
            return;
        }
        isHovering = true;
    }

    public void SwapInventoryItem(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= items.Length || indexB >= items.Length) return;

        ItemInstance temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;

        playerState.UpdateInventory(items);
        uiManager.UpdateItemUI();

        if (!quickPanel) return;

        SwapQuickSlots(indexA, indexB);
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) continue;
        }
    }

    private void SwapQuickSlots(int indexA, int indexB)
    {
        int isIndexA = -1;
        int isIndexB = -1;

        for (int i = 0; i < quickPanel.quickSlotsIndex.Length; i++)
        {
            if (quickPanel.quickSlotsIndex[i] == indexA)
            {
                isIndexA = i;
            }
            if (quickPanel.quickSlotsIndex[i] == indexB)
            {
                isIndexB = i;
            }
        }

        if (isIndexA == -1 && isIndexB == -1) return;
        
        if (isIndexA >= 0)
        {
            quickPanel.quickSlotsIndex[isIndexA] = indexB;
        }
        if (isIndexB >= 0)
        {
            quickPanel.quickSlotsIndex[isIndexB] = indexA;
        }
    }

}

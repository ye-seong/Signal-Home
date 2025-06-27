using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickPanel : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int slotIndex;
    [SerializeField] private GameObject[] quickSlots;

    [HideInInspector] public int currentSlotIndex = 0;

    [HideInInspector] public int maxSlots = 5;
    [HideInInspector] public int[] quickSlotsIndex;

    private Inventory inventory;
    private Image[] quickSlotIcons;
    private PlayerItemHandler playerItemHandler;

    void Start()
    {
        quickSlotsIndex = new int[] { -1, -1, -1, -1, -1 };
        quickSlotIcons = new Image[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            quickSlotIcons[i] = quickSlots[i].transform.Find("ItemIcon").GetComponent<Image>();
        }
        inventory = player.GetComponent<Inventory>();
        playerItemHandler = player.GetComponent<PlayerItemHandler>();
        HoldingItem();
    }

    public void SwitchQuickSlotItem()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            int key = i + 1;
            if (Input.GetKeyDown(key.ToString()))
            {
                ItemInstance itemData = inventory.items[inventory.hoverIndex];

                if (itemData)
                {
                    // A 아이템이 이미 퀵슬롯에 있는지 찾기
                    int aItemSlotIndex = -1;
                    for (int j = 0; j < maxSlots; j++)
                    {
                        if (quickSlotsIndex[j] == inventory.hoverIndex)
                        {
                            aItemSlotIndex = j; // A 아이템이 j번째에 있음
                            break;
                        }
                    }

                    if (aItemSlotIndex != -1) // A 아이템이 이미 퀵슬롯에 있을 때
                    {
                        // 스와핑 로직
                        SwapQuickSlotItems(aItemSlotIndex, i);
                    }
                    else // A 아이템이 퀵슬롯에 없을 때
                    {
                        // 일반 추가
                        AddQuickSlotItem(i, inventory.hoverIndex, itemData);
                    }
                    HoldingItem();
                }
                else
                {
                    RemoveQuickSlotItem(i);
                }
                return;
            }
        }
    }

    private void SwapQuickSlotItems(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return; // 같은 자리면 아무것도 안 함

        // 임시 저장
        int tempFromItem = quickSlotsIndex[fromIndex];  // A 아이템 인덱스
        int tempToItem = quickSlotsIndex[toIndex];      // B 아이템 인덱스 (없을 수도 있음)

        // A 아이템을 toIndex로 이동
        if (tempFromItem != -1)
        {
            ItemInstance aItemData = inventory.items[tempFromItem];
            AddQuickSlotItem(toIndex, tempFromItem, aItemData);
        }

        // B 아이템을 fromIndex로 이동 (B 아이템이 있었다면)
        if (tempToItem != -1)
        {
            ItemInstance bItemData = inventory.items[tempToItem];
            AddQuickSlotItem(fromIndex, tempToItem, bItemData);
        }
        else
        {
            // B 아이템이 없었다면 fromIndex는 비우기
            RemoveQuickSlotItem(fromIndex);
        }
    }

    public void AddQuickSlotItem(int index, int invenIndex, ItemInstance itemData)
    {
        quickSlotsIndex[index] = invenIndex;
        quickSlotIcons[index].sprite = itemData.itemData.icon;
        quickSlotIcons[index].color = new Color(1f, 1f, 1f, 1f);
    }

    public void RemoveQuickSlotItem(int index)
    {
        quickSlotsIndex[index] = -1;
        quickSlotIcons[index].sprite = null;
        quickSlotIcons[index].color = new Color(1f, 1f, 1f, 0f);
    }

    private void RemoveCurrentHoldingItem()
    {
        if (!playerItemHandler.currentItem) return;
        playerItemHandler.ClearHolding();
        //int inventoryIndex = quickSlotsIndex[currentSlotIndex];
        //if (inventoryIndex < 0) return;
        //int itemId = inventory.items[inventoryIndex].itemData.itemID;

        //if (!playerItemHandler.currentItem) return;

        //if (itemId != playerItemHandler.currentItem.itemData.itemID)
        //{
        //    Debug.Log(playerItemHandler.currentItem.itemData.itemName + "을 내림");
        //    playerItemHandler.ClearHolding();
        //}
    }

    public void HoldingItem()
    {
        RemoveCurrentHoldingItem();
        int invenIndex = quickSlotsIndex[currentSlotIndex];
        if (!inventory) return;
        MarkCurrentSlot();
        if (invenIndex < 0)
        {
            player.GetComponent<PlayerItemHandler>().SetHolding(null);
            return;
        }
        ItemInstance item = inventory.items[invenIndex];
        if (item)
        {
            player.GetComponent<PlayerItemHandler>().SetHolding(item);
        }
        else
        {
            player.GetComponent<PlayerItemHandler>().SetHolding(null);
        }
    }

    private void MarkCurrentSlot()
    {
        Image[] quickSlotBackground = new Image[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            quickSlotBackground[i] = quickSlots[i].transform.Find("SlotBackground").GetComponent<Image>();
        }
        
        if (currentSlotIndex < 0 || currentSlotIndex >= quickSlots.Length) return;

        for (int i = 0; i < quickSlotBackground.Length; i++)
        {
            if (!quickSlotBackground[i]) return;
            if (i == currentSlotIndex)
            {
                quickSlotBackground[i].color = new Color(1f, 0.5f, 0f, 1f);
            }
            else
            {
                quickSlotBackground[i].color = new Color(217, 217, 217, 225);
            }
        }
    }
}

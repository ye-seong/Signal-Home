using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuickPanel : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int slotIndex;
    [SerializeField] private GameObject[] quickSlots;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerItemHandler playerItemHandler;

    [HideInInspector] public int currentSlotIndex = 0;

    [HideInInspector] public int maxSlots = 5;

    
    private Image[] quickSlotIcons;

    private PlayerState playerState;
    
    private void Awake()
    {
        if (player)
        {
            playerState = player.GetComponent<PlayerState>();
        }
        quickSlotIcons = new Image[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            quickSlotIcons[i] = quickSlots[i].transform.Find("ItemIcon").GetComponent<Image>();
        }
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
                    // A �������� �̹� �����Կ� �ִ��� ã��
                    int aItemSlotIndex = -1;
                    for (int j = 0; j < maxSlots; j++)
                    {
                        if (playerState.quickSlotIndexs[j] == inventory.hoverIndex)
                        {
                            aItemSlotIndex = j; // A �������� j��°�� ����
                            break;
                        }
                    }

                    if (aItemSlotIndex != -1) // A �������� �̹� �����Կ� ���� ��
                    {
                        // ������ ����
                        SwapQuickSlotItems(aItemSlotIndex, i);
                    }
                    else // A �������� �����Կ� ���� ��
                    {
                        // �Ϲ� �߰�
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
        if (fromIndex == toIndex) return; // ���� �ڸ��� �ƹ��͵� �� ��

        // �ӽ� ����
        int tempFromItem = playerState.quickSlotIndexs[fromIndex];  // A ������ �ε���
        int tempToItem = playerState.quickSlotIndexs[toIndex];      // B ������ �ε��� (���� ���� ����)

        // A �������� toIndex�� �̵�
        if (tempFromItem != -1)
        {
            ItemInstance aItemData = inventory.items[tempFromItem];
            AddQuickSlotItem(toIndex, tempFromItem, aItemData);
        }

        // B �������� fromIndex�� �̵� (B �������� �־��ٸ�)
        if (tempToItem != -1)
        {
            ItemInstance bItemData = inventory.items[tempToItem];
            AddQuickSlotItem(fromIndex, tempToItem, bItemData);
        }
        else
        {
            // B �������� �����ٸ� fromIndex�� ����
            RemoveQuickSlotItem(fromIndex);
        }
    }

    public void AddQuickSlotItem(int index, int invenIndex, ItemInstance itemData)
    {
        //playerState = player.GetComponent<PlayerState>();

        playerState.quickSlotIndexs[index] = invenIndex;
        quickSlotIcons[index].sprite = itemData.itemData.icon;
        quickSlotIcons[index].color = new Color(1f, 1f, 1f, 1f);
    }

    public void RemoveQuickSlotItem(int index)
    {
        playerState.quickSlotIndexs[index] = -1;
        quickSlotIcons[index].sprite = null;
        quickSlotIcons[index].color = new Color(1f, 1f, 1f, 0f);
    }

    private void RemoveCurrentHoldingItem()
    {
        if (!playerItemHandler.currentItem) return;
        playerItemHandler.ClearHolding();
    }

    public void HoldingItem()
    {
        RemoveCurrentHoldingItem();
        int invenIndex = playerState.quickSlotIndexs[currentSlotIndex];
        if (!inventory) return;
        MarkCurrentSlot();
        if (invenIndex < 0)
        {
            playerItemHandler.SetHolding(null);
            return;
        }
        ItemInstance item = inventory.items[invenIndex];
        if (item)
        {
            playerItemHandler.SetHolding(item);
        }
        else
        {
            playerItemHandler.SetHolding(null);
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

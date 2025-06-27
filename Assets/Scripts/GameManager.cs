using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    public static bool IsUIOpen = false;
    public ItemData[] allItems;

    private PlayerState playerState;
    private void Start()
    {
        playerState = player.GetComponent<PlayerState>();
        GetAllItems();
        InitializeUnlockedItems();
    }

    private void GetAllItems()
    {
        allItems = Resources.LoadAll<ItemData>("Items");
        Array.Sort(allItems, (a, b) => a.itemID - b.itemID);
    }

    private void InitializeUnlockedItems()
    {
        playerState.unlockedItems = new bool[allItems.Length];

        foreach (ItemData item in allItems)
        {
            playerState.unlockedItems[item.itemID] = false;
        }
    }
}

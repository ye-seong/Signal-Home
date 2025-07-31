using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("Scripts")]
    public ZoneManager zoneManager;
    public SaveManager saveManager;
    public UIManager uiManager;

    [Header("Semaphore")]
    public SemaphoreSystem[] semaphores;

    public static bool IsUIOpen = false;
    public ItemData[] allItems;
    public EnemyData[] allEnemies;

    private PlayerState playerState;
    private void Start()
    {
        playerState = player.GetComponent<PlayerState>();
        GetAllItems();
        GetAllEnemies();
        InitializeUnlockedItems();
        zoneManager.SpawnEnemyByZone(allEnemies);
        saveManager.LoadGame();
        if (!File.Exists(saveManager.savePlayerPath))
        {
            saveManager.SaveStart();
        }
        for (int i = 0; i < GameState.currentSemaphoreNumber + 1; i++)
        {
            SetActiveSemaphore(i);
        }
    }

    private void GetAllItems()
    {
        allItems = Resources.LoadAll<ItemData>("Items");
        Array.Sort(allItems, (a, b) => a.itemID - b.itemID);
    }

    private void GetAllEnemies()
    {
        allEnemies = Resources.LoadAll<EnemyData>("Enemies");
        Array.Sort(allEnemies, (a, b) => a.enemyID - b.enemyID);
    }
    private void InitializeUnlockedItems()
    {
        playerState.unlockedItems = new bool[allItems.Length];

        foreach (ItemData item in allItems)
        {
            playerState.unlockedItems[item.itemID] = false;
        }
    }
    public void SetActiveSemaphore(int num)
    {
        if (num < 0 || num >= semaphores.Length) return;
        semaphores[num].gameObject.SetActive(true);
        if (num < GameState.currentSemaphoreNumber)
        {
            semaphores[num].isUnlocked = true;
        }
    }

    public void SetOperateSemaphore()
    {
        if (!GameState.IsOperate) return;
        foreach (SemaphoreSystem semaphore in semaphores)
        {
            semaphore.lineRenderer.enabled = true;
        }
    }
}

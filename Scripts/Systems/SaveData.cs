using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SavePlayerData
{
    public PlayerStats playerStats;
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public SavedItemInstance[] inventoryItems;
    public SavedItemInstance[] equipmentItems;
    
    public bool[] unlockedItems;
    public int[] quickSlotIndexs;
    public int currentHandleItemIndex;

    public bool isOperate;
    public DateTime currentTime;
}

[System.Serializable]
public class SaveMapData
{
    public SavedItemInstance[] mapItems;
    public int semaphoreNumber;
}

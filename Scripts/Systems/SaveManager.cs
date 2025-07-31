using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [HideInInspector] public string savePlayerPath => Path.Combine(Application.persistentDataPath, "savePlayerData.json");
    [HideInInspector] public string saveMapPath => Path.Combine(Application.persistentDataPath, "saveMapData.json");


    public void GoMenuScene()
    {
        //Debug.Log("메인 메뉴로 이동합니다.");
        Time.timeScale = 1f;
        playerController.isPaused = false;
        GameState.IsUIOpen = false;
        SceneManager.LoadScene("MenuScene");
    }
    public virtual void SaveStart()
    {
        SavePlayerData savePlayerData = CollectAllPlayerData();

        string json = JsonUtility.ToJson(savePlayerData, true);
        File.WriteAllText(savePlayerPath, json);
    }
    public virtual void SaveGame()
    {
        //Debug.Log("게임 저장 시작!");
        SavePlayerData savePlayerData = CollectAllPlayerData();
        SaveMapData saveMapData = CollectAllMapData();

        string json = JsonUtility.ToJson(savePlayerData, true);
        File.WriteAllText(savePlayerPath, json);
        json = JsonUtility.ToJson(saveMapData, true);
        File.WriteAllText(saveMapPath, json);

        //Debug.Log("게임 저장 완료!");
    }

    public virtual void LoadGame()
    {
        if (File.Exists(savePlayerPath))
        {
            string json = File.ReadAllText(savePlayerPath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(json);
            json = File.ReadAllText(saveMapPath);
            SaveMapData saveMapData = JsonUtility.FromJson<SaveMapData>(json);
            ApplyAllGameData(savePlayerData, saveMapData);
        }
        
        //if (File.Exists(savePlayerPath) && File.Exists(saveMapPath))
        //{
        //    string json = File.ReadAllText(savePlayerPath);
        //    SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(json);
        //    json = File.ReadAllText(saveMapPath);
        //    SaveMapData saveMapData = JsonUtility.FromJson<SaveMapData>(json);
        //    ApplyAllGameData(savePlayerData, saveMapData);
        //    //Debug.Log("게임 불러오기 완료!");
        //}
        //else if (File.Exists(saveMapPath))
        //{
        //    string json = File.ReadAllText(saveMapPath);
        //    SaveMapData saveMapData = JsonUtility.FromJson<SaveMapData>(json);
        //    ApplyAllGameData(null, saveMapData);
        //}
    }
    private SavePlayerData CollectAllPlayerData()
    {
        SavePlayerData saveData = new SavePlayerData();
        if (playerState)
        {
            saveData.playerStats = playerState.stats;
            saveData.playerPosition = playerState.transform.position;
            saveData.playerRotation = playerState.transform.rotation;
            saveData.quickSlotIndexs = playerState.quickSlotIndexs;
            saveData.currentHandleItemIndex = uiManager.quickSlotPanel.GetComponent<QuickPanel>().currentSlotIndex;

            // equipmentItems 배열 초기화
            saveData.equipmentItems = new SavedItemInstance[playerState.equipmentItems.Length];
            for (int i = 0; i < playerState.equipmentItems.Length; i++)
            {
                if (playerState.equipmentItems[i])
                {
                    saveData.equipmentItems[i] = playerState.equipmentItems[i].ToSaveData();
                }
                else
                {
                    saveData.equipmentItems[i] = null;
                }
            }

            saveData.inventoryItems = new SavedItemInstance[playerState.inventoryItems.Length];
            for (int i = 0; i < playerState.inventoryItems.Length; i++) 
            {
                if (playerState.inventoryItems[i])
                {
                    saveData.inventoryItems[i] = playerState.inventoryItems[i].ToSaveData();
                }
                else
                {
                    saveData.inventoryItems[i] = null;
                }
            }

            saveData.unlockedItems = playerState.unlockedItems;
            saveData.isOperate = GameState.IsOperate;
            saveData.currentTime = System.DateTime.Now;
        }
        return saveData;
    }

    private SaveMapData CollectAllMapData()
    {
        SaveMapData saveData = new SaveMapData();
        List<SavedItemInstance> worldItems = new List<SavedItemInstance>();

        foreach (ItemInstance item in FindObjectsOfType<ItemInstance>())
        {
            if (item)
            {
                if (item.isWorldItem)
                {
                    worldItems.Add(item.ToSaveData());
                }
            }
        }
        saveData.mapItems = worldItems.ToArray();
        saveData.semaphoreNumber = GameState.currentSemaphoreNumber;
        return saveData;
    }
    private void ApplyAllGameData(SavePlayerData savePlayerData, SaveMapData saveMapData)
    {
        if (playerState)
        {
            playerState.quickSlotIndexs = new int[] { -1, -1, -1, -1, -1 };
            playerState.inventoryItems = new ItemInstance[GameConstants.MAX_INVENTORY_SIZE];
            if (savePlayerData != null)
            {
                playerState.stats = savePlayerData.playerStats;
                playerState.transform.position = savePlayerData.playerPosition;
                playerState.transform.rotation = savePlayerData.playerRotation;

                // inventoryItems 복원 (Create 메서드로 통일)
                for (int i = 0; i < savePlayerData.inventoryItems.Length; i++)
                {
                    if (savePlayerData.inventoryItems[i] != null)
                    {
                        ItemData itemData = FindItemDataByName(savePlayerData.inventoryItems[i].itemName);
                        if (itemData != null)
                        {
                            playerState.inventoryItems[i] = ItemInstance.Create(itemData, savePlayerData.inventoryItems[i]);
                            playerState.inventoryItems[i].gameObject.SetActive(false);
                        }
                    }
                }
                uiManager.inventory.items = playerState.inventoryItems;
                uiManager.UpdateItemUI();
                // equipmentItems 복원 (Create 메서드로 통일)
                playerState.equipmentItems = new ItemInstance[savePlayerData.equipmentItems.Length];
                for (int i = 0; i < savePlayerData.equipmentItems.Length; i++)
                {
                    if (savePlayerData.equipmentItems[i] != null)
                    {
                        ItemData itemData = FindItemDataByName(savePlayerData.equipmentItems[i].itemName);
                        if (itemData != null)
                        {
                            playerState.equipmentItems[i] = ItemInstance.Create(itemData, savePlayerData.equipmentItems[i]);
                            playerState.equipmentItems[i].gameObject.SetActive(false);
                        }
                    }
                }

                playerState.quickSlotIndexs = savePlayerData.quickSlotIndexs;
                uiManager.quickSlotPanel.GetComponent<QuickPanel>().currentSlotIndex = savePlayerData.currentHandleItemIndex;

                uiManager.RefreshQuickSlotItems(playerState.quickSlotIndexs);
                uiManager.RefreshEquipmentItems(playerState.equipmentItems);

                if (uiManager.quickSlotPanel.GetComponent<QuickPanel>().currentSlotIndex >= 0)
                {
                    if (playerState.inventoryItems[savePlayerData.currentHandleItemIndex])
                    {
                        int index = playerState.quickSlotIndexs[savePlayerData.currentHandleItemIndex];
                        if (index >= 0)
                        {
                            playerState.playerItemHandler.currentItem = playerState.inventoryItems[index];
                            playerState.playerItemHandler.SetHoldingItem(playerState.playerItemHandler.currentItem, playerState.playerItemHandler.rightHandBone);
                        }
                    }
                }
            }

            foreach (SavedItemInstance savedItem in saveMapData.mapItems)
            {
                if (savedItem != null)
                {
                    ItemData itemData = FindItemDataByName(savedItem.itemName);
                    if (itemData != null)
                    {
                        ItemInstance worldItem = ItemInstance.Create(itemData, savedItem);
                        worldItem.isWorldItem = true;
                        worldItem.gameObject.SetActive(true);
                        worldItem.transform.position = savedItem.position;
                        worldItem.transform.rotation = savedItem.rotation;
                    }
                }
            }

            if (savePlayerData != null)
            {
                playerState.unlockedItems = savePlayerData.unlockedItems;
                GameState.IsOperate = savePlayerData.isOperate;
            }
            
            GameState.currentSemaphoreNumber = saveMapData.semaphoreNumber;
            playerState.RefreshUI();
        }
    }

    // ItemData를 이름으로 찾는 헬퍼 함수 (필요하면 추가)
    private ItemData FindItemDataByName(string itemName)
    {
        for (int i = 0; i < gameManager.allItems.Length; i++)
        {
            if (gameManager.allItems[i].itemName == itemName)
            {
                return gameManager.allItems[i];
            }
        }
        // 여기서 ItemData를 찾는 로직 구현
        // 예: Resources.Load나 미리 만든 ItemData 리스트에서 찾기
        return Resources.Load<ItemData>("Items/" + itemName);
    }

}

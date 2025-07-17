using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class UseItem : MonoBehaviour
{
    public GameObject GameManager;

    private PlayerState playerState;
    private InteractionDetector interactionDetector;
    private GameManager gameManager;
    private UIManager uiManager;
    private PlayerItemHandler playerItemHandler;

    private bool isScan = false;
    private float scanTime = 0f;
    private ItemInstance scanItem;

    private RaycastHit hit;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        interactionDetector = GetComponent<InteractionDetector>();
        gameManager = GameManager.GetComponent<GameManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerItemHandler = GetComponent<PlayerItemHandler>();    
    }

    private void Update()
    {
        UseScannerInUpdate();
    }
    public void UseItems(ItemInstance item)
    {
        switch(item.itemData.itemName)
        {
            case "��ĳ��":
                UseScanner();
                break;
            case "������":
                UseKnife(item.itemData.attackPower);
                break;
            default:
                return;
        }
    }

    private void UseScannerInUpdate()
    {
        if (!playerItemHandler.currentItem)
        {
            scanTime = 0f;
            isScan = false;
            uiManager.scanBar.SetActive(false);
            return;
        }

        if (isScan && Input.GetMouseButton(1))
        {
            scanTime += Time.deltaTime;
            if (scanTime >= 2f)
            {
                isScan = false;
                scanTime = 0f;
                playerState.AddUnlockedItems(scanItem.itemData);
                uiManager.scanBar.SetActive(false);
            }
            else
            {
                if (uiManager)
                {
                    uiManager.UpdateScanUI(scanTime);
                }
            }
        }

        if ((isScan && Input.GetMouseButtonUp(1) && scanTime > 0f) || !interactionDetector.GetCurrentTarget() || playerItemHandler.currentItem.itemData.itemName != "��ĳ��")
        {
            scanTime = 0f;
            isScan = false;
            uiManager.scanBar.SetActive(false);
        }
    }
    private void UseScanner()
    {
        if (!interactionDetector || !playerState) return;
        scanItem = interactionDetector.GetCurrentTarget();
        if (!scanItem)
        {
            return;
        }
        int id = scanItem.itemData.itemID;
        
        if (playerState.unlockedItems[id])
        {
            return;
        }
        if (gameManager.allItems[id].itemType == ItemType.Resources)
        {
            return;
        }

        isScan = true;
    }

    private void UseKnife(float damage)
    {
        LivingEntity entity = interactionDetector.GetLivingEntityTarget();

        if (entity is Animal animal)
        {
            animal.TakeDamage(damage);
            Debug.Log($"ǥ�� : {animal.animalData.animalName} | ������ : {damage} | ���� HP : {animal.GetCurrentHealth()}");
        }
        else if (entity is Enemy enemy)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"ǥ�� : {enemy.enemyData.enemyName} | ������ : {damage} | ���� HP : {enemy.GetCurrentHealth()}");
        }
        else if (entity is DestuctibleObject obj)
        {
            obj.TakeDamage(damage);
            Debug.Log($"ǥ�� : {obj.destuctibleData.destuctibleName} | ������ : {damage} | ���� HP : {obj.GetCurrentHealth()}");
        }
    }
}

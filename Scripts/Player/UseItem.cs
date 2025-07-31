using UnityEngine;

public class UseItem : MonoBehaviour
{
    public GameObject GameManager;

    private PlayerState playerState;
    private InteractionDetector interactionDetector;
    private GameManager gameManager;
    private UIManager uiManager;
    private PlayerItemHandler playerItemHandler;
    private Inventory inventory;
    private PlayerController playerController;

    private bool isScan = false;
    private float scanTime = 0f;
    private ItemInstance scanItem;
    public bool chargeUIOpen = false;

    private float attackCooldown = 0f;
    [HideInInspector] public float maxAttackCooldown;
    private bool useWeapon = false;

    private RaycastHit hit;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        interactionDetector = GetComponent<InteractionDetector>();
        gameManager = GameManager.GetComponent<GameManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerItemHandler = GetComponent<PlayerItemHandler>();    
        inventory = GetComponent<Inventory>();
        playerController = GetComponent<PlayerController>();    
    }

    private void Update()
    {
        if (playerItemHandler.currentItem)
        {
            ReloadCharge(playerItemHandler.currentItem);
            if (useWeapon)
            {
                WeaponAttackCooldown();
            }
        }
        UseScannerInUpdate();
        UseFlashlightInUpdate();
    }
    public void UseItems(ItemInstance item)
    {
        switch(item.itemData.itemName)
        {
            case "��ĳ��":
                if (Input.GetMouseButtonDown(1)) UseScanner();
                break;
            case "������":
                if (Input.GetMouseButtonDown(0)) UseKnife(item.itemData.attackPower);
                break;
            case "��Ʈ��":
                //if (Input.GetMouseButtonDown(1)) SetJetPack(item);
                break;
            case "������":
                if (Input.GetMouseButtonDown(1)) UseFlashlight(item);
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

        bool isCan = false;
        if (isScan && Input.GetMouseButton(1) && !chargeUIOpen)
        {
            isCan = CanUseBatteryProduct();
            
            if (isCan)
            {
                UseBattery();
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
        }

        if ((isScan && Input.GetMouseButtonUp(1) && scanTime > 0f) || !interactionDetector.GetCurrentTarget() || 
            playerItemHandler.currentItem.itemData.itemName != "��ĳ��" || !isCan)
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
        useWeapon = true;
        if (attackCooldown > 0f)
        {
            //Debug.Log("��Ÿ���� ���ƿ��� �ʾҽ��ϴ�!");
            return;
        }
        playerController.animator.SetTrigger("DoUseKnife");
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

    private void UseFlashlightInUpdate()
    {
        if (!playerItemHandler.currentItem) return;
        if (playerItemHandler.currentItem.itemData.itemName != "������") return;

        bool isBattery = CanUseBatteryProduct();

        if (!isBattery)
        {
            playerItemHandler.currentItem.gameObject.GetComponentInChildren<Light>().enabled = false;
            return;
        }
        
        if (playerItemHandler.currentItem.Get<bool>("isFlashlightOn"))
        {
            UseBattery();
        }
    }
    private void UseFlashlight(ItemInstance item)
    {
        if (!inventory || !item || chargeUIOpen || GameState.IsUIOpen || !CanUseBatteryProduct()) return;
        item.gameObject.GetComponentInChildren<Light>().enabled = !item.gameObject.GetComponentInChildren<Light>().enabled;
        item.Set<bool>("isFlashlightOn", item.gameObject.GetComponentInChildren<Light>().enabled);
    }

    private void SetJetPack(ItemInstance item)
    {
        if (chargeUIOpen) return;
        playerItemHandler.SetBackSocket(item);
        inventory.RemoveItemFromInstance(item);
        uiManager.UpdateItemUI();
    }

    private void ReloadCharge(ItemInstance item)
    {
        if (!inventory || GameState.IsUIOpen) return;
        ChargeManager chargeManager = item.GetComponent<ChargeManager>();

        if (Input.GetKeyDown(KeyCode.R) && chargeManager)
        {
            if (!chargeUIOpen)
            { 
                chargeManager.SetChargeSlots();
                chargeUIOpen = true;
            }
            else
            {
                chargeManager.SetcurrentCharge(inventory.items);
                uiManager.ClearChargeSlots();
                uiManager.chargeSlotPanel.SetActive(false);
                chargeUIOpen = false;
            }
        }
        if (chargeUIOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                chargeManager.SetcurrentChargeIndex("left");
            }
            else if (Input.GetMouseButtonDown(1))
            {
                chargeManager.SetcurrentChargeIndex("right");
            }
        }
    }

    private bool CanUseBatteryProduct()
    {
        ItemInstance battery = playerItemHandler.currentItem.Get<ItemInstance>("Battery");
        if (!battery)
        {
            //Debug.Log("���͸��� �����ϼ���!");
            return false;
        }
        float rate = battery.Get<float>("batteryUsageRate");
        if (rate <= 0)
        {
            //Debug.Log("���͸��� ��� �����Ǿ����ϴ�.");
            return false;
        }
        return true;
    }

    private void UseBattery()
    {
        ItemInstance battery = playerItemHandler.currentItem.Get<ItemInstance>("Battery");
        float rate = battery.Get<float>("batteryUsageRate");
        rate -= Time.deltaTime;
        battery.Set<float>("batteryUsageRate", rate);
        uiManager.SetChargeText();
    }
    
    private void WeaponAttackCooldown()
    {
        if (playerItemHandler.currentItem.itemData.itemType == ItemType.Weapon)
        {
            if (attackCooldown <= maxAttackCooldown)
            {
                attackCooldown += Time.deltaTime;
            }
            else
            {
                //Debug.Log("��Ÿ���� ���ƿԽ��ϴ�!");
                attackCooldown = 0f;
                useWeapon = false;
            }
        }
    }
}

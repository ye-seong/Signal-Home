using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Bar")]
    public Image healthBar;
    public Image satietyBar;
    public Image hydrationBar;
    public GameObject scanBar;
    public Image scanGaugeBar;

    [Header("Text")]
    public Text healthText;
    public Text satietyText;
    public Text hydrationText;

    [Header("Slot")]
    public GameObject inventorySlotPrefab;
    public GameObject benchSlotPrefab;
    public GameObject quickSlotPrefab;

    [Header("UI Panels")]
    public GameObject inventoryPanel;   
    public GameObject inventorySlotPanel;
    public GameObject benchPanel;
    public GameObject benchSlotPanel;
    public GameObject quickSlotPanel;

    [Header("Prefabs")]
    public GameObject bench;

    [Header("Player")]
    public GameObject player;

    [Header("Game")]
    public GameObject GameManager;

    private ScrollRect benchScrollRect;

    private PlayerState playerState;

    private KeyCode inventoryKey = KeyCode.E;
    private bool isInventoryOpen = false;
    public bool isBenchOpen = false;

    private Inventory inventory;
    private GameManager gameManager;

    void Start()
    {
        if (benchPanel != null)
        {
            benchScrollRect = benchPanel.GetComponentInChildren<ScrollRect>();
        }
        if (player != null)
        {
            playerState = player.GetComponent<PlayerState>();
            UpdateUI(playerState.stats);
            playerState.OnStatsChanged += UpdateUI;
        }
        if (inventoryPanel != null && inventorySlotPanel != null)
        {
            inventory = player.GetComponent<Inventory>();
            UpdateItemUI();
        }
        if (GameManager != null)
        {
            gameManager = GameManager.GetComponent<GameManager>();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    public void OnInventorySlotDoubleClick(int slotIndex)
    {
        if (!inventory.items[slotIndex] || !inventory.UseItem(slotIndex)) return;
        Destroy(inventory.items[slotIndex].gameObject);
        inventory.RemoveItem(slotIndex);
        UpdateItemUI();
    }

    private void ToggleInventory()
    {
        if (!inventoryPanel || isBenchOpen) return;
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        SetMouseSate();
    }

    public void ToggleBench()
    {
        if (!benchPanel) return;
        isBenchOpen = !isBenchOpen;
        benchPanel.SetActive(isBenchOpen);

        if (isBenchOpen)
        {
            InitializeBenchUI();
        }
        if (isBenchOpen && benchScrollRect != null)
        {
            benchScrollRect.verticalNormalizedPosition = 1f;
        }
        SetMouseSate();
    }

    private void SetMouseSate()
    {
        if (isInventoryOpen || isBenchOpen)
        {
            GameState.IsUIOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            GameState.IsUIOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateUI(PlayerStats stats)
    {
        if (healthBar && satietyBar && hydrationBar)
        {
            healthBar.fillAmount = stats.health / GameConstants.MAX_HEALTH;
            satietyBar.fillAmount = stats.satiety / GameConstants.MAX_SATIETY;
            hydrationBar.fillAmount = stats.hydration / GameConstants.MAX_HYDRATION;
        }

        if (healthText && satietyText && hydrationText)
        {
            healthText.text = stats.health.ToString("F0");  
            satietyText.text = stats.satiety.ToString("F0");
            hydrationText.text = stats.hydration.ToString("F0");
        }
    }

    public void UpdateScanUI(float time)
    {
        scanBar.SetActive(true);
        if (scanBar && scanGaugeBar)
        {
            scanGaugeBar.fillAmount = time / 2f;
        }
    }
    public void UpdateItemUI()
    {
        if (!inventory || !inventorySlotPanel) return;
        for (int i = 0; i < inventory.items.Length; i++)
        {
            Transform transform = inventorySlotPanel.transform.GetChild(i);

            Image itemImage = transform.Find("ItemIcon").GetComponent<Image>();

            if (inventory.items[i] == null)
            {
                itemImage.sprite = null;
                itemImage.color = new Color(0, 0, 0, 0);
            }
            else
            {
                itemImage.sprite = inventory.items[i].itemData.icon;
                itemImage.color = Color.white;
            }
        }
    }
    public void OnBenchSlotClick(int slotIndex)
    {
        bool isCan = bench.GetComponent<Bench>().CanCraft(playerState, gameManager.allItems[slotIndex]);
        if (isCan)
        {
            bench.GetComponent<Bench>().CraftItem(player, playerState, gameManager.allItems[slotIndex]);
            //Debug.Log("������ �Ϸ��߽��ϴ�!");
            UpdateItemUI();
        }
        else
        {
            //Debug.Log("���ۿ� �ʿ��� ��ᰡ �����մϴ�!");
        }
    }

    public void InitializeBenchUI()
    {
        if (!benchPanel || !benchSlotPanel) return;

        // ���� ���۴� ���� �ʱ�ȭ
        for (int i = benchSlotPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(benchSlotPanel.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < playerState.unlockedItems.Length; i++)
        {
            if (playerState.unlockedItems[i])
            {
                GameObject slot = Instantiate(benchSlotPrefab, benchSlotPanel.transform);
                BenchSlot slotData = slot.GetComponent<BenchSlot>();

                Image itemImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
                itemImage.sprite = gameManager.allItems[i].icon;
                itemImage.color = Color.white;

                slotData.SetSlotIndex(i);
            }
        }
    }

    public void ClearTooltip(GameObject Tooltip)
    {
        for (int i = Tooltip.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(Tooltip.transform.GetChild(i).gameObject);
        }
    }
}
 
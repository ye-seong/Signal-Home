using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform bodyCenter;
    [SerializeField] private GameManager gameManager;

    private PlayerItemHandler playerItemHandler;
    private RaycastHit hit;
    private KeyCode InteractionKey = KeyCode.F;

    private ItemInstance highlightedItem;
    private Inventory inventory;
    private PlayerController playerController;
    void Start()
    {
       playerItemHandler = GetComponent<PlayerItemHandler>();
        inventory = GetComponent<Inventory>();
        playerController = GetComponent<PlayerController>();
    }
    void Update()
    {
        SetHighlight();
        if (Input.GetKeyDown(InteractionKey) && !playerController.isPaused)
        {
            CheckInteraction();
        }
    }

    private void CheckInteraction()
    {
        if (CheckIsUIOpen()) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.collider.CompareTag("Bench"))
            {
                uiManager.ToggleBench(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Semaphore"))
            {
                if (GameState.IsOperate) return;
                AddCrystalKey(hit.collider.gameObject);
            }
        }
    }

    private void AddCrystalKey(GameObject hitObject)
    {
        if (playerItemHandler.currentItem)
        {
            if (playerItemHandler.currentItem.itemData.itemID == 31)
            {
                SemaphoreSystem semaphore = hitObject.GetComponent<SemaphoreSystem>();
                if (semaphore)
                {
                    semaphore.SpawnKeyCrystal();
                    GameState.currentSemaphoreNumber++;
                    gameManager.SetActiveSemaphore(GameState.currentSemaphoreNumber);
                    inventory.RemoveItemFromInstance(playerItemHandler.currentItem);
                }
            }
        }
    }
    private bool CheckIsUIOpen()
    {
        if (uiManager.isBenchOpen)
        {
            GameObject Tooltip = GameObject.Find("Tooltip");
            if (Tooltip) uiManager.ClearTooltip(Tooltip);
            uiManager.ToggleBench(uiManager.currentBench);
            uiManager.currentBench = null;
            return true;
        }
        return false;
    }

    public ItemInstance GetCurrentTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, 50f))
        {
            float distanceFromBody = Vector3.Distance(bodyCenter.position, hit.collider.transform.position);

            ItemInstance item = hit.collider.GetComponentInParent<ItemInstance>();
            if (item != null && distanceFromBody <= 2f)
            {
                return item;
            }
        }
        return null;
    }

    public LivingEntity GetLivingEntityTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, 50f))
        {
            float distanceFromBody = Vector3.Distance(bodyCenter.position, hit.collider.transform.position);
            LivingEntity target = hit.collider.GetComponent<LivingEntity>();
            if (target && distanceFromBody <= 3f)
            {
                return target;
            }
        }
        return null;
    }

    private void SetHighlight()
    {
        ItemInstance item = GetCurrentTarget();
        if (highlightedItem)
        {
            if (!item)
            {
                highlightedItem.HideOutLine();
                highlightedItem = null;
            }
            else if (item && item != highlightedItem)
            {
                highlightedItem.HideOutLine();
                highlightedItem = item;
            }
        }

        if (item)
        {
            item.ShowOutLine();
            highlightedItem = item;
        }
    }
}

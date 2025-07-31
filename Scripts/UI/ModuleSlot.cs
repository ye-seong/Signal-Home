using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModuleSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Sprite slotImage;
    [SerializeField] private Image itemIcon;

    public ArmorModuleType moduleType;
    private ItemInstance currentArmor;

    private GameObject player;
    private PlayerState playerState;
    private Inventory inventory;
    private UIManager uiManager;
    private PlayerItemHandler playerItemHandler;

    private void Start()
    {
        player = GameObject.Find("Player");
        if (player)
        {
            playerState = player.GetComponent<PlayerState>();
            inventory = player.GetComponent<Inventory>();
            playerItemHandler = player.GetComponent<PlayerItemHandler>();
        }
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void OnEnable()
    {
        ArmorModuleManager.OnModuleChanged += OnModuleUpdated;
    }

    void OnDisable()
    {
        ArmorModuleManager.OnModuleChanged -= OnModuleUpdated;
    }

    private void OnModuleUpdated(int index, ItemInstance module, ItemInstance armor)
    {
        if (index == slotIndex)
        {
            SetModuleSlots(module, armor);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            RemoveItem(moduleType);
        }
    }

    public void SetModuleSlots(ItemInstance module, ItemInstance armor)
    {
        currentArmor = armor;
        if (module)
        {
            itemIcon.sprite = module.itemData.icon;
            itemIcon.color = Color.white;
        }
        else
        {
            itemIcon.sprite = slotImage;
            itemIcon.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void RemoveItem(ArmorModuleType type)
    {

        int index = -1;
        if (type == ArmorModuleType.Defense) index = 0;
        else if (type == ArmorModuleType.Resistance) index = 1;
        else if (type == ArmorModuleType.Skill) index = 2;
        else return;

        ItemInstance[] armorModules = currentArmor.Get<ItemInstance[]>("armorModules");
        if (!armorModules[index]) return;

        inventory.AddItem(armorModules[index]);
        armorModules[index] = null;

        currentArmor.Set("armorModules", armorModules);
        SetModuleSlots(null, currentArmor);

        uiManager.UpdateItemUI();
    }
}

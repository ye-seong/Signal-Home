using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite slotImage;

    public EquipmentType equipmentType;
    public Image itemIcon;

    private GameObject player;
    private PlayerState playerState;
    private Inventory inventory;
    private UIManager uiManager;
    private PlayerItemHandler playerItemHandler;
    private PlayerController playerController;

    private void Awake()
    {
        player = GameObject.Find("Player");
        if (player)
        {
            playerState = player.GetComponent<PlayerState>();
            inventory = player.GetComponent<Inventory>();
            playerItemHandler = player.GetComponent<PlayerItemHandler>();
            playerController = player.GetComponent<PlayerController>();
        }
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            RemoveItem(equipmentType);
        }
    }

    public void SetEquipmentSlot(ItemInstance item)
    {
        if (item)
        {
            itemIcon.sprite = item.itemData.icon;
            itemIcon.color = Color.white;
        }
        else
        {
            itemIcon.sprite = slotImage;
            itemIcon.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void RemoveItem(EquipmentType type)
    {
        int index = -1;
        if (type == EquipmentType.Face) index = 0;
        else if (type == EquipmentType.Body) index = 1;
        else if (type == EquipmentType.Back) index = 2;
        else return;

        if (!inventory.AddItem(playerState.equipmentItems[index]) || !playerItemHandler) return;

        
        if (index == 2)
        {
            playerItemHandler.ClearBackSocketHolding();
        }

        playerState.equipmentItems[index] = null;
        SetEquipmentSlot(null);

        if (index == 1)
        {
            playerState.ResetModuleEffect();
        }

        uiManager.UpdateItemUI();
    }

}

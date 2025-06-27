using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private GameObject UIManager;
    [SerializeField] private Image itemIcon;
    [SerializeField] private RectTransform backgroundRect;

    [Header("etc")]
    [SerializeField] private GameObject player;
    [SerializeField] private int slotIndex;

    private UIManager uiManager;
    private Inventory inventory;
    private GameObject dragObject;
    private Canvas canvas;

    private void Start()
    {
        if (UIManager)
        {
            uiManager = UIManager.GetComponent<UIManager>();
            inventory = player.GetComponent<Inventory>();
        }
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!inventory.items[slotIndex]) return;

        if (eventData.clickCount == 2)
        {
            uiManager.OnInventorySlotDoubleClick(slotIndex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if (!inventory.items[slotIndex]) return;

        if (!itemIcon) return;

        itemIcon.color = new Color(1, 1, 1, 0.5f);

        dragObject = new GameObject("DragIcon");
        dragObject.transform.SetParent(canvas.transform, false);

        inventory.draggedObjectIndex = this.slotIndex;

        Image dragImage = dragObject.AddComponent<Image>();
        dragImage.sprite = itemIcon.sprite;
        dragImage.color = new Color(1, 1, 1, 0.8f);
        dragImage.raycastTarget = false;

        RectTransform dragRect = dragObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = itemIcon.rectTransform.sizeDelta;
        dragRect.position = itemIcon.transform.position;

        //dragObject.transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData) 
    {
        if (!inventory.items[slotIndex]) return;

        if (!dragObject) return;
        dragObject.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (!inventory.items[slotIndex]) return;

        float mouseX = eventData.position.x;
        float mouseY = eventData.position.y;

        if (!backgroundRect) return;

        float backgroundWidth = backgroundRect.sizeDelta.x;
        float backgroundHeight = backgroundRect.sizeDelta.y;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mouseX < (screenWidth - backgroundWidth) / 2 || mouseX > (screenWidth + backgroundWidth) / 2 ||
            mouseY < (screenHeight - backgroundHeight) / 2 || mouseY > (screenHeight + backgroundHeight) / 2)
        {
            if (!inventory || !uiManager) return;
            inventory.ThrowItem(slotIndex);
            inventory.RemoveItem(slotIndex);
            uiManager.UpdateItemUI();
            CleanupDragIcon(true);
        }
        else
        {
            CleanupDragIcon(false);
            inventory.SwapInventoryItem(inventory.hoverIndex, inventory.draggedObjectIndex);
            inventory.isHovering = true;
        }
    }

    private void CleanupDragIcon(bool destroy)
    {
        if (dragObject)
        {
            Destroy(dragObject);
            dragObject = null;
            if (destroy)
            {
                itemIcon.color = new Color(1, 1, 1, 0f);
            }
            else
            {
                itemIcon.color = new Color(1, 1, 1, 1f);
            }
        }
    }


    // QuickSlot ·ÎÁ÷

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.SetHoverIndex(slotIndex);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventory.SetHoverIndex(-1);
    }
}

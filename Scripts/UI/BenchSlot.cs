using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BenchSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image itemIcon;

    private GameObject UIManager;
    private UIManager uiManager;
    public int slotIndex;

    private void Start()
    {
        UIManager = GameObject.Find("UIManager");
        if (UIManager)
        {
            uiManager = UIManager.GetComponent<UIManager>();
        }
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            uiManager.OnBenchSlotClick(slotIndex);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;


public class RecipeTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    public GameObject recipeSlotPrefab;

    private GameObject player;
    private PlayerState playerState;
    private BenchSlot benchSlot;
    private GameObject Tooltip;
    private UIManager uiManager;
    private GameManager gameManager;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerState = player.GetComponent<PlayerState>();
        }
        benchSlot = GetComponent<BenchSlot>();
        Tooltip = GameObject.Find("Tooltip");
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playerState || !benchSlot || !gameManager) return;

        if (!Tooltip) return;
        int slotIndex = benchSlot.slotIndex;

        if (!gameManager.allItems[slotIndex]) return;
        ItemData recipeData = gameManager.allItems[slotIndex];

        SetRecipeSlots(recipeData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (!Tooltip) return;
        HideTooltip();
    }

    private void SetRecipeSlots(ItemData recipeData)
    {
        var requireGroups = recipeData.ingredients.GroupBy(item => item.itemName);

        ShowTooltip();

        foreach (var group in requireGroups)
        {
            GameObject recipeSlot = Instantiate(recipeSlotPrefab, Tooltip.transform);
            string count = $"x{group.Count()}";
            
            recipeSlot.GetComponentInChildren<Image>().sprite = group.First().icon;
            recipeSlot.GetComponentInChildren<Image>().color = Color.white;
            recipeSlot.GetComponentInChildren<TextMeshProUGUI>().text = count;
        }
    }

    public void ShowTooltip()
    {
        Tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        uiManager.ClearTooltip(Tooltip);
    }
}
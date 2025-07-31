using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;
    public RectTransform tooltipRect;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerItemHandler : MonoBehaviour
{
    [HideInInspector] public ItemInstance currentItem;

    [Header("Hand Bones")]
    public Transform leftHandBone;
    public Transform rightHandBone;

    void Start()
    {
        
    }

    private void Update()
    {
        
    }
    public void SetHolding(ItemInstance item)
    {
        if (!item || item == currentItem) return;
        currentItem = item;
        SetActiveCurrentItem();
        switch (item.itemData.itemType)
        {
            case ItemType.Product:
                break;
            case ItemType.Weapon:
                break;
            default:
                return;
        }
    }

    public void ClearHolding()
    {
        if (!currentItem) return;
        currentItem.gameObject.SetActive(false);
        currentItem = null;
    }

    private void SetActiveCurrentItem()
    {
        if (!currentItem) return;
        currentItem.transform.position = rightHandBone.position;
        currentItem.transform.rotation = rightHandBone.rotation;

        currentItem.transform.SetParent(rightHandBone);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider[] colliders = currentItem.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        currentItem.gameObject.SetActive(true);
    }

    public void UseItem()
    {
        if (!currentItem) return;

        UseItem useItem = GetComponent<UseItem>();
        if (useItem)
        {
            useItem.UseItems(currentItem);
        }
    }
}

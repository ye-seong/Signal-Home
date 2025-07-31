using UnityEngine;

public class PlayerItemHandler : MonoBehaviour
{
    [HideInInspector] public ItemInstance currentItem;
    [HideInInspector] public ItemInstance backSocketItem;

    [Header("Hand Bones")]
    public Transform leftHandBone;
    public Transform rightHandBone;

    [Header("Socket")]
    public Transform backSocket;

    private Animator animator;
    private PlayerState playerState;
    private UseItem useItem;
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerState = GetComponent<PlayerState>();
        useItem = GetComponent<UseItem>();
    }

    public void SetHolding(ItemInstance item)
    {
        if (!item || item == currentItem) return;

        currentItem = item;
        SetHoldingItem(currentItem, rightHandBone);
        switch (item.itemData.itemType)
        {
            case ItemType.Product:
                break;
            case ItemType.Weapon:
                useItem.maxAttackCooldown = item.itemData.attackSpeed;
                break;
            default:
                return;
        }
    }

    public void SetBackSocket(ItemInstance item)
    {
        if (!item || item == backSocketItem) return;
        backSocketItem = item;
        SetHoldingItem(backSocketItem, backSocket);
        if (item == currentItem)
        {
            currentItem = null;
        }
    }
    public void ClearHolding()
    {
        if (!currentItem) return;
        currentItem.gameObject.SetActive(false);
        currentItem = null;
        animator.SetBool("Holding", false);
    }

    public void ClearBackSocketHolding()
    {
        if (!backSocketItem) return;
        backSocketItem.gameObject.SetActive(false);
        backSocketItem = null;
    }

    public void SetHoldingItem(ItemInstance item, Transform transform)
    {
        if (!item) return;
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        animator.SetBool("Holding", true);

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        Collider[] colliders = item.itemCollider.GetComponents<Collider>();
        //Debug.Log("Colliders Count: " + colliders.Length);
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        item.gameObject.SetActive(true);
    }
    public void UseItem()
    {
        if (!currentItem) return;

        if (useItem)
        {
            if (currentItem.itemData.itemType == ItemType.Weapon)
            {
                SetMaxAttackCooldown();
            }
            useItem.UseItems(currentItem);
        }
    }

    public void SetMaxAttackCooldown()
    {
        ItemInstance armor = playerState.equipmentItems[1];
        if (armor)
        {
            ItemInstance attackSpeedModule = armor.Get<ItemInstance[]>("armorModules")[2];
            if (attackSpeedModule)
            {
                ArmorSkill armorSkill = attackSpeedModule.GetComponent<ArmorSkill>();
                useItem.maxAttackCooldown = currentItem.itemData.attackSpeed * (1 - armorSkill.attackSpeed);
            }
            else
            {
                useItem.maxAttackCooldown = currentItem.itemData.attackSpeed;
            }
        }
    }

    public void ResetWeapon()
    {
        useItem.maxAttackCooldown = currentItem.itemData.attackSpeed;
    }
}

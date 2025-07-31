using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    protected float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void DropItems(ItemData[] itemdatas)
    {
        ItemInstance instance;
        foreach (ItemData item in itemdatas)
        {
            Vector3 dropPosition = transform.position + transform.forward * 0.5f + Vector3.up * 0.1f;
            instance = ItemInstance.Create(item, null);
            instance.transform.position = dropPosition;
            instance.transform.rotation = Quaternion.identity;
        }
    }
    protected abstract void Die();
     
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => currentHealth <= 0;
}

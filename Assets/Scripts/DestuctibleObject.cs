using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestuctibleObject : LivingEntity
{
    public DestuctibleData destuctibleData;
    protected override void Start()
    {
        maxHealth = destuctibleData.maxHealth;
        base.Start();
    }
    protected override void Die()
    {
        Debug.Log($"{destuctibleData.destuctibleName}ÀÌ ÆÄ±«µÆ´Ù!");
        Destroy(gameObject);
    }
}

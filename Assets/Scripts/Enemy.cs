using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    public EnemyData enemyData;
    protected override void Start()
    {
        maxHealth = enemyData.maxHealth;
        base.Start();
    }
    protected override void Die()
    {
        Debug.Log("적이 죽었다!");
        DropItems(enemyData.dropItems);
        Destroy(gameObject);
    }
}

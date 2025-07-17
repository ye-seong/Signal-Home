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
        Debug.Log("���� �׾���!");
        DropItems(enemyData.dropItems);
        Destroy(gameObject);
    }
}

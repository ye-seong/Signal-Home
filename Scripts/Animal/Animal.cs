using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : LivingEntity
{
    public AnimalData animalData;
    protected override void Start()
    {
        maxHealth = animalData.maxHealth;
        base.Start();
    }
    protected override void Die()
    {
        Debug.Log("������ �׾���!");
        Destroy(gameObject);
    }
}

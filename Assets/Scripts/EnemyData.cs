using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Living Entity Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Information")]
    public int enemyID = -1;
    public string enemyName;
    public string description;
    public GameObject enemyPrefab;

    [Header("Enemy State")]
    public float maxHealth = 100f;
    public float damage = 10f;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public float detectionRange = 5f;
    public float attackCooldown = 1f;

    [Header("Drop Item")]
    public ItemData[] dropItems;
}

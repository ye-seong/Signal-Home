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
    public EnvironmentType environmentType;

    [Header("Enemy State")]
    public float maxHealth = 100f;      // 최대 체력
    public float damage = 10f;          // 데미지
    public float attackRange;    // 공격범위
    public float detectionRange = 5f;   // 탐지범위
    public float attackCooldown = 1f;   // 공격 쿨타임
    public float fieldOfViewAngle = 120f; // 시야 각도
    public float moveSpeed = 3.5f;      // 이동 속도
    public float rotationSpeed = 5f;    // 회전 속도    

    [Header("Drop Item")]
    public ItemData[] dropItems;
}

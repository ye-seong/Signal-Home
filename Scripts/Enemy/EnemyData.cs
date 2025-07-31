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
    public float maxHealth = 100f;      // �ִ� ü��
    public float damage = 10f;          // ������
    public float attackRange;    // ���ݹ���
    public float detectionRange = 5f;   // Ž������
    public float attackCooldown = 1f;   // ���� ��Ÿ��
    public float fieldOfViewAngle = 120f; // �þ� ����
    public float moveSpeed = 3.5f;      // �̵� �ӵ�
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�    

    [Header("Drop Item")]
    public ItemData[] dropItems;
}

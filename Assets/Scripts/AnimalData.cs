using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animal", menuName = "Living Entity Data/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("Animal Information")]
    public int animalID = -1;
    public string animalName;
    public string description;
    public GameObject animalPrefab;

    [Header("Animal State")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;

    [Header("Drop Item")]
    public ItemData[] dropItems;
}



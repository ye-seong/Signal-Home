using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destuctible Object", menuName = "Living Entity Data/Destuctible Data")]
public class DestuctibleData : ScriptableObject
{
    [Header("Destuctible Information")]
    public int destuctibleID = -1;
    public string destuctibleName;
    public string description;
    public GameObject destuctiblePrefab;

    [Header("Destuctible State")]
    public float maxHealth = 100f;

    [Header("Drop Item")]
    public ItemData[] dropItems;
}

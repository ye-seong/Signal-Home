using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityRandom = UnityEngine.Random;

public class LivingSpawner : MonoBehaviour
{
    [Header("Spawner Zone")]
    public ZoneManager zoneManager;

    [Header("Spawn Settings")]
    private float spawnRadius = 10f;
           
    public void SpawnEnemyRandomPosition(Enemy enemy, Vector3 centerPoint)
    {
        if (!zoneManager || !enemy) return;

        Vector3 randomDirection = UnityRandom.insideUnitSphere * spawnRadius;
        randomDirection += centerPoint;
        enemy.centerPoint = centerPoint;
        NavMeshHit hit;

        if(NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
        {
            enemy.transform.position = hit.position;
        }
    }
}



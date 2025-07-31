using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public GameObject player;

    [Header("Scripts")]
    public LivingSpawner livingSpawner;

    [Header("Forest Zone")]
    public GameObject[] forestZones;

    [Header("Desert Zone")]
    public GameObject[] desertZones;

    [Header("Cave Zone")]
    public GameObject[] caveZones;

    [Header("Beach Zone")]
    public GameObject[] beachZones;

    [Header("Sky Zone")]
    public GameObject[] skyZones;


    public void SpawnEnemyByZone(EnemyData[] datas)
    {
        if (datas.Length <= 0 || !livingSpawner) return;

        int count = 0;
        Vector3 point = Vector3.zero;

        foreach (EnemyData data in datas)
        {
            if (data.environmentType == EnvironmentType.Forest)
            {
                foreach (GameObject zone in forestZones)
                {
                    count = zone.GetComponent<ZoneController>().SpawnCount(EnvironmentType.Forest);
                    for (int i = 0; i < count; i++)
                    {
                        //Debug.Log("Forest 俊辑 积己!");
                        Enemy enemy = Enemy.Create(data);
                        point = zone.GetComponent<Transform>().position;
                        livingSpawner.SpawnEnemyRandomPosition(enemy, point);
                    }
                }
            }
            else if (data.environmentType == EnvironmentType.Desert)
            {
                foreach (GameObject zone in desertZones)
                {
                    count = zone.GetComponent<ZoneController>().SpawnCount(EnvironmentType.Desert);
                    for (int i = 0; i < count; i++)
                    {
                        //Debug.Log("Desert 俊辑 积己!");
                        Enemy enemy = Enemy.Create(data);
                        point = zone.GetComponent<Transform>().position;
                        livingSpawner.SpawnEnemyRandomPosition(enemy, point);
                    }
                }
            }
            else if (data.environmentType == EnvironmentType.Cave)
            {
                foreach (GameObject zone in caveZones)
                {
                    count = zone.GetComponent<ZoneController>().SpawnCount(EnvironmentType.Cave);
                    for (int i = 0; i < count; i++)
                    {
                        Enemy enemy = Enemy.Create(data);
                        point = zone.GetComponent<Transform>().position;
                        livingSpawner.SpawnEnemyRandomPosition(enemy, point);
                    }
                }
            }
            else if (data.environmentType == EnvironmentType.Beach)
            {
                foreach (GameObject zone in beachZones)
                {
                    count = zone.GetComponent<ZoneController>().SpawnCount(EnvironmentType.Beach);
                    for (int i = 0; i < count; i++)
                    {
                        Enemy enemy = Enemy.Create(data);
                        point = zone.GetComponent<Transform>().position;
                        livingSpawner.SpawnEnemyRandomPosition(enemy, point);
                    }
                }
            }

            else if (data.environmentType == EnvironmentType.Sky)
            {
                foreach (GameObject zone in skyZones)
                {
                    count = zone.GetComponent<ZoneController>().SpawnCount(EnvironmentType.Sky);
                    for (int i = 0; i < count; i++)
                    {
                        Enemy enemy = Enemy.Create(data);
                        point = zone.GetComponent<Transform>().position;
                        livingSpawner.SpawnEnemyRandomPosition(enemy, point);
                    }
                }
            }
        }
    }

    public bool IsInHotZone()
    {
        //foreach (GameObject zone in desertZones)
        //{
        //    if (zone.GetComponent<ZoneController>().IsInZone())
        //    {
        //        return true;
        //    }
        //}
        return false;
    }
}

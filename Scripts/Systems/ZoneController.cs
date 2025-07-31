using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public EnvironmentType environmentType;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private int minEnemies;
    [SerializeField] private int maxEnemies;


    public int SpawnCount(EnvironmentType type)
    {
        if (environmentType != type) return 0;

        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        return enemyCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerState playerState = other.GetComponent<PlayerState>();
            switch(environmentType)
            {
                case EnvironmentType.Desert:
                    playerState.EnterInHotZone();
                    Debug.Log("Player�� Desert Zone�� ���Խ��ϴ�.");
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerState playerState = other.GetComponent<PlayerState>();
            switch (environmentType)
            {
                case EnvironmentType.Desert:
                    playerState.ExitFromHotZone();
                    Debug.Log("Player�� Desert Zone�� �������ϴ�.");
                    break;
                default:
                    break;
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private Enemy enemy;
    private PlayerState playerState;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        playerState = GameObject.Find("Player").GetComponent<PlayerState>();
        GetComponent<Collider>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemy)
            {
                playerState.ModifyHealth(-enemy.enemyData.damage);
            }
        }
    }
}

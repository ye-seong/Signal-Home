using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    // 총알 유지 시간
    public float lifeTime;

    [HideInInspector] public float damage = 10f;
    [HideInInspector] public bool startFollow = false;
    [HideInInspector] public Enemy enemy;

    private float currentTime = 0f;
    private void Update()
    {
        if (startFollow)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= lifeTime)
            {
                Destroy(gameObject);
                return;
            }
            FollowTarget();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerState playerState = other.gameObject.GetComponent<PlayerState>();
            if (playerState != null)
            {
                playerState.ModifyHealth(-damage);
            }
            Destroy(gameObject); 
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    public void FollowTarget()
    {
        Vector3 direction = (enemy.target.transform.position - transform.position).normalized;

        if (direction != Vector3.zero && enemy)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemy.projectileSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, enemy.target.transform.position + Vector3.up * 1.5f, enemy.projectileSpeed * Time.deltaTime);
        }
    }
}

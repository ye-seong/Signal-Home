using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAttackSystem : MonoBehaviour
{
    private Enemy enemy;

    private GameObject bomberMissile;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    // 적이 공격을 수행하는 로직
    public void PerformAttackByName(string name)
    {
        switch(name)
        {
            case "Brute":
                enemy.attackPoint.enabled = true;
                break;
            case "Spitter":
                ThrowProjectile(enemy.attackTransform);
                break;
            case "Bomber":
                if (!bomberMissile)
                {
                    ThrowGuidedMissile(enemy.attackTransform);
                }
                break;
            case "Stalker":
                
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (enemy.currentState.CurrentStateType == StateType.Attack)
        {
            LookAtPlayer();
        }
    }

    private void ThrowProjectile(Transform attackTransform)
    {
        GameObject projectile = Instantiate(enemy.projectilePrefab, attackTransform.position, attackTransform.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        ProjectileManager projectileManager = projectile.GetComponent<ProjectileManager>();
        if (rb && projectileManager)
        {
            projectileManager.damage = enemy.enemyData.damage;
            rb.AddForce(attackTransform.forward * enemy.projectileSpeed, ForceMode.VelocityChange);
        }
    }

    private void ThrowGuidedMissile(Transform attackTransform)
    {
        Debug.Log("폭탄 투하!!!");

        bomberMissile = Instantiate(enemy.projectilePrefab, attackTransform.position, attackTransform.rotation);
        ProjectileManager projectileManager = bomberMissile.GetComponent<ProjectileManager>();
        if (projectileManager)
        {
            projectileManager.damage = enemy.enemyData.damage;
            projectileManager.startFollow = true;
            projectileManager.enemy = enemy; 
        }
    }

    public void LookAtPlayer()
    {
        Vector3 direction = (enemy.target.transform.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        switch(enemy.aiType)
        {
            case AIType.Ground:
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, enemy.agent.angularSpeed * Time.deltaTime);
                break;
            case AIType.Flying:
                transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, enemy.enemyData.rotationSpeed * Time.deltaTime);
                break;
            case AIType.Swimming:
                break;
            default:
                break;
        }
    }
}

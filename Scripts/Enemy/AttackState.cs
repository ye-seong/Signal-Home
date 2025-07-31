using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackState : EnemyState
{
    public override StateType CurrentStateType => StateType.Attack;

    private float currentCooldown;
    public AttackState(Enemy enemy) : base(enemy) { }
    public override void Enter()
    {
        //Debug.Log("적이 공격 상태로 진입");
        switch(enemy.aiType)
        {
            case AIType.Ground:
                enemy.agent.isStopped = true;
                break;
            case AIType.Flying:
                break;
            case AIType.Swimming:
                break;
            default:
                break;
        }
        
        currentCooldown = enemy.enemyData.attackCooldown;
    }

    public override void Update()
    {
        if (enemy.playerState.isInvisible)
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
        if (CanAttack())
        {
            enemy.PerformAttack();
        }
        if (!enemy.IsInAttackRange(enemy.target.transform))
        {
            enemy.ChangeState(new ChaseState(enemy));
        }
    }

    public override void Exit()
    {
        //Debug.Log("적이 공격 행동을 벗어남");
        switch (enemy.aiType)
        {
            case AIType.Ground:
                enemy.agent.isStopped = false;
                break;
            case AIType.Flying:

                break;
            case AIType.Swimming:

                break;
            default:
                break;
        }
    }

    private bool CanAttack()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            currentCooldown = enemy.enemyData.attackCooldown;
            return true;
        }
        return false;
    }
    
}

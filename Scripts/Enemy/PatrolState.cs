using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Random;

public class PatrolState : EnemyState
{
    public override StateType CurrentStateType => StateType.Patrol;

    public PatrolState(Enemy enemy) : base(enemy) { }
    public override void Enter()
    {
        //Debug.Log("적이 순찰 상태로 진입");
        enemy.PerformPatrol();
    }

    public override void Update()
    {
        if (enemy.StartChase() && enemy.IsInCenter(enemy.target.transform) && enemy.IsInCenter(enemy.transform))
        {
            enemy.ChangeState(new ChaseState(enemy));
        }
        else if (enemy.StartChase() && enemy.IsInDistance(enemy.target.transform))
        {
            enemy.ChangeState(new AttackState(enemy));
        }
    }

    public override void Exit()
    {
        //Debug.Log("적이 순찰 행동을 벗어남");
        enemy.StopCoroutine(enemy.patrolCoroutine);
    }

}

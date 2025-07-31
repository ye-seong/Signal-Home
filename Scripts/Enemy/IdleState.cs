using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : EnemyState
{
    public override StateType CurrentStateType => StateType.Idle;

    public IdleState(Enemy enemy) : base(enemy) { }
    public override void Enter()
    {
        //Debug.Log("적이 대기 상태로 진입");
        enemy.agent.isStopped = true;
    }

    public override void Update()
    {
        if (enemy.StartChase())
        {
            enemy.ChangeState(new ChaseState(enemy));
        }
    }

    public override void Exit()
    {
        //Debug.Log("적이 대기 행동을 벗어남");
        enemy.agent.isStopped = false;
    }
}

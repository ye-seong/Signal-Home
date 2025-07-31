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
        //Debug.Log("���� ��� ���·� ����");
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
        //Debug.Log("���� ��� �ൿ�� ���");
        enemy.agent.isStopped = false;
    }
}

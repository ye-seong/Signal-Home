using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class ChaseState : EnemyState
{
    public override StateType CurrentStateType => StateType.Chase;

    public ChaseState(Enemy enemy) : base(enemy) { }
    public override void Enter()
    {
        //Debug.Log("���� ���� ���·� ����");
    }

    public override void Update()
    {
        
        if (!enemy.IsInDistance(enemy.target.transform))
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
        else if (enemy.IsInAttackRange(enemy.target.transform))
        {
            enemy.ChangeState(new AttackState(enemy));
        }
        Moving();
    }

    public override void Exit()
    {
        //Debug.Log("���� ���� �ൿ�� ���");
    }

    private void Moving()
    {
        if (enemy.IsInCenter(enemy.transform))
        {
            switch(enemy.aiType)
            {
                case AIType.Ground:
                    enemy.agent.destination = enemy.target.transform.position;
                    break;
                case AIType.Flying:
                    FlyingTypeMoving();
                    break;
                case AIType.Swimming:
                    break;
                default:
                    break;
            }
        }
    }

    private void FlyingTypeMoving()
    {
        Vector3 direction = (enemy.target.transform.position - enemy.transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemy.enemyData.rotationSpeed * Time.deltaTime);
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.target.transform.position, enemy.enemyData.moveSpeed * Time.deltaTime);
        }
    }
}

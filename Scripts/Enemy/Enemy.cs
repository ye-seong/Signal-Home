using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityRandom = UnityEngine.Random;

public class Enemy : LivingEntity
{
    [Header("Default Settings")]
    public EnemyData enemyData;
    public Collider attackPoint;
    public Transform attackTransform;

    [Header("AI Settings")]
    public NavMeshAgent agent;
    public AIType aiType;

    [Header("Projectile Settings")] 
    public GameObject projectilePrefab;
    public float projectileSpeed;

    [Header("Patrol Settings")]
    public float patrolRadius;

    [HideInInspector] public EnemyState currentState;
    [HideInInspector] public GameObject target;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Coroutine patrolCoroutine;
    [HideInInspector] public Vector3 centerPoint;

    private EnemyAttackSystem attackSystem;
    [HideInInspector] public PlayerState playerState;

    protected override void Start()
    {
        target = GameObject.Find("Player");

        rb = GetComponent<Rigidbody>();
        rb.mass = 10f;
        rb.drag = 0f;
        rb.angularDrag = 20f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        maxHealth = enemyData.maxHealth;
        centerPoint = transform.position;
        agent = GetComponent<NavMeshAgent>();
        ChangeState(new PatrolState(this));

        attackSystem = GetComponent<EnemyAttackSystem>();
        playerState = target.GetComponent<PlayerState>();

        base.Start();
    }
    private void Update()
    {
        switch(currentState.CurrentStateType)
        {
            case StateType.Idle:
                //DoIdle();
                break;
            case StateType.Patrol:
                //DoPatrol();
                break;
            case StateType.Chase:
                //DoChase();
                break;
            case StateType.Attack:
                //DoAttack();
                break;
            case StateType.Dead:
                break;
            default:
                break;
        }
        currentState.Update();
    }

    private void Awake()
    {
        if (!attackPoint)
        {
            attackPoint = GetComponentInChildren<Collider>();
            if (attackPoint)
            {
                attackPoint.enabled = false;
            }
        }
    }
    public static Enemy Create(EnemyData data)
    {
        GameObject obj = Instantiate(data.enemyPrefab);
        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.enemyData = data;
        return enemy;
    }
    protected override void Die()
    {
        Debug.Log("적이 죽었다!");
        DropItems(enemyData.dropItems);
        Destroy(gameObject);
    }
    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private bool CanSeeAngle(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        return angleToTarget <= enemyData.fieldOfViewAngle / 2f;
    }

    private bool IsObstacleBetween(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position); 

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance))
        {
            return hit.transform != target;
        }
        return false;
    }

    public bool IsInDistance(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= enemyData.detectionRange; 
    }

    public bool IsInAttackRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= enemyData.attackRange;
    }

    public bool IsInCenter(Transform target)
    {
        return Vector3.Distance(centerPoint, target.position) <= patrolRadius;
    }
    public void PerformAttack()
    {
        if (!attackSystem) return;
        StartCoroutine(AttackWithCooldown());
    }
    IEnumerator AttackWithCooldown()
    {
        //Debug.Log("공격합니다~");
        if (attackSystem)
        {
            attackSystem.PerformAttackByName(enemyData.enemyName);
        }
        yield return new WaitForSeconds(enemyData.attackCooldown);
    }

    public void PerformPatrol()
    {
        switch(aiType)
        {
            case AIType.Ground:
                patrolCoroutine = StartCoroutine(WaitForPatrolCoroutine());
                break;
            case AIType.Flying:
                patrolCoroutine = StartCoroutine(WaitForPatrolCoroutineFlying());
                break;
            case AIType.Swimming:
                break;
            default:
                break;
        }
    }

    IEnumerator WaitForPatrolCoroutine()
    {
        while (true)
        {
            SetNewRandomDestination();

            while (agent.pathPending || agent.remainingDistance > 0.5f)
                yield return null;

            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator WaitForPatrolCoroutineFlying()
    {
        while (true)
        {
            Vector3 targetPosition = SetNewRandomPosition();

            while (Vector3.Distance(transform.position, targetPosition) > 0.5f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0; 
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyData.rotationSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyData.moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);
        }
    }
    void SetNewRandomDestination()
    {
        Vector3 randomDirection = UnityRandom.insideUnitSphere * patrolRadius;
        randomDirection += centerPoint;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private Vector3 SetNewRandomPosition()
    {
        Vector3 randomDirection = UnityRandom.insideUnitSphere * patrolRadius;
        randomDirection += centerPoint;
        return randomDirection;
    }
    public bool StartChase()
    {
        if (playerState)
        {
            if (playerState.isInvisible)
            {
                return false;
            }
        }
        if (CanSeeAngle(target.transform) && !IsObstacleBetween(target.transform)
            && IsInDistance(target.transform)) return true;
        return false;
    }
}

public enum AIType
{
    None,
    Ground,
    Flying,
    Swimming
}
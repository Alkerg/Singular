using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum BomberEnemyState
{
    Idle,
    Attacking,
    Relocating,
    Dead
}

public class BomberEnemy : EnemyBase
{
    public Transform target;
    private NavMeshAgent agent;
    public BomberEnemyState currentState = BomberEnemyState.Idle;
    public float visionRange = 15f;
    public float attackRange = 10f;
    public float attackRate = 2f;
    public float damage = 30f;

    public float relocationInterval = 6f;
    public float relocationDistance = 6f;
    public int maxPositionAttempts = 10;
    private float lastAttackTime;
    private float lastRelocationTime;
    private Coroutine relocationCoroutine;
    public Animator animator;

    public override void Start()
    {
        base.Start();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (target == null)
            return;

        if(_healthManager._currentHealth <= 0)
        {
            currentState = BomberEnemyState.Dead;
            return;
        }

        switch (currentState)
        {
            case BomberEnemyState.Idle:
                IdleState();
                break;

            case BomberEnemyState.Attacking:
                AttackingState();
                break;

            case BomberEnemyState.Relocating:
                RelocatingState();
                break;
            case BomberEnemyState.Dead:
                StopMoving();
                break;
        }
    }

    void IdleState()
    {
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 0f);

        if (IsPlayerInVisionRange())
        {
            currentState = BomberEnemyState.Attacking;
        }
    }

    void AttackingState()
    {
        if (!IsPlayerInVisionRange())
        {
            currentState = BomberEnemyState.Idle;
            return;
        }

        transform.LookAt(target);

        if (IsPlayerInAttackRange())
        {
            TryAttack();
        }

        if (Time.time - lastRelocationTime > relocationInterval)
        {
            lastRelocationTime = Time.time;
            currentState = BomberEnemyState.Relocating;
        }
    }

    void RelocatingState()
    {
        animator.SetFloat("MovX", 1f);
        animator.SetFloat("MovY", 0f);

        if (relocationCoroutine == null)
        {
            relocationCoroutine = StartCoroutine(RelocateRoutine());
        }
    }

    IEnumerator RelocateRoutine()
    {
        Vector3 newPosition = GetRandomPosition();

        agent.isStopped = false;
        agent.SetDestination(newPosition);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        relocationCoroutine = null;

        if (IsPlayerInVisionRange())
            currentState = BomberEnemyState.Attacking;
        else
            currentState = BomberEnemyState.Idle;
    }

    void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }
    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackRate)
            return;

        lastAttackTime = Time.time;
        Attack();
    }

    public override void Attack()
    {
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 1f);
        // TODO: Launch proyectile
        Debug.Log("Bomber launches projectile");

        target.GetComponent<HealthManager>()?.TakeDamage(damage);
    }

    private bool IsPlayerInVisionRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= visionRange;
    }

    private bool IsPlayerInAttackRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
    }

     Vector3 GetRandomPosition()
    {
        for (int i = 0; i < maxPositionAttempts; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere.normalized;
            Vector3 candidate = transform.position + randomDir * relocationDistance;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(hit.position, transform.position) > relocationDistance * 0.6f)
                {
                    return hit.position;
                }
            }
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

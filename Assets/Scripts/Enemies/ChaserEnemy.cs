using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum ChaserEnemyState
{
    Idle,
    IdleAttacking,
    ApproachingPlayer,
    ChangingPosition,
    Dead
}

public class ChaserEnemy : EnemyBase
{
    [SerializeField] private ChaserEnemyState currentState = ChaserEnemyState.Idle;
    private NavMeshAgent agent;
    private float _lastAttackTime;
    private Coroutine changePositionCoroutine;
    private int maxPositionAttempts = 5;
    private float minDistanceFromPlayer = 3f;
    private float maxDistanceFromPlayer = 5f;
    private int idleAttackCount = 0;
    private int maxIdleAttacks = 2;
    private int consecutiveHits = 0;
    private int maxConsecutiveHits = 2;

    [SerializeField] private float closeRange = 2f;
    public float visionRange = 8f;
    public float attackRange = 8f;
    public float attackRate = 2f;
    public float moveSpeed = 2f;
    public float damage = 20f;
    public Animator animator;
    public Transform target;


    public override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (target == null) return;

        if (!IsPlayerInVisionRange())
        {
            IdleState();
        }

        if(_healthManager._currentHealth <= 0)
        {
            currentState = ChaserEnemyState.Dead;
        }

        switch (currentState)
        {
            case ChaserEnemyState.Idle:
                IdleState();
                break;

            case ChaserEnemyState.IdleAttacking:
                IdleAttackingState();
                break;

            case ChaserEnemyState.ApproachingPlayer:
                ApproachingState();
                break;

            case ChaserEnemyState.ChangingPosition:
                TryChangePosition();
                break;
            case ChaserEnemyState.Dead:
                StopMoving();
                break;
        }

    }

    void IdleState()
    {
        StopMoving();
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 0f);
        if(IsPlayerInVisionRange())
        {
            currentState = ChaserEnemyState.ApproachingPlayer;
        }
    }

    void IdleAttackingState()
    {
        StopMoving();
        transform.LookAt(target);
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 0f);

        if (idleAttackCount < maxIdleAttacks)
        {
            if (TryAttack())
            {
                idleAttackCount++;
            }
        }
        else
        {
            idleAttackCount = 0;
            currentState = ChaserEnemyState.ApproachingPlayer;
        }
    }

    void ApproachingState()
    {
        transform.LookAt(target);
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 1f);

        agent.isStopped = false;
        agent.SetDestination(target.position);

        TryAttack();

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentState = ChaserEnemyState.IdleAttacking;
        }
    }

    bool TryAttack()
    {
        if (Time.time - _lastAttackTime < attackRate) 
            return false;

        _lastAttackTime = Time.time;
        Attack();

        return true;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        consecutiveHits++;

        if (consecutiveHits >= maxConsecutiveHits)
        {
            consecutiveHits = 0;
            currentState = ChaserEnemyState.ChangingPosition;
        }
    }

    void ApproachPlayer()
    {
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 1f);

        agent.isStopped = false;
        agent.SetDestination(target.position);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            SetDefaultState();
        }
    }

    void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    /* void TryAttack()
    {
        //animator.SetFloat("MovX", 0f);
        //animator.SetFloat("MovY", 0f);

        if (Time.time - _lastAttackTime < attackRate) return;

        _lastAttackTime = Time.time;
        Attack();
    } */

    public override void Attack()
    {
        //transform.LookAt(target);


        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<HealthManager>()?.TakeDamage(damage);
                Debug.Log("Enemy chase attack");
            }
        }
    }

    private bool IsPlayerInVisionRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= visionRange;
    }

    private bool IsPlayerInAttackRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    void TryChangePosition()
    {
        //Debug.Log(agent.velocity);
        animator.SetFloat("MovX", Mathf.Clamp(agent.velocity.x, -1f, 1f));
        animator.SetFloat("MovY", Mathf.Clamp(agent.velocity.z, -1f, 1f));
        
        if (changePositionCoroutine == null)
        {
            changePositionCoroutine = StartCoroutine(ChangePositionCoroutine());
        }
    }

    IEnumerator ChangePositionCoroutine()
    {
        Vector3 newPosition = GetRandomPositionAroundPlayer();

        agent.isStopped = false;
        agent.SetDestination(newPosition);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        changePositionCoroutine = null;

        idleAttackCount = 0;
        currentState = ChaserEnemyState.IdleAttacking;
    }

    void SetDefaultState()
    {
        animator.SetFloat("MovX", 0f);
        animator.SetFloat("MovY", 0f);
        currentState = ChaserEnemyState.IdleAttacking;
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        for (int i = 0; i < maxPositionAttempts; i++)
        {
            // Random direction around the player
            Vector3 randomDir = Random.insideUnitSphere.normalized;

            // Random distance between minimum and maximum
            float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

            // Candidate position
            Vector3 candidatePosition = target.position + randomDir * randomDistance;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(candidatePosition, out hit, visionRange, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
    
}

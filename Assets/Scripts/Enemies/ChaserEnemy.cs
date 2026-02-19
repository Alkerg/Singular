using UnityEngine;
using UnityEngine.AI;

public class ChaserEnemy : EnemyBase
{

    private NavMeshAgent agent;
    public Transform target;
    private float _lastAttackTime;
    public float visionRange = 15f;
    public float attackRange = 10f;
    public float attackRate = 2f;
    public float moveSpeed = 2f;
    public float damage = 20f;

    public override void Start()
    {
        base.Start();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (target == null) return;

        if (!IsPlayerInVisionRange())
        {
            StopMoving();
            return;
        }

        if (IsPlayerInAttackRange())
        {
            StopMoving();
            TryAttack();
            return;
        }

        ChasePlayer();
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    void TryAttack()
    {
        if (Time.time - _lastAttackTime < attackRate) return;

        _lastAttackTime = Time.time;
        Attack();
    }

    public override void Attack()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    
}

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BomberEnemy : EnemyBase
{
    public Transform target;
    public float attackRange = 10f;
    public float explosionRadius = 5f;
    public float attackRate = 2f;
    public float damage = 30f;

    private Coroutine _attackCoroutine;

    public override void Start()
    {
        base.Start();
        target = FindFirstObjectByType<Player>()?.transform;
    }

    void Update()
    {
        if (IsPlayerInAttackRange())
        {
            StartAttackRoutine();
        }else
        {
            StopAttackingRoutine();
        }
    }

    private void StartAttackRoutine()
    {
        if (_attackCoroutine == null)
        {
            _attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }


    private void StopAttackingRoutine()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(attackRate);
        }
    }

    public override void Attack()
    {
        // TODO: Launch proyectile and make damage in area
        target.GetComponent<HealthManager>()?.TakeDamage(damage);
    }

    private bool IsPlayerInAttackRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

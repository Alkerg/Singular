using System;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthManager _healthManager;
    public GameObject healthFragmentGroupPrefab;
    private Animator _animator;
    private CapsuleCollider _collider;
    public virtual void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        _healthManager = GetComponent<HealthManager>();
        _healthManager.OnPlayerDeath += Die;
    }


    public void OnDestroy()
    {
        _healthManager.OnPlayerDeath -= Die;
    }


    public virtual void Attack(){}

    public void Die()
    {
        var healthFragments = FragmentPool.Instance.Get(healthFragmentGroupPrefab);
        healthFragments.Activate(transform.position, transform.rotation, Vector3.one, Vector3.one, 0.3f, 0.3f, 0.2f, 0.2f, false);
        GameStatusManager.enemiesCount -= 1;
        _collider.enabled = false;
        _animator.SetBool("isDead",true);
    }

    public virtual void TakeDamage(float damage)
    {
        _healthManager.TakeDamage(damage);
    }

}

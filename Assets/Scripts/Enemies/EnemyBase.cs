using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthManager _healthManager;
    public GameObject healthFragmentGroupPrefab;
    public virtual void Start()
    {
        _healthManager = GetComponent<HealthManager>();
        _healthManager.OnPlayerDeath += Die;
    }


    private void OnDestroy()
    {
        _healthManager.OnPlayerDeath -= Die;
    }


    public virtual void Attack(){}

    private void Die()
    {
        var healthFragments = FragmentPool.Instance.Get(healthFragmentGroupPrefab);
        healthFragments.Activate(transform.position, transform.rotation, Vector3.one, Vector3.one, 0.3f, 0.3f, 0.2f, 0.2f, false);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        _healthManager.TakeDamage(damage);
    }

    /*public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GrabbableObject"))
        {
            TakeDamage(20f);
        } 
    } */

}

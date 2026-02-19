using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public HealthManager _healthManager;
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
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        _healthManager.TakeDamage(damage);
    }

/*     public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GrabbableObject"))
        {
            TakeDamage(20f);
        } 
    } */

}

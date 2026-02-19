using UnityEngine;

public interface IDestructible
{
    void Destroy();
    void Destroy(Vector3 hitPoint, Vector3 hitForce);
}

public class DestructibleObject : MonoBehaviour, IDestructible
{
    public GameObject fragmentedObjectPrefab; 
    private BoxCollider _boxCollider;
    [Header("Impact Settings")]
    [SerializeField] private float _damage = 20f;
    [SerializeField] private float _hitForceMutiplier = .1f;
    [SerializeField] private float _explosionForce = 1f;
    [SerializeField] private float _explosionRadius = 1f;
    [SerializeField] private float _upwardModifier = .5f;
    [SerializeField] private float _torque = 1f;
    private float _velocityThreshold = 30f;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(_damage);
            Destroy(collision.contacts[0].point, collision.relativeVelocity * _hitForceMutiplier);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Environment") && collision.relativeVelocity.magnitude > _velocityThreshold)
        {
            Destroy(collision.contacts[0].point, collision.relativeVelocity * _hitForceMutiplier);
        }
    }

    public void Destroy(Vector3 hitPoint, Vector3 hitForce)
    {
        _boxCollider.enabled = false;

        var fragments = FragmentPool.Instance.Get(fragmentedObjectPrefab);

        fragments.Activate(transform.position, transform.rotation, hitPoint, hitForce, _explosionForce, _explosionRadius, _upwardModifier, _torque);

        Destroy(gameObject);
    }

    public void Destroy()
    {
        _boxCollider.enabled = false;
        Instantiate(fragmentedObjectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

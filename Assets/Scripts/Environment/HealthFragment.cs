using UnityEngine;

public class HealthFragment : MonoBehaviour
{
    public float healAmount = 10f;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.Heal(healAmount);
                gameObject.SetActive(false);
            }
        }
    }
}

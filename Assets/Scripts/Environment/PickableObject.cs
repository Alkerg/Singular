using System.Collections;
using UnityEngine;

public interface IPickable
{
    void GoTowardsPlayer(Transform target);
    void ThrowToTarget(Vector3 targetPosition);
}

public class PickableObject : MonoBehaviour, IPickable
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GoTowardsPlayer(Transform target)
    {
        Debug.Log("Going towards player");
        StartCoroutine(MoveToPlayer(target));
    }

    public void ThrowToTarget(Vector3 targetPosition)
    {
        Debug.Log("Thrown to target");
    }

    IEnumerator MoveToPlayer(Transform target)
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, time);
            yield return null;
        }
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.transform.SetParent(target);   
    }
}

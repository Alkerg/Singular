using System.Collections;
using UnityEngine;
using System;

public class TelekinesisAbility : AbilityBase
{
    public float range = 20f;
    public Transform handTransform;
    public Rigidbody grabbedObject;
    public float throwForce = 300f;
    private Coroutine _moveToHandCoroutine;
    private Outline outline;

    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, range))
        {
            
            if (hit.collider.CompareTag("GrabbableObject"))
            {
                outline = hit.collider.GetComponent<Outline>();
                if (outline != null) outline.enabled = true;
            }
            else
            {
                if (outline != null) outline.enabled = false;
            }
        }
    }

    public void GrabObject()
    {
        // Launch raycast from camera
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, range))
            {
                // Check if hit object is grabbable            
                if (hit.collider.CompareTag("GrabbableObject"))
                {
                    // Get rigidbody of hit object and make it kinematic to disable physics interactions
                    grabbedObject = hit.collider.GetComponent<Rigidbody>();
                    grabbedObject.isKinematic = true;
                    // Move object to player's hand and consume stamina
                    _moveToHandCoroutine = StartCoroutine(MoveToPlayerHand(handTransform));
                    staminaManager.TakeStamina(staminaNeeded);
                }
            }
    }

    public void ThrowObject()
    {
        // Stop moving object to hand if still in progress
        StopCoroutine(_moveToHandCoroutine);
        _moveToHandCoroutine = null;

        // Detach object from hand
        grabbedObject.transform.SetParent(null);
        grabbedObject.isKinematic = false;
        
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 targetPoint;

        // Lauch raycast to determine throw direction, ignoring grabbable objects
        if (Physics.Raycast(ray, out RaycastHit hit, range, ~LayerMask.GetMask("GrabbableObject")))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * range;
        }

        // Calculate throw direction
        Vector3 throwDir = (targetPoint - handTransform.position).normalized;

        // Reset velocity before throwing the object
        grabbedObject.linearVelocity = Vector3.zero;
        grabbedObject.angularVelocity = Vector3.zero;

        // Apply throw force
        grabbedObject.AddForce(throwDir * throwForce, ForceMode.Impulse);

        // Clear reference to grabbed object
        grabbedObject = null;
    }

    public override void Activate()
    {
        if (!canUse) return;

        // Check if player is already grabbing an object and has enough stamina to grab a new one
        if(grabbedObject == null && staminaManager._currentStamina >= staminaNeeded)
        {
            GrabObject();
        }
        else if(grabbedObject != null)
        {
            ThrowObject();
        }
    }

    IEnumerator MoveToPlayerHand(Transform target)
    {
        float time = 0f;
        // Move object to hand over 1 second
        while (time < 1f)
        {
            time += Time.deltaTime;
            grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, target.position, time);
            yield return null;
        }
        grabbedObject.transform.SetParent(target);  
    }

    void OnDrawGizmos()
    {
        // Visualize raycast for teleknesis ability in editor
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range);       
    }
}

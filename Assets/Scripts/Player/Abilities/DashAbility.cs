using UnityEditor.Callbacks;
using UnityEngine;

public class DashAbility : AbilityBase
{
    private Rigidbody _rb;
    private float _dashForce = 150f;
    public override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public override void Activate()
    {
        // Check if player has enough stamina to dash
        if(staminaManager._currentStamina < staminaNeeded) canUse = false;
        if (!canUse) return;

        // Apply dash force in the direction the player is facing
        _rb.AddForce(transform.forward * _dashForce, ForceMode.Impulse);
        // Consume stamina for dashing
        staminaManager.TakeStamina(staminaNeeded);
    }
}

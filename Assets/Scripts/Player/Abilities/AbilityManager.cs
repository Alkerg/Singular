using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    public List<AbilityBase> abilities;

    void Awake()
    {
    }

    void Start()
    {
        
    }

    public void ActivateTelekinesisAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            abilities[0].Activate();
        }
    }

    public void ActivateDashAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            abilities[1].Activate();
        }
    }

    public void ActivateFlyAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            abilities[2].Activate();
        }
    }
}

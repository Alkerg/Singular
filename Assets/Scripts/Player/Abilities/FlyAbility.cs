using Unity.VisualScripting;
using UnityEngine;

public class FlyAbility : AbilityBase
{
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    public override void Activate()
    {
        Debug.Log("Fly Ability Activated");
    }
}

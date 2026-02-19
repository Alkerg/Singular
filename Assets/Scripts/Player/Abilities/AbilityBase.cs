using UnityEngine;

public interface IAbility
{
    abstract void Activate();
    void Deactivate();
}

public class AbilityBase : MonoBehaviour, IAbility
{
    protected StaminaManager staminaManager;
    public float staminaNeeded = 20f;
    public bool canUse = true;

    public virtual void Start()
    {
        staminaManager = GetComponent<StaminaManager>();
    }
    public virtual void Activate()
    {

    }
    public virtual void Deactivate(){}
}

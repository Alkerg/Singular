using UnityEngine;

public interface IWeapon
{
    void Shoot();
    void Reload();
}

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public WeaponData weaponData;

    public abstract void Shoot();

    public abstract void Reload();
}

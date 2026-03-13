using UnityEngine;

public interface IWeapon
{
    void Shoot();
    void Reload();
}

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public WeaponData weaponData;
    public WeaponIKData weaponIKData;
    public Transform rightHandController;
    public Transform leftHandController;
    public AudioSource audioSource;

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public virtual void Shoot()
    {
        audioSource.PlayOneShot(weaponData.shootSound);
    }

    public abstract void Reload();
}

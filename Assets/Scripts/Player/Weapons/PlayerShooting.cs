using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private int _currentWeaponIndex = 0;
    public WeaponBase[] weapons;
    private WeaponBase _currentWeapon;
    void Start()
    {
        _currentWeapon = weapons[_currentWeaponIndex];
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Launch raycast from camera
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, _currentWeapon.weaponData.range))
            {
                Debug.Log("Hit: " + hit.collider.name);

                // Check if hit object is an enemy
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Get enemy health component and apply damage
                    /* HealthManager enemyHealth = hit.collider.GetComponent<HealthManager>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(_currentWeapon.weaponData.damage);
                    } */
                    EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                    if (enemy != null) enemy.TakeDamage(_currentWeapon.weaponData.damage);
                }
            }

        }
    }
    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        weapons[_currentWeaponIndex].gameObject.SetActive(false);
        _currentWeaponIndex = (_currentWeaponIndex + 1) % weapons.Length;
        weapons[_currentWeaponIndex].gameObject.SetActive(true);
        _currentWeapon = weapons[_currentWeaponIndex];
    }
}

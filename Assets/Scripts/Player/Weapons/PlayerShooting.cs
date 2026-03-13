using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private int _currentWeaponIndex = 0;
    private PlayerMovement _playerMovement;
    private Coroutine _rotatePlayerCoroutine;
    public WeaponBase[] weapons;
    public WeaponBase _currentWeapon {get; private set;}
    
    
    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _currentWeapon = weapons[_currentWeaponIndex];
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Rotate player towards camera forward direction
            if(_rotatePlayerCoroutine != null) StopCoroutine(_rotatePlayerCoroutine);
            _rotatePlayerCoroutine = StartCoroutine(_playerMovement.RotatePlayerTowardsCameraForward());

            // Play SFX
            _currentWeapon.Shoot();
            
            // Set IK controllers
            if(!_playerMovement._isAiming) _playerMovement.SetIKControllers(); 

            // Launch raycast from camera
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, _currentWeapon.weaponData.range))
            {
                Debug.Log("Hit: " + hit.collider.name);

                // Check if hit object is an enemy
                if (hit.collider.CompareTag("Enemy"))
                {
                    EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                    if (enemy != null) enemy.TakeDamage(_currentWeapon.weaponData.damage);
                }
            }

        }
        
        if(context.canceled)
        {
            // Reset IK controllers
            if(!_playerMovement._isAiming) _playerMovement.ResetIKControllers(); 
        }
    }

    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        // Disable current weapon and enable next one
        weapons[_currentWeaponIndex].gameObject.SetActive(false);
        _currentWeaponIndex = (_currentWeaponIndex + 1) % weapons.Length;
        weapons[_currentWeaponIndex].gameObject.SetActive(true);
        _currentWeapon = weapons[_currentWeaponIndex];
        
        // Update IK targets for new weapon on aiming state
        if(_playerMovement._isAiming) _playerMovement.SetIKControllers(); 
    }
}

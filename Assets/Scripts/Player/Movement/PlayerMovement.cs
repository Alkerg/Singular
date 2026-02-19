using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Vector2 _moveInput;
    private float _speed;
    private float _normalSpeed = 7f;
    private float _sprintSpeed = 9f;
    private bool _isAiming;
    private Transform _cam => Camera.main.transform;
    Vector3 _cachedAimDir;

    public CinemachineCamera freeLookCamera;
    public CinemachineCamera thirdPersonAimCamera;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.FindActionMap("Global").Enable();
        _speed = _normalSpeed;
    }


    void Update()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

    }

    void LateUpdate()
    {
        if (_isAiming)
        {
            _cachedAimDir = thirdPersonAimCamera.transform.forward;
            _cachedAimDir.y = 0f;
        }

        if (_isAiming && _cachedAimDir.sqrMagnitude > 0.001f)
        {
            Vector3 aimDir = thirdPersonAimCamera.transform.forward;
            aimDir.y = 0f;
            _rb.MoveRotation(Quaternion.LookRotation(aimDir));
        } 
    }

    void FixedUpdate()
    {
        Vector3 camForward = _cam.forward;
        Vector3 camRight = _cam.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 movement = camForward.normalized * _moveInput.y + camRight.normalized * _moveInput.x;

        _rb.linearVelocity = movement * _speed;

        if (!_isAiming && movement.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(movement),
                0.2f
            );
        }

        
   
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _speed = context.performed ? _sprintSpeed : _normalSpeed;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAiming = true;

            freeLookCamera.Priority = 0;
            thirdPersonAimCamera.Priority = 10;
        }

        if (context.canceled)
        {
            _isAiming = false;

            freeLookCamera.Priority = 10;
            thirdPersonAimCamera.Priority = 0;
        }
    }
}

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Aiming
}

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
    private Vector3 _cachedAimDir;
    private Animator _animator;
    private PlayerState _currentState;

    public CinemachineCamera freeLookCamera;
    public CinemachineCamera thirdPersonAimCamera;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _playerInput.actions.FindActionMap("Global").Enable();
        _speed = _normalSpeed;
        _currentState = PlayerState.Idle;
    }


    void Update()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        if (_currentState == PlayerState.Idle && _moveInput.sqrMagnitude > 0.01f)
        {
            _currentState = PlayerState.Walking;
            _animator.SetBool("isWalking", true);
        }
        else if (_currentState == PlayerState.Walking && _moveInput.sqrMagnitude <= 0.01f)
        {
            _currentState = PlayerState.Idle;
            _animator.SetBool("isWalking", false);
        }
    }

    void LateUpdate()
    {
        /* Vector3 aimDir = thirdPersonAimCamera.transform.forward;
        aimDir.y = 0f;
        
        if (_isAiming && aimDir.sqrMagnitude > 0.001f)
        {
            _rb.MoveRotation(Quaternion.LookRotation(aimDir));
        }  */

        if (_isAiming)
        {
            Vector3 aimDir = _cam.forward;
            aimDir.y = 0f;

            if (aimDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimDir);
                _rb.MoveRotation(Quaternion.Slerp(
                    _rb.rotation,
                    targetRotation,
                    15f * Time.deltaTime
                ));
            }
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

            thirdPersonAimCamera.transform.position = freeLookCamera.transform.position;
            thirdPersonAimCamera.transform.rotation = freeLookCamera.transform.rotation;

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

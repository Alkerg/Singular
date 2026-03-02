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
    private Vector2 _lookInput;
    private float _speed;
    private float _normalSpeed = 7f;
    private float _sprintSpeed = 9f;
    private bool _isAiming;
    private Transform _cam => Camera.main.transform;
    private Animator _animator;
    private PlayerState _currentState;
    private float _pitch;

    [Header("Look Settings")]
    [SerializeField] private float _lookSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -30f;
    [SerializeField] private float _maxPitch = 60f;

    [Header("Camera Target")]
    [SerializeField] private Transform _cameraTarget;

    [Header("Cinemachine Cameras")]
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
        _lookInput = _playerInput.actions["Look"].ReadValue<Vector2>();

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

        // Rotating the player based on look input when aiming
        if (_isAiming)
        {
            // Rotating player on Y axis (left and right looking)
            float yRotation = _lookInput.x * _lookSensitivity;
            //transform.Rotate(Vector3.up, yRotation);
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, yRotation, 0f));

            // Rotating camera target on X axis (up and down looking)
            _pitch -= _lookInput.y * _lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            _cameraTarget.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }


    }

    void LateUpdate()
    {
        
    }


    void FixedUpdate()
    {
        // Get camera's forward and right vectors
        Vector3 camForward = _cam.forward;
        Vector3 camRight = _cam.right;

        camForward.y = 0;
        camRight.y = 0;

        // Calculate movement direction based on input and camera orientation
        Vector3 movement = camForward.normalized * _moveInput.y + camRight.normalized * _moveInput.x;

        // Apply X, Z movement to player
        _rb.linearVelocity = movement * _speed;

        // Rotate player to face movement direction if not aiming
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
        // Toggle player's speed between normal and sprinting based on input
        _speed = context.performed ? _sprintSpeed : _normalSpeed;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {  
            // Set aiming camera priority
            _isAiming = true;

            freeLookCamera.Priority = 0;
            thirdPersonAimCamera.Priority = 10;
        }

        if (context.canceled)
        {
            // Set free look camera priority
            _isAiming = false;

            freeLookCamera.Priority = 10;
            thirdPersonAimCamera.Priority = 0;
        }
    }
}

using System.Collections;
using System.Xml.Serialization;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Aiming,
    Dead
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
    private Transform _cam => Camera.main.transform;
    private Animator _animator;
    private float _pitch;
    private PlayerShooting _playerShooting;
    private HealthManager _healthManager;
    public bool _isAiming {get; private set;}
    public PlayerState _currentState { get; private set; }
    public Transform weaponContainer;

    [Header("Aiming Right Hand")]
    public TwoBoneIKConstraint rightHandIK;
    public Transform rightHandIKTarget;
    public Transform rightHandController;
    public Transform rightHandHint;

    [Header("Aiming Left Hand")]
    public TwoBoneIKConstraint leftHandIK;
    public Transform leftHandIKTarget;
    public Transform leftHandController;
    public Transform leftHandHint;

    [Header("Look Settings")]
    [SerializeField] private float _lookSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -30f;
    [SerializeField] private float _maxPitch = 60f;

    [Header("Camera Target")]
    [SerializeField] private Transform _cameraTarget;

    [Header("Cinemachine Cameras")]
    public CinemachineCamera freeLookCamera;
    public CinemachineCamera thirdPersonAimCamera;
    public bool isRotatingTowardsCamera = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _animator = GetComponent<Animator>();
        _playerShooting = GetComponent<PlayerShooting>();
        _healthManager = GetComponent<HealthManager>();
        _healthManager.OnPlayerDeath += Die;
        rightHandIK.weight = 0f;
        leftHandIK.weight = 0f;
        _playerInput.actions.FindActionMap("Global").Enable();
        _speed = _normalSpeed;
        _currentState = PlayerState.Idle;
    }


    void Update()
    {
        _moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();
        _lookInput = _playerInput.actions["Look"].ReadValue<Vector2>();

        if(_currentState == PlayerState.Dead) return;
        

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
        if (!_isAiming && movement.sqrMagnitude > 0.01f && !isRotatingTowardsCamera)
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
            // Set IK controllers to aiming positions and weights
            SetIKControllers();
            
            // Set aiming camera priority
            _isAiming = true;

            freeLookCamera.Priority = 0;
            thirdPersonAimCamera.Priority = 10;
        }

        if (context.canceled)
        {
            // Set IK weights to 0 to disable aiming pose
            rightHandIK.weight = 0f;
            leftHandIK.weight = 0f;

            // Set free look camera priority
            _isAiming = false;

            freeLookCamera.Priority = 10;
            thirdPersonAimCamera.Priority = 0;
        }
    }

    public void SetIKControllers()
    {
        // Set IK weights to enable aiming pose
        rightHandIK.weight = _playerShooting._currentWeapon.weaponIKData.rightHandIKWeight;
        leftHandIK.weight = _playerShooting._currentWeapon.weaponIKData.leftHandIKWeight;

        // Set weapon hands to hand controllers
        rightHandController.localPosition = _playerShooting._currentWeapon.weaponIKData.rightHandControllerPosition;
        rightHandController.localRotation = Quaternion.Euler(_playerShooting._currentWeapon.weaponIKData.rightHandControllerRotation);

        leftHandController.localPosition = _playerShooting._currentWeapon.weaponIKData.leftHandControllerPosition;
        leftHandController.localRotation = Quaternion.Euler(_playerShooting._currentWeapon.weaponIKData.leftHandControllerRotation);

        // Set weapon controllers for IK
        rightHandIKTarget.position =  rightHandController.position;
        rightHandIKTarget.rotation = rightHandController.rotation;

        leftHandIKTarget.position = leftHandController.position;
        leftHandIKTarget.rotation = leftHandController.rotation;
    }

    public void ResetIKControllers()
    {
        rightHandIK.weight = 0f;
        leftHandIK.weight = 0f;
    }

    public IEnumerator RotatePlayerTowardsCameraForward()
    {
        isRotatingTowardsCamera = true;
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude < 0.01f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                80 * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = targetRotation; 
        isRotatingTowardsCamera = false;
    }

    public void Die()
    {
        if (_currentState == PlayerState.Dead) return;

        _currentState = PlayerState.Dead;
        _animator.SetTrigger("isDead");        
    }
}

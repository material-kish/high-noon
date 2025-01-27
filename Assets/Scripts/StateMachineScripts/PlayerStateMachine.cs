using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // input booleans (iHeartGameDev tutorial)
    bool _isJumpPressed = false;
    bool _isMovementPressed;

    // constants (iHeartGameDev)
    float _rotationFactorPerFrame = 15;

    public ParticleSystem jetFire1;
    public ParticleSystem jetFire2;
    public PlayerInventory playerInventory;

    [SerializeField] float _rotationSpeed;
    [SerializeField] float _turnSmoothTime;
    [SerializeField] float _jumpSpeed;
    [SerializeField] float _jumpButtonGracePeriod;
    [SerializeField] float _jetForce;
    [SerializeField] float _jumpHorizontalSpeed;
    [SerializeField] float _boostSpeed;
    [SerializeField] float _boostTime;
    [SerializeField] float _wallJumpBuffer;
    [SerializeField] float _wallJumpForce;
    [SerializeField] Transform _cameraTransform;

    CharacterController _characterController;
    Animator _animator;
    CapsuleCollider _capsuleCollider;

    float _ySpeed;
    float _inputMagnitude;
    float? _jumpButtonPressedTime;
    float? _lastGroundedTime;
    float _origStepOffset;
    float _wallJumpStartTime;
    bool _touchingWall;
    bool _wallJumpStarted;
    bool _raycastIsGrounded;
    bool _wasJumpPressed;
    Vector2 _moveVector2;
    Vector3 _betterMoveVector;
    Vector3 _moveDirection;
    Vector3 _wallJumpNormal;
    //Vector3 _airVelocity;

    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Animator Animator { get { return _animator; } }
    public CapsuleCollider CapsuleCollider { get { return _capsuleCollider; } }
    public float JumpButtonGracePeriod { get { return _jumpButtonGracePeriod; } }
    public float YSpeed { get { return _ySpeed; } set { _ySpeed = value; } }
    public float JumpSpeed { get { return _jumpSpeed; } }
    public float OrigStepOffset { get { return _origStepOffset; } }
    public float InputMagnitude { get { return _inputMagnitude; } }
    public float JumpHorizontalSpeed { get { return _jumpHorizontalSpeed; } }
    public float? JumpButtonPressedTime { get { return _jumpButtonPressedTime; } set { _jumpButtonPressedTime = value; } }
    public float? LastGroundedTime { get { return _lastGroundedTime; } set { _lastGroundedTime = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool WasJumpPressed { get { return _wasJumpPressed; } set { _wasJumpPressed = value; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool RaycastIsGrounded { get { return _raycastIsGrounded; } }
    public Vector2 MoveVector2 { get { return _moveVector2; } set { _moveVector2 = value; } }
    public Vector3 BetterMoveVector { get { return _betterMoveVector; } set { _betterMoveVector = value; } }
    //public Vector3 AirVelocity { get { return _airVelocity; } set { _airVelocity = value; } }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();


        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.UpdateStates();


        //_ySpeed += Physics.gravity.y * Time.deltaTime;

        RaycastGroundCheck();

        _animator.SetFloat("Input Magnitude", _inputMagnitude, 0.05f, Time.deltaTime);

        // set movement vector to inputs
        _betterMoveVector.x = MoveVector2.x;
        _betterMoveVector.z = MoveVector2.y;
        HandleRotation();

    }

    void HandleRotation()
    {
        // move in direction of camera
        _betterMoveVector = Quaternion.AngleAxis(_cameraTransform.rotation.eulerAngles.y, Vector3.up) * _betterMoveVector;
        _betterMoveVector.Normalize();

        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(_betterMoveVector, Vector3.up);
            // rotate the character to face the positionToLookAt            
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);

        }
    }

    

    void RaycastGroundCheck()
    {
        float extraRaycastHeight = 0.1f;
        _raycastIsGrounded = Physics.Raycast(_capsuleCollider.bounds.center, Vector3.down, _capsuleCollider.bounds.extents.y + extraRaycastHeight);
        if (_raycastIsGrounded)
        {
            //Debug.Log("is grounded man");
            _lastGroundedTime = Time.time;
        }
        //else
        //{
        //    Debug.Log("not grounded man");
        //}
    }


    //input functions
    public void JumpFunction(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _jumpButtonPressedTime = Time.time;
    }

    public void MoveFunction(InputAction.CallbackContext context)
    {
        if (context.performed || context.started)
        {
            _moveVector2 = context.ReadValue<Vector2>();
            //Debug.Log("_moveVector2: " + _moveVector2);
            _inputMagnitude = _moveVector2.magnitude;
            _isMovementPressed = _inputMagnitude != 0;
            _moveVector2.Normalize();
        }
        else
        {
            Debug.Log("movement exited");
            _moveVector2 = Vector2.zero;
            _isMovementPressed = false;
        }
    }

    public void JetUpFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jet Up performed");
        }
    }

    public void JetBoostFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Forward Boost performed");
        }
    }

    public void DebugCurrentStates(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Current State: " + _currentState);
        }
    }

}

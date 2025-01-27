using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SavedMovement : MonoBehaviour
{
    //declaring stuff

    public ParticleSystem jetFire1;
    public ParticleSystem jetFire2;
    public PlayerInventory playerInventory;

    [SerializeField] float rotationSpeed;
    [SerializeField] float turnSmoothTime;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpButtonGracePeriod;
    [SerializeField] float jetForce;
    [SerializeField] float jumpHorizontalSpeed;
    [SerializeField] float wallJumpCooldown;
    [SerializeField] float wallJumpForce;
    [SerializeField] float boostSpeed;
    [SerializeField] float boostTime;
    [SerializeField] Transform cameraTransform;

    CharacterController characterController;
    Animator animator;

    float turnSmoothVelocity;
    float ySpeed;
    float? jumpButtonPressedTime;
    float? lastGroundedTime;
    float origStepOffset;
    float inputMagnitude;
    float wallJumpTimer;
    bool moveInputted;
    bool isJumping;
    bool isGrounded;
    bool touchingWall;
    bool hasWallJumped = false;
    Vector3 wallJumpNormal;
    Vector2 moveVector2;
    Vector3 betterMoveVector;
    Vector3 moveDirection;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        origStepOffset = characterController.stepOffset;
        jetFire1.Stop();
        jetFire2.Stop();
    }

    //input functions
    public void JumpFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump pressed");
            jumpButtonPressedTime = Time.time;

            // do wall jump
            if (touchingWall)
            {
                touchingWall = false;
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                moveDirection = wallJumpNormal;
                moveDirection.Normalize();
                hasWallJumped = true;

            }
        }
    }

    public void MoveFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Movement!");
            if (hasWallJumped != true)
            {
                moveVector2 = context.ReadValue<Vector2>();
                moveVector2.Normalize();
            }
        }
        else
        {
            moveVector2 = Vector2.zero;
        }
        betterMoveVector.x = moveVector2.x;
        betterMoveVector.z = moveVector2.y;
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

    // Update is called once per frame
    void Update()
    {
        //gravity and grounded
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
            wallJumpNormal = Vector3.zero;
        }

        JumpHandler();

        MoveHandler();

        AirMovementHandler();

    }

    void JumpHandler()
    {
        // if recently grounded - jump animation stuff
        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = origStepOffset;
            ySpeed = 0;
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);
            // if jump button pressed recently - for jumping slightly before hitting ground
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            characterController.stepOffset = 0;
            animator.SetBool("isGrounded", false);
            isGrounded = false;

            if (isJumping && ySpeed < 0 || ySpeed < -2)
            {
                animator.SetBool("isFalling", true);
            }
            else if (isJumping)
            {
                animator.SetBool("isFalling", false);
            }
        }
        if (hasWallJumped)
        { //wall jump timer
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
                wallJumpNormal = Vector3.zero;
                wallJumpTimer = wallJumpCooldown;
            }
        }
    }

    void MoveHandler()
    {
        // movement stuff
        if (betterMoveVector != Vector3.zero)
        {
            animator.SetFloat("Input Magnitude", moveVector2.magnitude, 0.05f, Time.deltaTime);
            animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(betterMoveVector.x, betterMoveVector.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    void AirMovementHandler()
    {
        if (!isGrounded)
        {
            Vector3 velocity;
            if (!hasWallJumped)
            {
                Debug.Log("hasWallJumped = false");
                velocity = moveDirection.normalized * moveVector2.magnitude * jumpHorizontalSpeed;
            }
            else
            {
                Debug.Log("hasWallJumped = true");
                velocity = moveDirection.normalized * wallJumpForce;
            }
            velocity.y = ySpeed;
            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y = ySpeed * Time.deltaTime;
            characterController.Move(velocity);
        }
    }

    //walljump
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!characterController.isGrounded && hit.normal.y < 0.1f)
        {
            touchingWall = true;
            wallJumpNormal = hit.normal;
        }
    }
}

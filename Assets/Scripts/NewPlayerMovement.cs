using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMovement : MonoBehaviour
{
    //declaring stuff

    public ParticleSystem jetFire1;
    public ParticleSystem jetFire2;
    public PlayerInventory playerInventory;
    public bool isPaused;

    [SerializeField] float rotationSpeed;
    [SerializeField] float turnSmoothTime;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpButtonGracePeriod;
    [SerializeField] float jetForce;
    [SerializeField] float jumpHorizontalSpeed;
    [SerializeField] float boostSpeed;
    [SerializeField] float boostTime;
    [SerializeField] float wallJumpBuffer;
    [SerializeField] float wallJumpForce;
    [SerializeField] Transform cameraTransform;

    CharacterController characterController;
    Animator animator;

    float turnSmoothVelocity;
    float ySpeed;
    float? jumpButtonPressedTime;
    float? lastGroundedTime;
    float origStepOffset;
    float wallJumpStartTime;
    bool isJumping; 
    bool isGrounded;
    bool touchingWall;
    bool wallJumpStarted;
    bool isJetUpPressed;
    Vector2 moveVector2;
    Vector3 betterMoveVector;
    Vector3 moveDirection;
    Vector3 wallNormal;
    Vector3 airVelocity;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        origStepOffset = characterController.stepOffset;
    }

    //input functions
    public void JumpFunction(InputAction.CallbackContext context)
    {
        
        if (context.started && !isPaused)
        {
            Debug.Log("Jump pressed");
            jumpButtonPressedTime = Time.time;
            //isJumpPressed = true;

            
            if (touchingWall && !characterController.isGrounded)
            {
                ySpeed = jumpSpeed;
                touchingWall = false;
                wallJumpStarted = true;
                wallJumpStartTime = Time.time;
                moveDirection = wallNormal;
            }
        }
    }

    public void MoveFunction(InputAction.CallbackContext context)
    {
        if (context.performed || context.started)
        {
            Debug.Log("Movement!");
            moveVector2 = context.ReadValue<Vector2>();
            moveVector2.Normalize();
        }
        else
        {
            moveVector2 = Vector2.zero;
        }
        betterMoveVector.x = moveVector2.x;
        betterMoveVector.z = moveVector2.y;
    }

    public void JetUpStartFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jet Up started");
            isJetUpPressed = true;
        }
        
    }

    public void JetUpStopFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jet Up ENDED");
            isJetUpPressed = false;
        }

    }

    public void JetBoostStartFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (playerInventory.fuelLeft > 0)
            {
                Debug.Log("Forward Boost performed");
                //isJetBoostPressed = true;
                playerInventory.FuelUsed("forward");
                //isJetBoostPressed = false;
                StartCoroutine(Boost(transform.forward));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //gravity and grounded
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (characterController.isGrounded)
        {
            //touchingWall = false;
            lastGroundedTime = Time.time;
            wallJumpStarted = false;
            wallNormal = Vector3.zero;
            //isJumpPressed = false;
        }

        touchingWall = false;

        JumpHandler();

        MoveHandler();

        JetHandler();

        AirMovementHandler();

        //Debug.Log("ySpeed = " + ySpeed);

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
            // (if recently grounded AND) if jump button pressed recently - for jumping slightly before hitting ground
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else // if in the air completely
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
    }

    void MoveHandler()
    {
        if (betterMoveVector != Vector3.zero)
        {
            animator.SetFloat("Input Magnitude", moveVector2.magnitude, 0.05f, Time.deltaTime);
            animator.SetBool("isMoving", true);
            float targetAngle = Mathf.Atan2(betterMoveVector.x, betterMoveVector.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            if (!wallJumpStarted)
            {
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            }
        }
        else // if no movement
        {
            animator.SetBool("isMoving", false);
        }
    }

    void AirMovementHandler()
    {
        if (!isGrounded)
        {
            if (!wallJumpStarted)
            {
                airVelocity = moveDirection.normalized * moveVector2.magnitude * jumpHorizontalSpeed;
            }
            else // if wall jumped
            {
                airVelocity = moveDirection.normalized * wallJumpForce;
            }
            airVelocity.y = ySpeed;
            characterController.Move(airVelocity * Time.deltaTime);
            if (Time.time - wallJumpStartTime >= wallJumpBuffer)
            {
                wallJumpStarted = false;
            }
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


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isGrounded && hit.normal.y < 0.1)
        {
            wallJumpStarted = false;
            touchingWall = true;
            wallNormal = hit.normal;
            //Debug.DrawRay(hit.point, wallNormal, Color.red, 1.25f);
        }
    }

    void JetHandler()
    {

        // jetpack trigger
        if (playerInventory.fuelLeft > 0)
        {
            // upwards
            if (isJetUpPressed)
            {
                playerInventory.FuelUsed("up");
                ySpeed += Time.deltaTime * jetForce;
                jetFire1.Play();
                jetFire2.Play();
            }
            else
            {
                jetFire1.Stop();
                jetFire2.Stop();
            }
        }
        else
        {
            jetFire1.Stop();
            jetFire2.Stop();
        }
    }

    //jetpack boost forward
    IEnumerator Boost(Vector3 vector)
    {
        float boostStartTime = Time.time;
        while (Time.time < boostStartTime + boostTime)
        {
            characterController.Move(vector * boostSpeed * Time.deltaTime);
            Quaternion toRotation = Quaternion.LookRotation(vector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            jetFire1.Play();
            jetFire2.Play();
            yield return null;
        }
        jetFire1.Stop();
        jetFire2.Stop();
    }

    //public void PauseUpdater()
    //{
    //    PauseMenu.pausePressed = true;
    //}
}

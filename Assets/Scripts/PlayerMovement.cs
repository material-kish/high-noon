using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpButtonGracePeriod;
    public float jetForce;
    public ParticleSystem jetFire1;
    public ParticleSystem jetFire2;
    public PlayerInventory playerInventory;


    [SerializeField] Transform cameraTransform;
    [SerializeField] float jumpHorizontalSpeed;
    [SerializeField] float wallJumpCooldown;
    [SerializeField] float wallJumpForce;
    [SerializeField] float boostSpeed;
    [SerializeField] float boostTime;


    Animator animator;
    CharacterController characterController;
    float ySpeed;
    float origStepOffset;
    float? lastGroundedTime;
    float? jumpButtonPressedTime;
    bool isJumping;
    bool isGrounded;
    Vector3 movementDirection;
    Vector2 moveVector2;
    float inputMagnitude;
    bool hasWallJumped = false;
    float wallJumpTimer;
    Vector3 wallJumpNormal;
    bool wallJumpReady;
    Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        origStepOffset = characterController.stepOffset;
        playerTransform = GetComponent<Transform>();
    }


    //input functions
    public void JumpFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Jump pressed");
            jumpButtonPressedTime = Time.time;
        }
    }

    public void MoveFunction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (hasWallJumped != true)
            {
                moveVector2 = context.ReadValue<Vector2>();
                inputMagnitude = Mathf.Clamp01(moveVector2.magnitude);
                animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
                moveVector2 = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
                moveVector2.Normalize();
            }
            if (wallJumpReady)
            {
                hasWallJumped = true;
                ySpeed = jumpSpeed;
                animator.SetBool("isJumping", true);
                isJumping = true;
                movementDirection = wallJumpNormal * wallJumpForce;
                inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
                animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
                movementDirection.Normalize();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        //if (hasWallJumped != true)
        //{
        //    movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        //    inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
        //    animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);
        //    movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        //    movementDirection.Normalize();
        //}

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
            // reset walljump detection
            wallJumpNormal = Vector3.zero;
        }

        //if (Input.GetButtonDown("Jump"))
        //{
        //    jumpButtonPressedTime = Time.time;
        //}

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            characterController.stepOffset = origStepOffset;
            ySpeed = 0;
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            animator.SetBool("isJumping", false);
            isJumping = false;
            animator.SetBool("isFalling", false);

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

        

        if (movementDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (!isGrounded)
        {
            Vector3 velocity = moveVector2 * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);
        }
        // jetpack trigger
        //if (playerInventory.fuelLeft > 0)
        //{
        //    // upwards
        //    if (Input.GetButton("Fire1"))
        //    {
        //        //movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        //        //movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
        //        //movementDirection.Normalize();
        //        playerInventory.FuelUsed("up");
        //        ySpeed += Time.deltaTime * jetForce;
        //        jetFire1.Play();
        //        jetFire2.Play();
        //    }
        //    //boost forward
        //    if (Input.GetButtonDown("Fire2"))
        //    {
        //        playerInventory.FuelUsed("forward");
        //        StartCoroutine(Boost(playerTransform.forward));
        //    }
        //}

        else
        {
            jetFire1.Stop();
            jetFire2.Stop();
        }

        //walljump timer
        if (hasWallJumped)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
                wallJumpNormal = Vector3.zero;
                wallJumpTimer = wallJumpCooldown;
            }
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
        if (!isGrounded  && hit.normal.y < 0.1f)
        {
            wallJumpReady = true;
            wallJumpNormal = hit.normal;
        }
    }
}

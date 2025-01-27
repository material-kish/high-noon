using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubstate();
    }

    public override void EnterState()
    {
        Debug.Log("Airborne State Entered");
        Ctx.Animator.SetBool("isJumping", true);
        Ctx.JumpButtonPressedTime = null;
        Ctx.LastGroundedTime = null;
        Ctx.CharacterController.stepOffset = 0;
    }

    public override void UpdateState()
    {
        HandleAirMovement();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Airborne State exited");
        Ctx.Animator.SetBool("isJumping", false);
        Ctx.Animator.SetBool("isFalling", false);
    }

    public override void InitializeSubstate()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void CheckSwitchStates()
    {
        //if (Ctx.RaycastIsGrounded)
        if (Time.time - Ctx.LastGroundedTime <= Ctx.JumpButtonGracePeriod)
        {
            SwitchState(Factory.Grounded());
        }
        //if (Ctx.WasJumpPressed || Ctx.YSpeed > 0)
        //{
        //    SwitchState(Factory.Jump());
        //}
        //else
        //{
        //    SwitchState(Factory.Falling());
        //}
    }

    void HandleAirMovement()
    {
        //gravity
        Ctx.YSpeed = 0;
        Ctx.YSpeed = Physics.gravity.y * Time.deltaTime;

        if (Ctx.YSpeed < 0.1f)
        {
            Debug.Log("Time to start falling");
            Ctx.Animator.SetBool("isFalling", true);
        }
        else
        {
            Debug.Log("Time to STOP falling");
            Ctx.Animator.SetBool("isFalling", false);
        }

        Vector3 airVelocity;
        airVelocity = Ctx.BetterMoveVector.normalized * Ctx.InputMagnitude * Ctx.JumpHorizontalSpeed;
        Debug.Log("YSpeed = " + Ctx.YSpeed);
        airVelocity.y = Ctx.YSpeed;
        Ctx.CharacterController.Move(airVelocity * Time.deltaTime);

    }
}

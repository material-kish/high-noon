using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubstate();
    }

    public override void EnterState()
    {
        Debug.Log("Grounded State Entered");
        Ctx.CharacterController.stepOffset = Ctx.OrigStepOffset;
        Ctx.Animator.SetBool("isGrounded", true);
        Ctx.Animator.SetBool("isJumping", false);
        Ctx.Animator.SetBool("isFalling", false);
        Ctx.YSpeed = -1;
    }

    public override void UpdateState()
    {
        JumpHandler();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Grounded state Exited");
        Ctx.Animator.SetBool("isGrounded", false);
        Ctx.Animator.SetBool("isJumping", true);
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
        //if (!Ctx.RaycastIsGrounded || Time.time - Ctx.JumpButtonPressedTime <= Ctx.JumpButtonGracePeriod)
        //{
        //    SwitchState(Factory.Airborne());
        //}
        if (!Ctx.RaycastIsGrounded)
        {
            SwitchState(Factory.Airborne());
        }

    }

    void OnAnimatorMove()
    {
        Vector3 velocity = Ctx.Animator.deltaPosition;
        velocity.y = Ctx.YSpeed * Time.deltaTime;
        Ctx.CharacterController.Move(velocity);
    }


    void JumpHandler()
    {
        //if (Ctx.IsJumpPressed)
        if (Time.time - Ctx.JumpButtonPressedTime <= Ctx.JumpButtonGracePeriod)
        {
            Debug.Log("jump speed set HAPPENING HERE");
            Ctx.YSpeed = Ctx.JumpSpeed;
        }
    }
}

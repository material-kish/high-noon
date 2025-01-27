//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerJumpState : PlayerBaseState
//{
//    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
//    : base(currentContext, playerStateFactory) { }

//    public override void EnterState()
//    {
//        Debug.Log("Jump state entered");
//        Ctx.WasJumpPressed = false;
//        JumpHandler();
//    }

//    public override void UpdateState()
//    {
//        CheckSwitchStates();
//    }

//    public override void ExitState()
//    {

//    }

//    public override void InitializeSubstate() { }

//    public override void CheckSwitchStates()
//    {
//        if (Ctx.YSpeed < 0)
//        {
//            SwitchState(Factory.Falling());
//        }
//    }
//    void JumpHandler()
//    {
//        Ctx.YSpeed = Ctx.JumpSpeed;
//        Ctx.JumpButtonPressedTime = null;
//    }
//}

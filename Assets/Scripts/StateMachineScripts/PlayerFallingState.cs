//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerFallingState : PlayerBaseState
//{
//    public PlayerFallingState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
//    : base(currentContext, playerStateFactory) { }

//    public override void EnterState()
//    {
//        Debug.Log("Fall state entered");
//        Ctx.Animator.SetBool("isFalling", true);
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
//        if (Ctx.YSpeed > 0)
//        {
//            SwitchState(Factory.Jump());
//        }
//    }
//}

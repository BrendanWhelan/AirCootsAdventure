using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class DyingState : State<PlayerStateMachine>
{
    public DyingState(PlayerStateMachine o) : base(o)
    {}
    public override void EnterState()
    {
        owner.isDying = true;
        owner.SetAnimationBool("isDying",true);
    }
    public override void ExitState()
    {
        owner.isDying = false;
        owner.SetAnimationBool("isDying",false);
    }
    public override void UpdateState()
    {
    }
    public override void FixedUpdateState()
    {
    }
}

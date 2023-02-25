using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class BounceState : State<PlayerStateMachine>
{

    public BounceState(PlayerStateMachine o) : base(o)
    {}

    private float bounceStateTime = 0.5f;
    private float timeInBounceState = 0f;
    
    public override void EnterState()
    {
        owner.isBouncing = true;
        timeInBounceState = 0f;
        owner.isJumping = true;
        owner.SetAnimationBool("isJumping",true);
    }
    
    public override void ExitState()
    {
        owner.isBouncing = false;
    }
    
    public override void UpdateState()
    {

        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }

        owner.CheckForGrapple();

        timeInBounceState += Time.deltaTime;
        if(timeInBounceState >= bounceStateTime)
            owner.ChangeState("Jump");
        
    }
    
    public override void FixedUpdateState()
    {
    }
}

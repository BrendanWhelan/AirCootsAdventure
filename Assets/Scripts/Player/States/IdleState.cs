using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class IdleState : State<PlayerStateMachine>
{
    private float timeSpentStandingStill = 0f;
    private bool checkingForSleep = true;
    public IdleState(PlayerStateMachine o) : base(o)
    {}
    public override void EnterState()
    {
        checkingForSleep = true;
        timeSpentStandingStill = 0f;
        //Setup idle animation
        //owner.SetMaxSpeed(7);
    }
    public override void ExitState()
    {
        if(!checkingForSleep)
            owner.SetAnimationBool("isSleeping",false);
    }
    public override void UpdateState()
    {
        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }
        
        //Check for attempted movement
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            owner.ChangeState("Move");
            return;
        }

        if (Input.GetMouseButton(1))
        {
            owner.ChangeState("Crouch");
            return;
        }
        
        //Check for grapple
        owner.CheckForGrapple();

        if (checkingForSleep)
        {
            timeSpentStandingStill += Time.deltaTime;
            if (timeSpentStandingStill >= 10f)
            {
                checkingForSleep = false;
                owner.SetAnimationBool("isSleeping",true);
            }
        }
    }
    public override void FixedUpdateState()
    {
    }
}

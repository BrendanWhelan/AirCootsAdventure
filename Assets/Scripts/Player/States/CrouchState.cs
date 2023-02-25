using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class CrouchState : State<PlayerStateMachine>
{
    private float crouchSpeed = 3f;
    private Vector3 moveDirection;
    
    public CrouchState(PlayerStateMachine o) : base(o)
    {}
    
    public override void EnterState()
    {
        //Set crouch anim bool
        moveDirection = Vector3.zero;
        owner.SetAnimationBool("isCrouching",true);
        owner.SetMaxSpeed(crouchSpeed);
    }
    public override void ExitState()
    {
        //crouch anim false
        owner.SetAnimationBool("isCrouching",false);
    }
    
    public override void UpdateState()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        if (!Input.GetMouseButton(1))
        {
            if (hor == 0 && vert == 0)
            {
                owner.ChangeState("Idle");
            }
            else
            {
                owner.ChangeState("Move");
            }
            return;
        }

        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }
        
        //Check for grapple
        owner.CheckForGrapple();

        moveDirection = owner.orientation.forward * vert + owner.orientation.right * hor;
        owner.SetAnimationBool("isMoving",moveDirection.magnitude > 0.1f);
    }
    
    public override void FixedUpdateState()
    {
        owner.MovePlayer(moveDirection.normalized, crouchSpeed, ForceMode.Force);
    }
}

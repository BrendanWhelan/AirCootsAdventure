using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class MoveState : State<PlayerStateMachine>
{
    private float walkSpeed = 5f;
    private float sprintSpeed = 10f;
    private Vector3 moveDirection;
    private bool isSprinting = false;
    
    public MoveState(PlayerStateMachine o) : base(o)
    {}
    
    public override void EnterState()
    {
        isSprinting = false;
        moveDirection = Vector3.zero;
        if(!owner.onIce)
            owner.SetMaxSpeed(walkSpeed);
        owner.SetAnimationBool("isMoving",true);
    }
    
    public override void ExitState()
    {
        owner.SetAnimationBool("isMoving",false);
    }
    
    public override void UpdateState()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isSprinting)
            {
                isSprinting = true;
                owner.SetMaxSpeed(sprintSpeed);
            }
        }
        else
        {
            if (isSprinting)
            {
                isSprinting = false;
                owner.SetMaxSpeed(walkSpeed);
            }
        }
        
        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }
        
        if (hor == 0 && vert == 0)
        {
            owner.ChangeState("Idle");
            return;
        }

        if (Input.GetMouseButton(1))
        {
            owner.ChangeState("Sliding");
        }
        
        //Check for grapple
        owner.CheckForGrapple();
        
        moveDirection = owner.orientation.forward * vert + owner.orientation.right * hor;
    }
    
    public override void FixedUpdateState()
    {
        if(owner.isGrounded)
            owner.MovePlayer(moveDirection.normalized,(isSprinting ? sprintSpeed : walkSpeed), ForceMode.Force);
    }
}

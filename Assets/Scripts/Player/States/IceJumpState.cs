using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class IceJumpState : State<PlayerStateMachine>
{
    private Vector3 moveDirection;

    private float airSpeed = 1.5f;
    public IceJumpState(PlayerStateMachine o) : base(o)
    {}
    
    public override void EnterState()
    {
        owner.ChangePhysicsMat(PlayerStateMachine.PlayerPhysics.Sliding);
        float speed = Mathf.Max(owner.GetSpeed(),3f);
        owner.SetMaxSpeed(speed);
        moveDirection = Vector3.zero;
        owner.isJumping = true;
        owner.SetAnimationBool("isJumping",true);
    }
    
    public override void ExitState()
    {
        owner.isJumping = false;
        owner.SetAnimationBool("isJumping",false);
    }
    
    public override void UpdateState()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }

        if (Input.GetMouseButton(1))
        {
            owner.ChangeState("Sliding");
        }
        
        owner.CheckForGrapple();

        moveDirection = owner.orientation.forward * vert + owner.orientation.right * hor;
    }
    
    public override void FixedUpdateState()
    {
        owner.MovePlayer(moveDirection.normalized,airSpeed, ForceMode.Force);
    }
}

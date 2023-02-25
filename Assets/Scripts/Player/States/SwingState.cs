using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class SwingState : State<PlayerStateMachine>
{
    private float maxSwingSpeed = 25f;
    
    private float thrustForce = 4f;
    //private float upwardThrustForce = 2f;

    private float horizontalInfluence;
    private float forwardInfluence;

    //private float maxReelForce = 150f;
    //private float currentReelForce = 0f;

    //private float timePassed = 0f;
    //private float timeToMaxReelForce = 1.5f;
    
    public SwingState(PlayerStateMachine o) : base(o)
    {}
    public override void EnterState()
    {
        owner.SetMaxSpeed(maxSwingSpeed);
        horizontalInfluence = 0f;
        forwardInfluence = 0f;
        owner.ChangePhysicsMat(PlayerStateMachine.PlayerPhysics.Sliding);
        owner.SetAnimationBool("isGrappling",true);
        owner.SetAnimationBool("isMoving",true);
        //currentReelForce = 0f;
        //timePassed = 0f;
    }
    public override void ExitState()
    {
        owner.SetAnimationBool("isGrappling",false);
        owner.SetAnimationBool("isMoving",false);
        owner.ChangePhysicsMat(PlayerStateMachine.PlayerPhysics.Standard);
        owner.RotateCam(Vector3.zero);
    }
    public override void UpdateState()
    {
        horizontalInfluence = Input.GetAxisRaw("Horizontal");

        forwardInfluence = Input.GetAxisRaw("Vertical");
        //if (forwardInfluence < 0f) forwardInfluence = 0f;
        
        owner.CheckForGrapple();
        //owner.UpdateGrapplePoint();
    }
    
    public override void FixedUpdateState()
    {
        Vector3 movement = owner.orientation.forward * forwardInfluence + owner.orientation.right * horizontalInfluence;
        movement = owner.GetGrappleDirection(movement);
        owner.MovePlayer(movement.normalized, thrustForce, ForceMode.Force);

        owner.RotateCam(movement.normalized*10);
    }
}

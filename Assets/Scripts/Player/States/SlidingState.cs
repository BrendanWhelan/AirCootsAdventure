using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class SlidingState : State<PlayerStateMachine>
{
    private float maxSlideSpeed = 50f;

    private float minimumSlideSpeed = 3f;

    private Vector3 slideDirection;

    private bool playingSlideSound = false;
    
    public SlidingState(PlayerStateMachine o) : base(o)
    {}
    
    public override void EnterState()
    {
        //Sliding anim
        //Lower player drag
        if (owner.isGrounded)
        {
            AudioManager.instance.PlaySound(Sounds.Sliding);
            playingSlideSound = true;
        }
        owner.SetMaxSpeed(Mathf.Max(owner.GetSpeed(),maxSlideSpeed));
        owner.ChangePhysicsMat(PlayerStateMachine.PlayerPhysics.Sliding);
        slideDirection = owner.GetVelocity().normalized;
        owner.SetAnimationBool("isCrouching",true);
        owner.isSliding = true;

    }
    
    public override void ExitState()
    {
        if(playingSlideSound)
            AudioManager.instance.StopSound(Sounds.Sliding);
        //Return to default ground drag
        owner.ChangePhysicsMat(PlayerStateMachine.PlayerPhysics.Standard);
        owner.SetAnimationBool("isCrouching",false);
        owner.isSliding = false;
    }
    
    public override void UpdateState()
    {
        if (owner.isGrounded)
        {
            if (!playingSlideSound)
            {
                AudioManager.instance.PlaySound(Sounds.Sliding);
                playingSlideSound = true;
            }
        }
        else
        {
            if (playingSlideSound)
            {
                AudioManager.instance.StopSound(Sounds.Sliding);
                playingSlideSound = false;
            }
        }
        if (owner.jumpButtonPressed)
        {
            owner.Jump();
        }
        
        //If ctrl released, return to move
        if (!Input.GetMouseButton(1))
        {
            if(owner.isGrounded)
                owner.ChangeState("Move");
            else
                owner.ChangeState("Jump");
        }
        
        //If speed drops below certain value return to crouch state
        if (owner.GetSpeed() <= minimumSlideSpeed)
        {
            owner.ChangeState("Crouch");
        }
        
        owner.CheckForGrapple();

    }
    
    public override void FixedUpdateState()
    {
        if (owner.OnSlope() && owner.GetVelocity().y <= -0.1f)
        {
            slideDirection = owner.GetSlopeSlideMove(slideDirection);
            owner.MovePlayer(slideDirection, 5, ForceMode.Force);
        }
    }
}

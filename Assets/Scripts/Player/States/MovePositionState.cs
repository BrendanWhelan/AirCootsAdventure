using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class MovePositionState : State<PlayerStateMachine>
{
    public Vector3 desiredPosition;

    private float maxMoveSpeed = 3f;

    private float attemptedMoveTime = 0f;
    private float maxMoveTime = 5f;
    
    public MovePositionState(PlayerStateMachine o) : base(o)
    {}
    
    public override void EnterState()
    {
        attemptedMoveTime = 0f;
        owner.SetMaxSpeed(maxMoveSpeed);
        moveDirection = Vector3.zero;
        owner.SetAnimationBool("isMoving",true);
    }
    
    public override void ExitState()
    {
        owner.SetAnimationBool("isMoving",false);
    }

    private Vector3 moveDirection;
    
    public override void UpdateState()
    {
        float distance = Vector3.Distance(owner.transform.position, desiredPosition);

        attemptedMoveTime += Time.deltaTime;
        if (attemptedMoveTime >= maxMoveTime || distance <= 0.1f)
        {
            owner.SetVelocity(Vector3.zero);
            owner.forcedMove = false;
            owner.ChangeState("Idle");
            return;
        }

        moveDirection = desiredPosition - owner.transform.position;
    }
    
    public override void FixedUpdateState()
    {
        owner.MovePlayer(moveDirection.normalized,maxMoveSpeed, ForceMode.Force);
    }
}

using System.Collections;
using System.Collections.Generic;
using StateMachines;
using UnityEngine;

public class GrapplingState : State<PlayerStateMachine>
{
    private Vector3 grapplePoint;
    private float overshootGrappleHeight = 1f;
    
    public GrapplingState(PlayerStateMachine o) : base(o)
    {}
    public override void EnterState()
    {
    }
    
    public override void ExitState()
    {
    }
    public override void UpdateState()
    {
    }
    public override void FixedUpdateState()
    {
    }

    public void SetGrapple(Vector3 hitPoint)
    {
        grapplePoint = hitPoint;
    }

    //Calculate the necessary force to create the proper arc. See https://www.youtube.com/watch?v=IvT8hjy6q4o
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight)/gravity));

        return velocityXZ + velocityY;
    }

    public void ExecuteGrapple()
    {
        Vector3 playerPos = owner.transform.position;
        Vector3 lowestPoint = new Vector3(playerPos.x, playerPos.y - 1f, playerPos.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootGrappleHeight;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootGrappleHeight;
        
        // owner.JumpToPosition(grapplePoint,highestPointOnArc);

        owner.DelayedStopGrapple();
    }
}

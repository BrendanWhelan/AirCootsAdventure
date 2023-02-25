using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingRope : MonoBehaviour
{
    [SerializeField]
    private LineRenderer grappleLine;
    [SerializeField]
    private PlayerStateMachine player;
    private Vector3 currentGrapplePosition;
    private Spring spring;
    
    //How many parts to the rope there are
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve effectCurve;

    private float initialGrappleTime = 0.2f;
    private float grappleTime = 0f;
    private bool reset = false;
    private bool initialGrapple = true;
    
    private void Awake()
    {
        spring = new Spring();
        spring.SetTarget(0);
    }
    private void LateUpdate()
    {
        DrawRope();
    }
    
    void DrawRope()
    {
        if (!player.threwGrapplingHook)
        {
            currentGrapplePosition = player.GetGrappleShootPoint();
            if (reset)
                return;
            if (grappleLine.positionCount > 0)
                grappleLine.positionCount = 0;
            grappleLine.enabled = false;
            grappleTime = 0f;
            reset = true;
            initialGrapple = true;
            return;
        }

        if (grappleLine.positionCount == 0)
        {
            spring.Reset();
            reset = false;
            grappleTime = 0f;
            initialGrapple = true;
            grappleLine.enabled = true;
            spring.SetVelocity(velocity);
            grappleLine.positionCount = quality + 1;
        }
        
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        Vector3 grapplePoint = player.GetGrapplePoint();
        Vector3 shootPoint = player.GetGrappleShootPoint();
        Vector3 up = Quaternion.LookRotation((grapplePoint - shootPoint).normalized) * Vector3.right;

        if (initialGrapple)
        {
            float percent = grappleTime / initialGrappleTime;
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, percent);
            if (percent >= 1f)
            {
                currentGrapplePosition = grapplePoint;
                initialGrapple = false;
            }
            grappleTime += Time.deltaTime;
        }
        else
        {
            currentGrapplePosition = grapplePoint;
        }
        
        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            Vector3 offset = up * (waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * effectCurve.Evaluate(delta));
            
            grappleLine.SetPosition(i,Vector3.Lerp(shootPoint,currentGrapplePosition,delta) + offset);
        }
    }
}

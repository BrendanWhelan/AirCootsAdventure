using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionRotate : Reaction
{
    [SerializeField]
    private float rotateSpeed = 0.2f;
    [SerializeField]
    Vector3 rotationToApply = Vector3.zero;

    private Vector3 returnRotation;

    private bool activated = false;

    private bool rotating = false;

    private float rotationTime = 0f;
    
    private void Awake()
    {
        returnRotation = transform.rotation.eulerAngles;
        rotationToApply = returnRotation + rotationToApply;
    }

    private void Update()
    {
        if (rotating)
        {
            if (GameManager.instance.GamePaused) return;

            rotationTime += Time.deltaTime;
            float percent = rotationTime / rotateSpeed;

            if (activated)
            {
                //Moving to rotation
                transform.rotation = Quaternion.Euler(Vector3.Lerp(returnRotation,rotationToApply,percent));
            }
            else
            {
                //Moving to start
                transform.rotation = Quaternion.Euler(Vector3.Lerp(rotationToApply,returnRotation, percent));
            }
            if (percent >= 1) rotating = false;
        }
    }

    public override void ActivateReaction(bool trigger)
    {
        base.ActivateReaction(trigger);

        activated = !activated;
        if (rotating)
        {
            rotationTime = rotateSpeed - rotationTime;
        }
        else
        {
            rotationTime = 0f;
        }
        rotating = true;
    }
}

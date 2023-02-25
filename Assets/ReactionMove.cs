using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReactionMove : Reaction
{
    [SerializeField]
    private float moveSpeed = 0.2f;
    [SerializeField]
    Vector3 moveToApply = Vector3.zero;

    private Vector3 returnMove;

    private bool activated = false;

    private bool moving = false;

    private float moveTime = 0f;
    
    private void Awake()
    {
        moveToApply = transform.position + moveToApply;
        returnMove = transform.position;
    }

    private void Update()
    {
        if (GameManager.instance.GamePaused) return;

        if (moving)
        {
            if (GameManager.instance.GamePaused) return;

            moveTime += Time.deltaTime;
            float percent = moveTime / moveSpeed;

            if (activated)
            {
                //Moving to rotation
                transform.position = Vector3.Lerp(returnMove,moveToApply,percent);
            }
            else
            {
                //Moving to start
                transform.position = Vector3.Lerp(moveToApply,returnMove, percent);
            }
            if (percent >= 1) moving = false;
        }
    }

    public override void ActivateReaction(bool trigger)
    {
        base.ActivateReaction(trigger);

        activated = !activated;
        if (moving)
        {
            moveTime = moveSpeed - moveTime;
        }
        else
        {
            moveTime = 0f;
        }
        moving = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReactionFallingBlock : Reaction
{
    [SerializeField]
    private float fallDrag = 0f;
    [SerializeField]
    private float mass;
    
    private Rigidbody rb;

    private Vector3 startPosition;

    private bool falling = false;
    
    private void Awake()
    {
        startPosition = transform.position;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = fallDrag;
        rb.mass = mass;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        GameManager.instance.SubscribeToPauseEvent(Paused);
    }

    public override void ActivateReaction(bool trigger)
    {
        base.ActivateReaction(trigger);
        if (trigger)
        {
            falling = true;
            rb.isKinematic = false;
        }
        else
        {
            falling = false;
            rb.isKinematic = true;
            transform.position = startPosition;
        }
    }

    private void Paused(bool paused)
    {
        if (paused)
        {
            rb.isKinematic = true;
        }
        else if(falling)
        {
            rb.isKinematic = false;
        }
    }
}

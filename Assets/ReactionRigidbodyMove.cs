using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionRigidbodyMove : MonoBehaviour
{
    private bool falling = false;

    [SerializeField]
    private Vector3 fallMove;
    
    [SerializeField]
    private float fallSpeed = 1f;

    private Vector3 startMove;

    [SerializeField]
    Rigidbody rb;

    private bool fell = false;
    private float fallTime = 0f;
    private void Awake()
    {
        startMove = transform.position;
        fallMove = startMove + fallMove;
    }

    private void FixedUpdate()
    {
        // if (falling)
        // {
        //     float percent = fallTime / fallSpeed;
        //     rb.MovePosition(Vector3.Lerp(startMove,fallMove,percent));
        //
        //     if (percent >= 1f) falling = false;
        //     
        //     fallTime += Time.fixedDeltaTime;
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (fell) return;
        if (collision.transform.tag.Equals("Player"))
        {
            falling = true;
            fell = true;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}

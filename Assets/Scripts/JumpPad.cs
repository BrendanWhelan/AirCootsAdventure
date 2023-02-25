using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private float jumpCooldown = 0.5f;
    private bool onCooldown = false;
    private float currentJumpCooldown = 0f;

    [SerializeField]
    private float jumpForce = 10f;

    private void Update()
    {
        if (onCooldown)
        {
            currentJumpCooldown += Time.deltaTime;
            if (currentJumpCooldown >= jumpCooldown)
            {
                onCooldown = false;
            }
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (onCooldown) return;
        if (other.transform.tag.Equals("Player"))
        {
            AudioManager.instance.PlaySound(Sounds.JumpPad);
            PlayerManager.instance.ApplyForce(transform.up * jumpForce, ForceMode.Impulse, true, true);
            onCooldown = true;
            currentJumpCooldown = 0f;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTouch : MonoBehaviour
{
    private float killCooldown = 2f;
    private bool onCooldown = false;
    private float currentKillCooldown = 0f;

    private void Update()
    {
        if (onCooldown)
        {
            currentKillCooldown += Time.deltaTime;
            if (currentKillCooldown >= killCooldown)
            {
                onCooldown = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onCooldown) return;
        if (collision.gameObject.tag.Equals("Player"))
        {
            PlayerManager.instance.KillPlayer();
            currentKillCooldown = 0f;
            onCooldown = true;
        }
    }
}

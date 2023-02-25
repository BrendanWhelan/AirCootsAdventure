using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
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
    
    private void OnTriggerEnter(Collider collider)
    {
        if (onCooldown) return;
        if (collider.tag.Equals("Player"))
        {
            PlayerManager.instance.KillPlayerDeathPlane(collider.bounds.max.y);
            currentKillCooldown = 0f;
            onCooldown = true;
        }
    }
}

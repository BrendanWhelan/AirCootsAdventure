using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float maxLife;
    private float currentLife;
    private float speed;
    
    public void Setup(Vector3 shootDirection, float shootSpeed, float maxLifetime)
    {
        transform.up = shootDirection;
        speed = shootSpeed;
        maxLife = maxLifetime;
        currentLife = 0f;
    }

    private void Update()
    {
        if (GameManager.instance.GamePaused) return;

        currentLife += Time.deltaTime;
        if (currentLife >= maxLife)
        {
            Die();
            return;
        }
        
        transform.position += transform.up * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerManager.instance.KillPlayer();
        }

        Die();
    }

    private void Die()
    {
        currentLife = 0f;
        this.gameObject.SetActive(false);
    }
}

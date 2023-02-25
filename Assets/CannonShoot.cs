using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShoot : MonoBehaviour
{
    [SerializeField]
    private float timeToShoot = 1f;
    private float currentShootTime = 0f;
    [SerializeField]
    private float bulletSpeed=1f;
    [SerializeField]
    private float bulletLifetime = 5f;

    private void Start()
    {
        Prewarm();
        currentShootTime = timeToShoot;
    }
    private void Update()
    {
        if (GameManager.instance.GamePaused) return;
        currentShootTime -= Time.deltaTime;
        if (currentShootTime <= 0)
        {
            Shoot();
            currentShootTime = timeToShoot;
        }
    }

    private void Shoot()
    {
        AudioManager.instance.PlaySoundPositional(Sounds.CannonShoot,transform.position);
        GameObject bullet = BulletPooling.instance.GetBullet(transform.position);
        bullet.GetComponent<Bullet>().Setup(transform.up, bulletSpeed, bulletLifetime);
    }

    private void Prewarm()
    {
        //We need to prewarm bullet positions based off of bullet lifetime and time to shoot.
        
        int numberOfBullets = Mathf.FloorToInt(bulletLifetime/timeToShoot);

        float bulletRemainingLifetime = bulletLifetime;
        for (int i = 0; i < numberOfBullets; i++)
        {
            GameObject bullet = BulletPooling.instance.GetBullet(transform.position + (transform.up *(bulletSpeed*i)));
            bullet.GetComponent<Bullet>().Setup(transform.up, bulletSpeed, bulletRemainingLifetime);
            bulletRemainingLifetime -= timeToShoot;
        }
    }
}

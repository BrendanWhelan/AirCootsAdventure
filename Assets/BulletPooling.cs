using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public static BulletPooling instance;

    [SerializeField]
    private GameObject bulletPrefab;
    
    private List<GameObject> bullets;
    
    public void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        instance = this;
        bullets = new List<GameObject>();
        AddBullets();
    }

    public GameObject GetBullet(Vector3 position)
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeSelf)
            {
                bullet.transform.position = position;
                bullet.SetActive(true);
                return bullet;
            }
        }
        
        //No inactive bullets, make more
        AddBullets();

        return bullets[bullets.Count-1];
    }

    private void AddBullets()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform);
            bullet.SetActive(false);
            bullets.Add(bullet);
        }
    }
}

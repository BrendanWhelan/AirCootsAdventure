using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    [SerializeField]
    private RespawnPoint[] respawnPoints;

    private int currentRespawnPoint = 0;

    [SerializeField]
    private float deathHeight = -100f;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
            }
        }

        instance = this;
    }

    public RespawnPoint GetRespawnPoint()
    {
        return respawnPoints[currentRespawnPoint];
    }

    public void SetRespawnPoint(int respawnPoint)
    {
        if (respawnPoint < currentRespawnPoint || respawnPoint >= respawnPoints.Length) return;
        currentRespawnPoint = respawnPoint;
    }

    public float GetDeathHeight()
    {
        return deathHeight;
    }
}

[System.Serializable]
public class RespawnPoint
{
    public Vector3 position;
    public Vector2 cameraValues;
}

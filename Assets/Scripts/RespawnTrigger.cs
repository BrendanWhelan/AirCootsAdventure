using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField]
    private int respawnPoint=0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            RespawnManager.instance.SetRespawnPoint(respawnPoint);
            this.gameObject.SetActive(false);
        }
    }
}

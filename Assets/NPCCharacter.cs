using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NPCCharacter : MonoBehaviour
{
    [SerializeField]
    private NPCNameHandler nameHandler;
    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //Display Name
            nameHandler.ShowName(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //Hide Name
            nameHandler.ShowName(false);
        }
    }

    public void Bounce()
    {
        transform.DOKill();
        transform.position = startPos;
        transform.DOShakePosition(1f, 0.05f);
    }
}

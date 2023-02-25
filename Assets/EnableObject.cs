using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToEnable;

    private void Start()
    {
        if(GameManager.instance.GetPartyObjective(3))
            Enable();
    }

    public void Enable()
    {
        objectToEnable.SetActive(true);
    }
}

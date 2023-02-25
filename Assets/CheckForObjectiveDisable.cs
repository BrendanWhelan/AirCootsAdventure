using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForObjectiveDisable : MonoBehaviour
{
    private void Start()
    {
        if(GameManager.instance.GetObjective(1))
            gameObject.SetActive(false);
    }
}

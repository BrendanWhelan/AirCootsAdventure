using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerUnlockObjective : MonoBehaviour
{
    [SerializeField]
    private int objectiveToMarkTrue;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
            GameManager.instance.SetObjective(objectiveToMarkTrue,true);
    }
}

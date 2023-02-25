using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPassThrough : Trigger
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            TriggerReactions(true);
        }
    }
}

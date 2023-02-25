using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnEnter : Trigger
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            TriggerReactions(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            TriggerReactions(false);
        }
    }
}

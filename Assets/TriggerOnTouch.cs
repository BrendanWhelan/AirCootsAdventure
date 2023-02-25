using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnTouch : Trigger
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            TriggerReactions(true);
        }
    }
}

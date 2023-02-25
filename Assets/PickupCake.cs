using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCake : Pickup
{
    [SerializeField]
    private Portal portalToActivate;

    private void Start()
    {
        if (GameManager.instance.GetObjective(0))
        {
            portalToActivate.EnablePortal();
            gameObject.SetActive(false);
        }
    }

    public override void PickUp()
    {
        base.PickUp();
        portalToActivate.EnablePortal();
        GameManager.instance.SetObjective(0,true);
        gameObject.SetActive(false);
    }
}

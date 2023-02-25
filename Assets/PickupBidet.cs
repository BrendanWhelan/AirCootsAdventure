using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBidet : Pickup
{
    [SerializeField]
    private Portal portalToActivate;
    
    private void Start()
    {
        if (GameManager.instance.GetObjective(2))
        {
            portalToActivate.EnablePortal();
            gameObject.SetActive(false);
        }
    }

    public override void PickUp()
    {
        base.PickUp();
        portalToActivate.EnablePortal();
        GameManager.instance.SetObjective(2,true);
        gameObject.SetActive(false);
    }
}

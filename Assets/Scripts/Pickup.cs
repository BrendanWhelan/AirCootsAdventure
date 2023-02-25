using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public virtual void PickUp()
    {
        AudioManager.instance.PlaySound(Sounds.ItemPickup);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCoin : Pickup
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite fadedSprite;
    private bool alreadyPickedUp = false;
    public override void PickUp()
    {
        base.PickUp();
        if(!alreadyPickedUp)
            ScoreManager.instance.PickupCoin(this);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up,90f*Time.deltaTime);
    }

    public void SetAlreadyPickedUp(bool alreadyPicked)
    {
        alreadyPickedUp = alreadyPicked;
        if (alreadyPicked)
            spriteRenderer.sprite = fadedSprite;
    }
}

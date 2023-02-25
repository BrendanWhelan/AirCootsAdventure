using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionToggleCollision : Reaction
{
    [SerializeField]
    private Collider colliderToDisable;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material startMaterial;
    [SerializeField]
    private Material toggledMaterial;

    [SerializeField]
    private bool startEnabled = false;
    
    public override void ActivateReaction(bool trigger)
    {
        base.ActivateReaction(trigger);
        meshRenderer.sharedMaterial = trigger ? toggledMaterial : startMaterial;
        colliderToDisable.enabled = (!startEnabled && trigger);
    }
}

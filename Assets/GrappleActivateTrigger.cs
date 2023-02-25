using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleActivateTrigger : GrappleSpot
{
    [SerializeField]
    private Trigger[] triggers;
    public override void Grappled()
    {
        base.Grappled();
        AudioManager.instance.PlaySound(Sounds.BlueNodeActive);
        foreach (Trigger trigger in triggers)
        {
            trigger.TriggerReactions(true);
        }
    }
}

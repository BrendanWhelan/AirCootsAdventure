using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionOneTime : Reaction
{
    private bool activated = false;

    [SerializeField]
    private Reaction[] reactions;
    
    public override void ActivateReaction(bool trigger)
    {
        if (activated)
        {
            if (!trigger) activated = false;
            return;
        }
        base.ActivateReaction(trigger);

        if (trigger)
        {
            foreach (Reaction reaction in reactions)
            {
                reaction.ActivateReaction(true);
            }
        }
    }
}

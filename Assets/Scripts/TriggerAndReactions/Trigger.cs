using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    protected Reaction[] reactionsToActivate;
    //Triggers store an array of reactions that they can activate when conditions are met
    public virtual void TriggerReactions(bool trigger)
    {
        foreach (Reaction reaction in reactionsToActivate)
        {
            reaction.ActivateReaction(trigger);
        }
    }
}

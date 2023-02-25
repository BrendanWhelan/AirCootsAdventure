using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionRedActivate : Reaction
{
    [SerializeField]
    private Reaction[] redReactions;

    private bool isInStartState = true;
    
    private void Start()
    {
        GameManager.instance.SubscribeToPlayerDeathEvent(PlayerDied);
    }

    public override void ActivateReaction(bool trigger)
    {
        base.ActivateReaction(trigger);
        isInStartState = !isInStartState;
        AudioManager.instance.PlaySound(Sounds.RedRingSwap);
        foreach (Reaction reaction in redReactions)
        {
            reaction.ActivateReaction(trigger);
        }
    }

    private void PlayerDied()
    {
        if (!isInStartState)
            ActivateReaction(true);
    }
}

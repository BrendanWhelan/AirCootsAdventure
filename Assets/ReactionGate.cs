using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionGate : Reaction
{
    [SerializeField]
    private Reaction[] gateOpenReactions;

    [SerializeField]
    private Material offMaterial;
    [SerializeField]
    private Material onMaterial;

    [SerializeField]
    private MeshRenderer[] gateLights;

    [SerializeField]
    private bool[] numberOfGateTriggers;

    [SerializeField]
    private Reaction[] oneTimeReactionsToReset;

    private int reactionsNeeded = 0;
    private int reactionsAchieved = 0;
    
    private bool activated = false;
    
    private void Start()
    {
        GameManager.instance.SubscribeToPlayerDeathEvent(PlayerDied);
        reactionsNeeded = numberOfGateTriggers.Length;
    }

    private void ResetGate()
    {
        if (activated)
        {
            foreach (Reaction reaction in gateOpenReactions)
            {
                reaction.ActivateReaction(false);
            }
        }

        foreach (MeshRenderer gateLight in gateLights)
        {
            gateLight.sharedMaterial = offMaterial;
        }

        for (int i = 0; i < numberOfGateTriggers.Length; i++) numberOfGateTriggers[i] = false;

        foreach (Reaction reaction in oneTimeReactionsToReset)
        {
            reaction.ActivateReaction(false);
        }
        reactionsAchieved = 0;
        activated = false;
    }

    public override void ActivateReaction(bool trigger)
    {
        if (activated) return;
        base.ActivateReaction(trigger);

        gateLights[reactionsAchieved].sharedMaterial = onMaterial;

        numberOfGateTriggers[reactionsAchieved] = true;

        reactionsAchieved++;
        if (reactionsAchieved >= numberOfGateTriggers.Length)
        {
            activated = true;
            foreach (Reaction reaction in gateOpenReactions)
            {
                reaction.ActivateReaction(true);
            }
        }
    }

    private void PlayerDied()
    {
        ResetGate();
    }
}

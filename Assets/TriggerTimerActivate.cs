using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTimerActivate : Trigger
{
    [SerializeField]
    private float timeActivated = 2f;
    private float currentTimer = 0f;

    private bool triggered = false;

    private void Update()
    {
        if (GameManager.instance.GamePaused) return;
        if (triggered)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= timeActivated)
            {
                AudioManager.instance.PlaySound(Sounds.BlueNodeDeactivate);
                TriggerReactions(false);
            }
        }
    }
    
    public override void TriggerReactions(bool trigger)
    {
        if (triggered && trigger)
        {
            currentTimer = 0f;
            return;
        }
        

        currentTimer = 0f;
        triggered = trigger;

        base.TriggerReactions(trigger);
    }
}

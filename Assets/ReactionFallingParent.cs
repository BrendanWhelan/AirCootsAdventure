using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionFallingParent : Reaction
{
    [SerializeField]
    private float timeToFall;
    [SerializeField]
    private float delayBeforeFalling = 0.5f;
    [SerializeField]
    private float timeToRegen = 2f;

    private float currentRegenTime = 0f;
    private float currentDelayTime = 0f;
    private float currentFallTime = 0f;

    [SerializeField]
    private Material cubeMaterial;

    private bool isFalling = false;
    private bool inDelay = false;
    private bool isRegening = false;

    
    private Color fadedColor = new Color(1, 1, 1, 0f);

    [Tooltip("Used to disable the objects when fully faded without stopping the script")]
    [SerializeField]
    private GameObject subparent;

    [SerializeField]
    private ReactionFallingBlock[] fallingBlocks;

    private void Awake()
    {
        cubeMaterial.color = Color.white;
    }

    private void Update()
    {
        if (GameManager.instance.GamePaused) return;
        if (isFalling)
        {
            if (inDelay)
            {
                currentDelayTime += Time.deltaTime;
                if (currentDelayTime >= delayBeforeFalling)
                {
                    inDelay = false;
                    foreach (ReactionFallingBlock reaction in fallingBlocks)
                    {
                        reaction.ActivateReaction(true);
                    }
                }
            }
            else
            {
                currentFallTime += Time.deltaTime;
                float percent = currentFallTime / timeToFall;

                cubeMaterial.color = Color.Lerp(Color.white, fadedColor, percent);
                
                if (percent >= 1f)
                {
                    subparent.SetActive(false);
                    isFalling = false;
                    isRegening = true;
                    foreach (ReactionFallingBlock reaction in fallingBlocks)
                    {
                        reaction.ActivateReaction(false);
                    }
                }
            }
        }
        else if (isRegening)
        {
            currentRegenTime += Time.deltaTime;
            if (currentRegenTime >= timeToRegen)
            {
                cubeMaterial.color = Color.white;
                subparent.SetActive(true);
                isRegening = false;
            }
        }
    }

    public override void ActivateReaction(bool trigger)
    {
        if (isFalling || isRegening) return;
        
        base.ActivateReaction(trigger);

        isFalling = true;
        inDelay = true;
        isRegening = false;
        currentFallTime = 0f;
        currentDelayTime = 0f;
        currentRegenTime = 0f;
    }
}

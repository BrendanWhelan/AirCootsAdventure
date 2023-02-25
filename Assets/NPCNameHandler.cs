using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNameHandler : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer nameRenderer;

    private bool isShowing = false;
    private bool isTransitioning = false;

    private float timeToTransition = 1f;
    private float transitionTime = 0f;

    private Vector3 amountToMoveUp = new Vector3(0, 0.2f, 0);

    private Vector3 startPosition;
    private void Awake()
    {
        startPosition = nameRenderer.transform.position;
        amountToMoveUp = startPosition + amountToMoveUp;
    }

    public void ShowName(bool show)
    {
        if (isTransitioning)
        {
            transitionTime = timeToTransition - transitionTime;
        }
        else
        {
            transitionTime = 0f;
        }
        
        isShowing = show;
        isTransitioning = true;
    }

    private void Update()
    {
        if (isTransitioning)
        {
            transitionTime += Time.deltaTime;
            float percent = transitionTime / timeToTransition;
            if (isShowing)
            {
                nameRenderer.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, percent);
                nameRenderer.transform.position = Vector3.Lerp(startPosition, amountToMoveUp, percent);
            }
            else
            {
                nameRenderer.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), percent);
                nameRenderer.transform.position = Vector3.Lerp(amountToMoveUp,startPosition, percent);
            }

            if (percent >= 1f)
            {
                isTransitioning = false;
            }
        }
    }
}

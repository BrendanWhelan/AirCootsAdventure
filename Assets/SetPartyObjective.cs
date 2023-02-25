using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SetPartyObjective : MonoBehaviour
{
    [SerializeField]
    private Canvas fadeCanvas;

    [SerializeField]
    private Image fadeImage;

    private int objectiveToMarkComplete = 0;

    [SerializeField]
    private Vector3 finishPosition;
    
    public void SetObjectiveComplete(int objective)
    {
        PlayerManager.instance.DisableControls(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeCanvas.enabled = true;
        fadeImage.DOColor(Color.black, 1f).OnComplete(FadeFinished);
        objectiveToMarkComplete = objective;
    }

    private void FadeFinished()
    {
        fadeImage.DOColor(Color.black, 1f).OnComplete(WaitFinished);
        GameManager.instance.SetPartyObjective(objectiveToMarkComplete);
        if (GameManager.instance.GetObjective(3))
        {
            PlayerManager.instance.MovePlayer(finishPosition);
        }
    }

    private void WaitFinished()
    {
        fadeImage.DOColor(new Color(0, 0, 0, 0), 1f).OnComplete(FadeInFinished);
    }
    
    private void FadeInFinished()
    {
        PlayerManager.instance.DisableControls(false);
    }
}

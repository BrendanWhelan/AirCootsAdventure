using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class PortalScoreHandler : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text coinsText;

    [SerializeField]
    private int level;
    
    private void Start()
    {
        //Get score
        TimeSpan time = GameManager.instance.GetLevelTime(level);
        int coins = GameManager.instance.GetCoins(level);
        UpdateText(time, coins);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, 0.5f);
        }
    }

    private void UpdateText(TimeSpan time, int numberOfCoins)
    {
        timeText.text = "Time: " + $"{time.Minutes:00}:{time.Seconds:00}";
        coinsText.text = "Coins: " + numberOfCoins.ToString() + "/15";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(instance.gameObject);
            }
        }
        instance = this;
        coinImage.sprite = coinSprite;
    }
    private bool timing = false;

    private float timeElapsed = 0f;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text coinText;

    [SerializeField]
    private PickupCoin[] coins;

    [SerializeField]
    private Image coinImage;

    [SerializeField]
    private Sprite coinSprite;
    
    private int coinCount = 0;

    [SerializeField]
    private int level;

    private bool[] coinsBool;

    private void Start()
    {
        timeElapsed = 0f;
        coinsBool = GameManager.instance.GetExactCoinBool(level);
        coinCount = GameManager.instance.GetCoins(level);
        timing = true;

        for (int i = 0; i < coinsBool.Length; i++)
        {
            coins[i].SetAlreadyPickedUp(coinsBool[i]);
        }
        
        UpdateText();
    }

    private void Update()
    {
        if (GameManager.instance.GamePaused || !timing) return;
        timeElapsed += Time.deltaTime;
        UpdateText();
    }

    private void UpdateText()
    {
        TimeSpan time = TimeSpan.FromSeconds(timeElapsed);
        timeText.text = $"{time.Minutes:00}:{time.Seconds:00}";
        coinText.text = coinCount.ToString() + "/15";
    }
    
    
    public void PickupCoin(PickupCoin coin)
    {
        coinCount++;
        UpdateText();
        for(int i =0; i < coins.Length;i++)
        {
            if (coins[i] == coin)
            {
                coinsBool[i] = true;
                return;
            }
        }
    }

    public void SetTiming(bool time)
    {
        timing = time;
    }

    public void Finish()
    {
        timing = false;
        GameManager.instance.SetCoinsExact(level,coinsBool);
        GameManager.instance.SetLevelTime(TimeSpan.FromSeconds(timeElapsed),level);
    }
}

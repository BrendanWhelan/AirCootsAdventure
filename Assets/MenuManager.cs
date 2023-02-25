using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Image crossfur;
    
    public static MenuManager instance;
    public void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }
        instance = this;
    }

    public void SetCrossfurOpacity(float opacity)
    {
        crossfur.color = new Color(1, 1, 1, opacity);
    }

    public void DisableCrossfur(bool disable)
    {
        crossfur.enabled = !disable;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessHandler : MonoBehaviour
{
    [SerializeField]
    private Volume ppVolume;

    public static PostProcessHandler instance;
    
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
    }
    // Start is called before the first frame update
    void Start()
    {
        ppVolume.enabled = SettingsManager.instance.GetPP();
    }

    public void SetPP(bool pp)
    {
        ppVolume.enabled = pp;
    }
}

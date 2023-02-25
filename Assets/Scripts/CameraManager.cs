using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField]
    private CinemachineBrain brain;
    [SerializeField]
    private CinemachineFreeLook thirdPersonCam;

    private float targetMaxHor = 1.5f;
    private float targetMaxVert = 0.025f;
    
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

    private void Start()
    {
        if (Camera.main != null)
        {
            CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        }
        
        GameManager.instance.SubscribeToPauseEvent(GamePaused);
        GamePaused(GameManager.instance.GamePaused);

        targetMaxHor = SettingsManager.instance.GetHorSensitivity();
        targetMaxVert = SettingsManager.instance.GetVertSense();

        bool x = SettingsManager.instance.GetXInvert();
        bool y = SettingsManager.instance.GetYInvert();
        
        thirdPersonCam.m_XAxis.m_InvertInput = x;
        thirdPersonCam.m_YAxis.m_InvertInput = !y;
        
        thirdPersonCam.m_XAxis.m_MaxSpeed = targetMaxHor;
        thirdPersonCam.m_YAxis.m_MaxSpeed = targetMaxVert;
    }

    private bool checkingForBlendFinish = false;

    private float bufferBlendstart = 0.1f;
    
    private void Update()
    {
        if (checkingForBlendFinish)
        {
            if (bufferBlendstart > 0f)
            {
                bufferBlendstart -= Time.deltaTime;
                return;
            }
            if (brain.IsBlending) return;

            checkingForBlendFinish = false;
            actionToTriggerOnBlendFinish?.Invoke();
            actionToTriggerOnBlendFinish = null;
        }
    }

    // private bool camDisabled = false;
    //
    // public void EnableThirdPersonCamera(bool enableCam)
    // {
    //     camDisabled = enableCam;
    // }

    private void GamePaused(bool gamePaused)
    {
        if (gamePaused)
        {
            thirdPersonCam.m_XAxis.m_MaxSpeed = 0;
            thirdPersonCam.m_YAxis.m_MaxSpeed = 0;
        }
        else
        {
            thirdPersonCam.m_XAxis.m_MaxSpeed = targetMaxHor;
            thirdPersonCam.m_YAxis.m_MaxSpeed = targetMaxVert;
        }
    }
    
    private UnityAction actionToTriggerOnBlendFinish;
    
    public void TriggerOnFinishBlend(UnityAction action)
    {
        bufferBlendstart = 0.1f;
        checkingForBlendFinish = true;
        actionToTriggerOnBlendFinish = action;
    }

    public void SetSensitivity(float horSens, float vertSens)
    {
        targetMaxHor = horSens;
        targetMaxVert = vertSens;
    }

    public void SetInversion(bool x, bool y)
    {
        thirdPersonCam.m_XAxis.m_InvertInput = x;
        thirdPersonCam.m_YAxis.m_InvertInput = !y;
    }
}

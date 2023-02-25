using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private GameObject portalCam;

    [SerializeField]
    private int levelToLoad = 0;

    [SerializeField]
    private int portalColor;
    
    private bool entered = false;

    [SerializeField]
    private bool portalEnabled = false;

    [SerializeField]
    private bool portalSubmitTime;
    private void OnTriggerEnter(Collider other)
    {
        if (entered || !portalEnabled) return;
        if (other.tag.Equals("Player"))
        {
            if (portalSubmitTime)
            {
                ScoreManager.instance.Finish();
            }
            AudioManager.instance.PlaySound(Sounds.Portal);
            AudioManager.instance.FadeCurrentSong();
            entered = true;
            portalCam.SetActive(true);
            PlayerManager.instance.FadePlayer();
            //CameraManager.instance.EnableThirdPersonCamera(false);
            GameManager.instance.PauseGame(true);
            CameraManager.instance.TriggerOnFinishBlend(LoadLevel);
        }
    }

    private void LoadLevel()
    {
        LoadingManager.instance.LoadScene(levelToLoad, portalColor);
    }

    public void EnablePortal()
    {
        portalEnabled = true;
    }
}

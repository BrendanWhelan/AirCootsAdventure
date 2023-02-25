using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public static EscapeMenu instance;

    private bool menuOpen = false;

    [SerializeField]
    private Canvas menuCanvas;

    public bool menuDisabled = false;

    private bool justOpened = false;
    
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

    private void Update()
    {
        //Don't open menu if game paused for other reasons
        if ((GameManager.instance.GamePaused && !menuOpen) || menuDisabled) return;
        if ((Input.GetKeyDown(KeyCode.Escape)  || Input.GetKeyDown(KeyCode.Tab))&& !justOpened)
        {
            if (menuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }

        justOpened = false;
    }

    public void OpenMenu()
    {
        justOpened = true;
        menuOpen = true;
        menuCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MenuManager.instance.DisableCrossfur(true);
        GameManager.instance.PauseGame(true);
    }

    public void CloseMenu()
    {
        menuOpen = false;
        menuCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MenuManager.instance.DisableCrossfur(false);
        GameManager.instance.PauseGame(false);
    }

    public void RestartButtonClicked()
    {
        menuCanvas.enabled = false;
        GameManager.instance.RestartLevel();
    }

    public void LeaveLevelButtonClicked()
    {
        menuCanvas.enabled = false;
        GameManager.instance.ReturnToHub();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SettingsClicked()
    {
        menuCanvas.enabled = false;
        SettingsManager.instance.OpenSettings();
    }
}

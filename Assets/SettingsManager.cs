using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
            return;
        }

        webGL = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3;
        if (webGL)
        {
            fullscreenToggle.SetIsOnWithoutNotify(false);
            fullscreenToggle.interactable = false;
            fullscreenToggle.image.color = new Color(1f, 1f, 1f, 0.5f);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    private Canvas settingsCanvas;
    public bool settingsOpen = false;

    private bool webGL;

    public void OpenSettings()
    {
        settingsOpen = true;
        EscapeMenu.instance.menuDisabled = true;
        settingsCanvas.enabled = true;
    }

    public void CloseSettings()
    {
        settingsOpen = false;
        settingsCanvas.enabled = false;
        EscapeMenu.instance.menuDisabled = false;
        EscapeMenu.instance.OpenMenu();
    }

    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private int currentResolution = 0;
    private int appliedResolution = 0;

    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider sfxVolumeSlider;

    private float currentMasterVolume = 0.5f;
    private float currentMusicVolume = 1f;
    private float currentSFXVolume = 1f;
    private float appliedMasterVolume = 0.5f;
    private float appliedMusicVolume = 1f;
    private float appliedSFXVolume = 1f;

    [SerializeField]
    private Toggle fullscreenToggle;
    [SerializeField]
    private Toggle vsyncToggle;
    [SerializeField]
    private Toggle postProcessingToggle;
    [SerializeField]
    private Toggle invertXToggle;
    [SerializeField]
    private Toggle invertYToggle;

    private bool appliedFullscreen = false;
    private bool currentFullscreen = false;

    private bool appliedVsync = false;
    private bool currentVsync = false;

    private bool appliedPP = true;
    private bool currentPP = true;

    private bool appliedInvertX = false;
    private bool currentInvertX = false;

    private bool appliedInvertY = false;
    private bool currentInvertY = false;

    [SerializeField]
    private TMP_Dropdown fpsDropdown;

    private int currentFPS = 0;
    private int appliedFPS = 0;

    [SerializeField]
    private Slider horSensitivity;
    [SerializeField]
    private Slider vertSensitivity;

    private float currentHorSens = 1.5f;
    private float appliedHorSens = 1.5f;

    private float currentVertSens = 0.025f;
    private float appliedVertSens = 0.025f;

    [SerializeField]
    private AudioMixer mainAudioMix;
    
    private void Start()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution
        {
            width = resolution.width,
            height = resolution.height
        }).Distinct().ToArray();
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == currentWidth && resolutions[i].height == currentHeight)
            {
                currentResolution = appliedResolution = i;
                break;
            }
        }

        List<TMP_Dropdown.OptionData> dropdowns = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(resolutions[i].width + "x" + resolutions[i].height);
            dropdowns.Add(option);
        }
        
        resolutionDropdown.AddOptions(dropdowns);
        
        UpdateResolutionDropdown();
        
        SaveLoadManager.instance.LoadSettings();
    }

    private void Update()
    {
        if (settingsOpen)
        {
            if ((Input.GetKeyDown(KeyCode.Escape) && !webGL)|| Input.GetKeyDown(KeyCode.Tab))
            {
                OnCancelClicked();
            }
        }
    }

    private void UpdateResolutionDropdown()
    {
        resolutionDropdown.SetValueWithoutNotify(currentResolution);
    }

    public void OnCancelClicked()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        //Reset fullscreen
        currentFullscreen = appliedFullscreen;
        fullscreenToggle.SetIsOnWithoutNotify(currentFullscreen);
        
        //Reset resolution
        currentResolution = appliedResolution;
        resolutionDropdown.SetValueWithoutNotify(currentResolution);

        if (!webGL)
        {
            Resolution res = resolutions[currentResolution];

            Screen.SetResolution(res.width, res.height, currentFullscreen);
        }
        
        currentFPS = appliedFPS;
        fpsDropdown.SetValueWithoutNotify(currentFPS);
        SetFPS();
        
        //Volume and sliders
        masterVolumeSlider.SetValueWithoutNotify(appliedMasterVolume);
        sfxVolumeSlider.SetValueWithoutNotify(appliedSFXVolume);
        musicVolumeSlider.SetValueWithoutNotify(appliedMusicVolume);
        currentMasterVolume = appliedMasterVolume;
        currentMusicVolume = appliedMusicVolume;
        currentSFXVolume = appliedSFXVolume;
        
        ChangeVolume(currentMasterVolume, "MasterVolume");
        ChangeVolume(currentMusicVolume, "MusicVolume");
        ChangeVolume(currentSFXVolume, "SFXVolume");

        //Post processing
        currentPP = appliedPP;
        postProcessingToggle.SetIsOnWithoutNotify(currentPP);
        if(PostProcessHandler.instance != null) PostProcessHandler.instance.SetPP(currentPP);
        postProcessingToggle.SetIsOnWithoutNotify(currentPP);

        //Vertical sync
        currentVsync = appliedVsync;
        vsyncToggle.SetIsOnWithoutNotify(currentVsync);
        if (currentVsync)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        
        //Camera Sens and Invert
        horSensitivity.SetValueWithoutNotify(appliedHorSens);
        vertSensitivity.SetValueWithoutNotify(appliedVertSens);
        currentHorSens = appliedHorSens;
        currentVertSens = appliedVertSens;
        CameraManager.instance.SetSensitivity(currentHorSens,currentVertSens);

        currentInvertX = appliedInvertX;
        invertXToggle.SetIsOnWithoutNotify(currentInvertX);
        currentInvertY = appliedInvertY;
        invertYToggle.SetIsOnWithoutNotify(currentInvertY);
        CameraManager.instance.SetInversion(currentInvertX, currentInvertY);
        
        CloseSettings();
    }

    public void ApplySettings()
    {
        appliedFullscreen = currentFullscreen;
        appliedResolution = currentResolution;
        appliedMasterVolume = currentMasterVolume;
        appliedMusicVolume = currentMusicVolume;
        appliedSFXVolume = currentSFXVolume;
        appliedHorSens = currentHorSens;
        appliedVertSens = currentVertSens;
        appliedFPS = currentFPS;
        appliedVsync = currentVsync;
        appliedPP = currentPP;
        appliedInvertX = currentInvertX;
        appliedInvertY = currentInvertY;
    }
    
    public void OnApplyClicked()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        ApplySettings();
        CloseSettings();
        SaveLoadManager.instance.SaveSettings();
    }

    public bool GetPP()
    {
        return appliedPP;
    }
    
    public void OnPPToggle()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentPP = postProcessingToggle.isOn;
        if (PostProcessHandler.instance != null) PostProcessHandler.instance.SetPP(currentPP);
    }

    public void OnFullscreenToggled()
    {
        if (webGL) return;
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = currentFullscreen;
    }

    public void OnMusicVolumeSliderChanged()
    {
        currentMusicVolume = musicVolumeSlider.value;
        ChangeVolume(currentMusicVolume,"MusicVolume");
    }
    
    public void OnSFXVolumeSliderChanged()
    {
        currentSFXVolume = sfxVolumeSlider.value;
        ChangeVolume(currentSFXVolume, "SFXVolume");
    }

    public void OnMasterVolumeSliderChanged()
    {
        currentMasterVolume = masterVolumeSlider.value;
        ChangeVolume(currentMasterVolume, "MasterVolume");
    }

    private void ChangeVolume(float value, string sliderName)
    {
        mainAudioMix.SetFloat(sliderName, Mathf.Log10(value) * 20);
    }
    
    public void OnResolutionDropdownOptionSelected()
    {
        if (webGL) return;
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentResolution = resolutionDropdown.value;
        if(currentResolution >= resolutions.Length)
        {
            Debug.LogWarning("Current Resolution Selected Greater Than The Number Of Resolution Options");
            currentResolution = appliedResolution;
            return;
        }

        Resolution res = resolutions[currentResolution];
        Screen.SetResolution(res.width, res.height, currentFullscreen);
    }

    public void OnFPSDropdownOptionSelected()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentFPS = fpsDropdown.value;
        SetFPS();
    }

    public bool GetXInvert()
    {
        return appliedInvertX;
    }

    public bool GetYInvert()
    {
        return appliedInvertY;
    }
    
    public void OnVertSensitivityChanged()
    {
        currentVertSens = vertSensitivity.value;
        CameraManager.instance.SetSensitivity(currentHorSens,currentVertSens);
    }

    public void OnHorSensitivityChanged()
    {
        currentHorSens = horSensitivity.value;
        CameraManager.instance.SetSensitivity(currentHorSens,currentVertSens);
    }

    public float GetHorSensitivity()
    {
        return appliedHorSens;
    }

    public float GetVertSense()
    {
        return appliedVertSens;
    }

    public void OnInvertXToggled()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentInvertX = invertXToggle.isOn;
        CameraManager.instance.SetInversion(currentInvertX, currentInvertY);
    }

    public void OnInvertYToggled()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentInvertY = invertYToggle.isOn;
        CameraManager.instance.SetInversion(currentInvertX, currentInvertY);
    }

    public void VSyncToggled()
    {
        AudioManager.instance.PlaySound(Sounds.MenuClick);
        currentVsync = vsyncToggle.isOn;
        if (currentVsync)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }
    
    private void SetFPS()
    {
        int framerate = 0;
        switch (currentFPS)
        {
            case 1:
                framerate = 60;
                break;
            case 2:
                framerate = 120;
                break;
            case 3:
                framerate = 144;
                break;
        }

        Application.targetFrameRate = framerate;
    }

    public SettingsInfo SaveSettings()
    {
        return new SettingsInfo(appliedFullscreen, appliedResolution, appliedMasterVolume, appliedMusicVolume, appliedSFXVolume, appliedHorSens, appliedVertSens, appliedPP, appliedFPS, appliedInvertX, appliedInvertY, appliedVsync);
    }
    
    public void LoadSettings(SettingsInfo settingsInfo)
    {
        if (settingsInfo != null)
        {
            currentFullscreen = settingsInfo.appliedFullscreen;

            currentResolution = settingsInfo.appliedResolution;
            if (currentResolution >= resolutions.Length)
                currentResolution = appliedResolution = 0;

            currentFPS = settingsInfo.appliedFPS;
            
            //Reset volumes and sliders
            currentMasterVolume = settingsInfo.appliedMasterVolume;
            currentMusicVolume = settingsInfo.appliedMusicVolume;
            currentSFXVolume = settingsInfo.appliedSFXVolume;

            //Camera stuff
            currentHorSens = settingsInfo.appliedHorSens;
            currentVertSens = settingsInfo.appliedVertSens;
            currentInvertX = settingsInfo.appliedInvertX;
            currentInvertY = settingsInfo.appliedInvertY;

            //Vsync and PP
            currentVsync = settingsInfo.appliedVsync;
            currentPP = settingsInfo.appliedPP;
        }
        else
        {
            currentFullscreen = true;
            currentFPS = 0;
            currentMasterVolume = 0.5f;
            currentMusicVolume = 0.4f;
            currentSFXVolume = 0.6f;

            if (!webGL)
            {
                currentHorSens = 1.5f;
                currentVertSens = 0.025f;
            }
            else
            {
                currentHorSens = 0.75f;
                currentVertSens = 0.0125f;
            }
            
            currentInvertX = false;
            currentInvertY = false;
            currentVsync = false;
            currentPP = true;
        }

        //Disable fullscreen in webgl
        
        if (webGL)
            currentFullscreen = false;
        
        fullscreenToggle.SetIsOnWithoutNotify(currentFullscreen);
        
        resolutionDropdown.SetValueWithoutNotify(currentResolution);
        fpsDropdown.SetValueWithoutNotify(currentFPS);

        if (currentResolution >= resolutions.Length)
        {
            currentResolution = 0;
        }
        if (!webGL)
        {
            Resolution res = resolutions[currentResolution];
            Screen.SetResolution(res.width, res.height, currentFullscreen);
        }
        
        SetFPS();

        masterVolumeSlider.SetValueWithoutNotify(currentMasterVolume);
        sfxVolumeSlider.SetValueWithoutNotify(currentSFXVolume);
        musicVolumeSlider.SetValueWithoutNotify(currentMusicVolume);


        ChangeVolume(currentMasterVolume, "MasterVolume");
        ChangeVolume(currentMusicVolume, "MusicVolume");
        ChangeVolume(currentSFXVolume, "SFXVolume");
        
        invertXToggle.SetIsOnWithoutNotify(currentInvertX);
        invertYToggle.SetIsOnWithoutNotify(currentInvertY);

        if (CameraManager.instance != null)
        {
            CameraManager.instance.SetSensitivity(currentHorSens, currentVertSens);
            CameraManager.instance.SetInversion(currentInvertX, currentInvertY);
        }
        
        postProcessingToggle.SetIsOnWithoutNotify(currentPP);
        vsyncToggle.SetIsOnWithoutNotify(currentVsync);
            
        if (currentVsync)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
        
        if (PostProcessHandler.instance != null) PostProcessHandler.instance.SetPP(currentPP);
        
        ApplySettings();
    }
}

[System.Serializable]
public class SettingsInfo
{
    public bool appliedFullscreen = false;
    public int appliedResolution = 0;
    public int appliedFPS = 0;
    public float appliedMasterVolume = 0.5f;
    public float appliedMusicVolume = 0.5f;
    public float appliedSFXVolume = 0.5f;
    public float appliedHorSens = 1.5f;
    public float appliedVertSens = 0.025f;
    public bool appliedPP = true;
    public bool appliedVsync = false;
    public bool appliedInvertX = false;
    public bool appliedInvertY = false;

    public SettingsInfo(bool fullscreen, int resolution, float masterVolume, float musicVolume, float sfxVolume, float mouseSensX,float mouseSensY, bool pp, int fps, bool invertX, bool invertY, bool vsync)
    {
        appliedFullscreen = fullscreen;
        appliedResolution = resolution;
        appliedMasterVolume = masterVolume;
        appliedMusicVolume = musicVolume;
        appliedSFXVolume = sfxVolume;
        appliedPP = pp;
        appliedFPS = fps;
        appliedVertSens = mouseSensY;
        appliedHorSens = mouseSensX;
        appliedInvertX = invertX;
        appliedInvertY = invertY;
        appliedVsync = vsync;
    }
}
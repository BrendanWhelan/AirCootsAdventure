using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickAnythingStart : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;

    private bool fading = false;
    
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (scene.name.Equals("TitleScreen")) return;
        fadeImage.DOFade(0f, 1f).OnComplete(FinishedFadingIn);
    }

    void Update()
    {
        if (fading) return;
        if (Input.anyKeyDown)
        {
            fading = true;
            GameManager.instance.PauseGame(true);
            fadeImage.DOFade(1f, 1f).OnComplete(LoadLevel);
            AudioManager.instance.FadeCurrentSong();
            AudioManager.instance.PlaySound(Sounds.MenuClick);
        }
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void FinishedFadingIn()
    {
        GameManager.instance.PauseGame(false);
        SceneManager.sceneLoaded -= SceneLoaded;
        Destroy(gameObject);
    }
}

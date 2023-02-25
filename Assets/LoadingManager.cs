using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] redPortalSprites;

    [SerializeField]
    private Sprite[] purplePortalSprites;

    [SerializeField]
    private Sprite[] tealPortalSprites;

    [SerializeField]
    private Image loadingScreenImage;

    private int portalColor;

    public static LoadingManager instance;
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

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(int sceneID, int portal)
    {
        portalColor = portal;
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        Sprite[] portalSprites;

        if (portalColor == 0) portalSprites = redPortalSprites;
        else if (portalColor == 1) portalSprites = purplePortalSprites;
        else portalSprites = tealPortalSprites;
        
        loadingScreenImage.color = new Color(1, 1, 1, 0f);
        loadingScreenImage.sprite = portalSprites[0];
        loadingScreenImage.gameObject.SetActive(true);
        
        //Fade in screen
        float timePassed = 0f;
        int i = 0;
        while (timePassed < 1f)
        {
            loadingScreenImage.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, timePassed / 1f);
            timePassed += 0.016f;
            i++;
            if (i >= redPortalSprites.Length) i = 0;
            loadingScreenImage.sprite = portalSprites[i];
            yield return new WaitForSeconds(0.016f);
        }

        loadingScreenImage.color = Color.white;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        while (!operation.isDone)
        {
            i++;
            if (i >= redPortalSprites.Length) i = 0;
            loadingScreenImage.sprite = portalSprites[i];
            yield return new WaitForSeconds(0.016f);
        }

        timePassed = 0f;
        while (timePassed < 0.5f)
        {
            loadingScreenImage.color = Color.Lerp( Color.white,new Color(1, 1, 1, 0), timePassed / 0.5f);
            timePassed += 0.016f;
            i++;
            if (i >= redPortalSprites.Length) i = 0;
            loadingScreenImage.sprite = portalSprites[i];
            yield return new WaitForSeconds(0.016f);
        }
        
        loadingScreenImage.color = new Color(1, 1, 1, 0f);
        loadingScreenImage.gameObject.SetActive(false);
        GameManager.instance.PauseGame(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    private string currentSavePath;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
            return;
        }

        instance = this;
        currentSavePath = Application.persistentDataPath + "/Saves/";
    }

    public void SaveSettings()
    {
        if (SettingsManager.instance != null)
        {
            try
            {
                SettingsInfo settingsInfo = SettingsManager.instance.SaveSettings();

                BinaryFormatter bf = new BinaryFormatter();
                if (!Directory.Exists(currentSavePath))
                {
                    Directory.CreateDirectory(currentSavePath);
                }

                FileStream stream = new FileStream(currentSavePath + "Settings.dat", FileMode.Create);

                bf.Serialize(new BufferedStream(stream), settingsInfo);

                stream.Close();

                Debug.Log("Settings Save Successful");
            }
            catch
            {
                Debug.LogWarning("Issue saving Settings");
            }
        }
        else
        {
            Debug.LogWarning("No Settings Manager");
        }
    }
    
    public void LoadSettings()
    {
        SettingsInfo settingsInfo = null;

        try
        {
            if (File.Exists(currentSavePath + "Settings.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(currentSavePath + "Settings.dat", FileMode.Open);

                settingsInfo = (SettingsInfo)bf.Deserialize(new BufferedStream(stream));
                
                stream.Close();
            }
            else
            {
                Debug.Log("Save Path For Settings Doesn't Exist. Using Defaults.");
            }
        }
        catch
        {
            Debug.LogWarning("Issue loading options");
        }
        
        SettingsManager.instance.LoadSettings(settingsInfo);
    }

    public void LoadGame()
    {
        GameInfo gameInfo = null;

        try
        {
            if (File.Exists(currentSavePath + "Game.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(currentSavePath + "Game.dat", FileMode.Open);

                gameInfo = (GameInfo)bf.Deserialize(new BufferedStream(stream));
                
                stream.Close();
            }
            else
            {
                Debug.Log("Save Path For Game Doesn't Exist. Using Defaults.");
            }
        }
        catch
        {
            Debug.LogWarning("Issue loading game");
        }
        
        GameManager.instance.LoadGame(gameInfo);
    }

    public void SaveGame()
    {
        if (GameManager.instance != null)
        {
            try
            {
                GameInfo gameInfo = GameManager.instance.SaveGame();

                BinaryFormatter bf = new BinaryFormatter();
                if (!Directory.Exists(currentSavePath))
                {
                    Directory.CreateDirectory(currentSavePath);
                }

                FileStream stream = new FileStream(currentSavePath + "Game.dat", FileMode.Create);

                bf.Serialize(new BufferedStream(stream), gameInfo);

                stream.Close();

                Debug.Log("Game Save Successful");
            }
            catch
            {
                Debug.LogWarning("Issue saving Game");
            }
        }
        else
        {
            Debug.LogWarning("No Game Manager");
        }
    }
}

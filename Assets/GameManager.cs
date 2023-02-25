using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        pauseEvent = new UnityEvent<bool>();
        playerDeathEvent = new UnityEvent();
        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.sceneLoaded += SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("Main Scene"))
        {
            for (int i = 0; i < portalsEnabled.Length; i++)
            {
                if(portalsEnabled[i])
                    PortalHandler.instance.EnablePortal(i);
            }
        }
    }
    private void SceneUnloaded(Scene scene)
    {
        if (scene.buildIndex is 0 or 1)
            previousLevel = 0;
        else
            previousLevel = scene.buildIndex-1;
        pauseEvent = new UnityEvent<bool>();
        playerDeathEvent = new UnityEvent();
        AudioManager.instance.KillSounds();
    }

    private int previousLevel = 0;
    
    [SerializeField]
    private Vector3[] playerHubStartPositions;

    private bool[] portalsEnabled = new bool[]
    {
        false,
        false,
        false
    };
    
    private bool[] dialogueChecks = new bool[]
    {
        false, //qt start
        false, //qt finish
        false, //yard start
        false, //yard finish
        false, //lud start
        false, //lud finish
        false,  //Slime
        false //Final
    };

    private bool[] objectiveChecks = new bool[]
    {
        false,//cake
        false,//slime
        false,//bidet
        false//finish
    };

    private bool[] partyChecks = new bool[]
    {
        false,//cake
        false,//slime
        false,//bidet
        false//Finished 
    };

    private bool[] levelOneCoins = new bool[15]
    {
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false
    };
    
    private bool[] levelTwoCoins = new bool[15]
    {
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false
    };
    
    private bool[] levelThreeCoins = new bool[15]
    {
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false,
        false
    };

    private void Start()
    {
        SaveLoadManager.instance.LoadGame();
        if (SceneManager.GetActiveScene().name.Equals("Main Scene"))
        {
            for (int i = 0; i < portalsEnabled.Length; i++)
            {
                if(portalsEnabled[i])
                    PortalHandler.instance.EnablePortalNoSave(i);
            }
        }
    }

    private TimeSpan[] levelTimes = new[]
    {
        TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero
    };
    
    private bool gamePaused = false;
    public bool GamePaused { get { return gamePaused; } }

    public void PauseGame(bool paused)
    {
        gamePaused = paused;
        pauseEvent?.Invoke(paused);
    }

    private UnityEvent<bool> pauseEvent;
    public void SubscribeToPauseEvent(UnityAction<bool> action)
    {
        pauseEvent.AddListener(action);
    }

    private UnityEvent playerDeathEvent;
    public void SubscribeToPlayerDeathEvent(UnityAction action)
    {
        playerDeathEvent.AddListener(action);
    }
    
    public void PlayerDied()
    {
        playerDeathEvent?.Invoke();
    }

    public Vector3 GetPlayerHubStartPosition()
    {
        return playerHubStartPositions[previousLevel];
    }
    
    
    //Check if the dialogue requirements are met. AKA, conversation hasnt been had, and any necessary objective is met
    public bool TryDialogue(DialogueRequirement requirement)
    {
        if (!dialogueChecks[requirement.dialogueToCheck])
        {
            if (requirement.objectiveToCheck >= 0)
            {
                if (objectiveChecks[requirement.objectiveToCheck])
                {
                    dialogueChecks[requirement.dialogueToCheck] = true;
                    SaveLoadManager.instance.SaveGame();
                    return true;
                }
            }
            else
            {
                dialogueChecks[requirement.dialogueToCheck] = true;
                return true;
            }
        }
        
        return false;
    }

    public void SetPortalEnabled(int portal)
    {
        portalsEnabled[portal] = true;
        SaveLoadManager.instance.SaveGame();
    }
    
    public bool GetObjective(int objective)
    {
        return objectiveChecks[objective];
    }

    public void SetObjective(int objective, bool obtained)
    {
        objectiveChecks[objective] = obtained;
        if (objectiveChecks[0] && objectiveChecks[1] && objectiveChecks[2]) objectiveChecks[3] = true;
        SaveLoadManager.instance.SaveGame();
    }

    public bool GetPartyObjective(int objective)
    {
        return partyChecks[objective];
    }

    public void SetPartyObjective(int objective)
    {
        partyChecks[objective] = true;
        if (partyChecks[0] && partyChecks[1] && partyChecks[2])
        {
            partyChecks[3] = objectiveChecks[3] = true;
        }
        SaveLoadManager.instance.SaveGame();
        if(PartyHandler.instance != null) PartyHandler.instance.UpdateParty();
    }

    public TimeSpan GetLevelTime(int level)
    {
        return levelTimes[level-1];
    }

    public void SetLevelTime(TimeSpan time, int level)
    {
        if (levelTimes[level-1] == TimeSpan.Zero || levelTimes[level-1] > time)
        {
            levelTimes[level-1] = time;
        }
        SaveLoadManager.instance.SaveGame();
    }

    public void SetCoinsExact(int level, bool[] coins)
    {
        switch (level)
        {
            case 1:
                levelOneCoins = coins;
                break;
            case 2:
                levelTwoCoins = coins;
                break;
            case 3:
                levelThreeCoins = coins;
                break;
        }
        SaveLoadManager.instance.SaveGame();
    }
    
    public int GetCoins(int level)
    {
        return level switch
        {
            1 => CountBool(levelOneCoins),
            2 => CountBool(levelTwoCoins),
            3 => CountBool(levelThreeCoins),
            _ => 0
        };
    }

    public bool[] GetExactCoinBool(int level)
    {
        return level switch
        {
            1 => levelOneCoins,
            2 => levelTwoCoins,
            _ => levelThreeCoins
        };
    }
    
    private int CountBool(bool[] array)
    {
        return array.Count(count => count);
    }

    public bool AllObjectivesFinished()
    {
        if (!objectiveChecks[0] || !objectiveChecks[1] || !objectiveChecks[2]) return false;
        objectiveChecks[3] = true;
        SaveLoadManager.instance.SaveGame();
        return true;
    }

    public void ReturnToHub()
    {
        AudioManager.instance.PlaySound(Sounds.Portal);
        AudioManager.instance.FadeCurrentSong();
        PauseGame(true);
        int level = SceneManager.GetActiveScene().buildIndex;
        LoadingManager.instance.LoadScene(1,level-2);
    }

    public void RestartLevel()
    {
        AudioManager.instance.PlaySound(Sounds.Portal);
        AudioManager.instance.FadeCurrentSong();
        PauseGame(true);
        int level = SceneManager.GetActiveScene().buildIndex;
        LoadingManager.instance.LoadScene(level,level-2);
    }

    public void LoadGame(GameInfo gameInfo)
    {
        if (gameInfo != null)
        {
            dialogueChecks = gameInfo.dialogues;
            objectiveChecks = gameInfo.objectives;
            partyChecks = gameInfo.partyChecks;
            levelOneCoins = gameInfo.levelOneCoins;
            levelTwoCoins = gameInfo.levelTwoCoins;
            levelThreeCoins = gameInfo.levelThreeCoins;
            levelTimes = gameInfo.levelTimes;
            portalsEnabled = gameInfo.portalsEnabled;

            if (objectiveChecks[0] && objectiveChecks[1] && objectiveChecks[2] && objectiveChecks[3])
            {
                partyChecks[0] = partyChecks[1] = partyChecks[2] = partyChecks[3] = true;
            }
        }
    }
    
    public GameInfo SaveGame()
    {
        return new GameInfo(dialogueChecks, objectiveChecks, partyChecks, levelOneCoins, levelTwoCoins, levelThreeCoins, levelTimes, portalsEnabled);
    }
}

[System.Serializable]
public class GameInfo
{
    public bool[] portalsEnabled;
    public bool[] dialogues;
    public bool[] objectives;
    public bool[] partyChecks;
    public bool[] levelOneCoins;
    public bool[] levelTwoCoins;
    public bool[] levelThreeCoins;
    public TimeSpan[] levelTimes;

    public GameInfo(bool[] dialogueBools, bool[] objectiveChecks, bool[] party, bool[] levelOneCoins, bool[] levelTwoCoins, bool[] levelThreeCoins, TimeSpan[] times, bool[] portals)
    {
        portalsEnabled = portals;
        dialogues = dialogueBools;
        objectives = objectiveChecks;
        partyChecks = party;
        this.levelOneCoins = levelOneCoins;
        this.levelTwoCoins = levelTwoCoins;
        this.levelThreeCoins = levelThreeCoins;
        levelTimes = times;
    }
}

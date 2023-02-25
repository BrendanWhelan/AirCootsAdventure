using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private GameObject audioPlayerPrefab;

    //Only need one music source
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioMixer mixer;
    
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

    private List<AudioPlayer> audioPlayers;
    
    private Dictionary<Sounds, SoundData> soundClips;

    [SerializeField]
    private SoundClip[] sounds;
    
    private void Start()
    {
        audioPlayers = new List<AudioPlayer>();
        CreateMoreAudioPlayers();

        soundClips = new Dictionary<Sounds, SoundData>();

        foreach (SoundClip soundClip in sounds)
        {
            if (!soundClips.ContainsKey(soundClip.soundType))
            {
                soundClips.Add(soundClip.soundType,new SoundData(soundClip));
            }
        }
    }

    private AudioPlayer CreateMoreAudioPlayers()
    {
        AudioPlayer playerToReturn = Instantiate(audioPlayerPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<AudioPlayer>();
        audioPlayers.Add(playerToReturn);
        playerToReturn.gameObject.SetActive(false);

        for (int i = 0; i < 9; i++)
        {
            AudioPlayer player = Instantiate(audioPlayerPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<AudioPlayer>();
            audioPlayers.Add(player);
            playerToReturn.gameObject.SetActive(false);
        }

        return playerToReturn;
    }

    private AudioPlayer FindAvailableAudioPlayer()
    {
        foreach (AudioPlayer player in audioPlayers)
        {
            if (!player.gameObject.activeSelf)
            {
                return player;
            }
        }
        //No Free players
        return CreateMoreAudioPlayers();
    }


    public void PlaySound(Sounds sound)
    {
        SoundData clip = soundClips[sound];

        if (clip.soundCount >= clip.maxSounds) return;
        AudioPlayer player = FindAvailableAudioPlayer();
        
        player.PlayAudio(clip.sound);
        clip.soundCount++;
    }

    public void PlaySoundPositional(Sounds sound, Vector3 position)
    {
        SoundData clip = soundClips[sound];

        AudioPlayer player = FindAvailableAudioPlayer();
        
        player.transform.position = position;
        
        player.PlayAudioPositional(clip.sound);
    }
    
    public void StopSound(Sounds sound)
    {
        foreach (AudioPlayer player in audioPlayers)
        {
            if (player.gameObject.activeSelf && player.sound.soundType == sound)
            {
                player.StopAudio();
                return;
            }
        }
    }
    
    public void AudioFinished(SoundClip sound)
    {
        SoundData clip = soundClips[sound.soundType];
        clip.SubtractOne();
    }

    public void KillSounds()
    {
        foreach (AudioPlayer player in audioPlayers)
        {
            if(player.gameObject.activeSelf)
                player.StopAudio();
        }

        windSource.volume = 0f;
        intendedWindVolume = 0f;
    }
    
    private class SoundData
    {
        public SoundClip sound;
        public int soundCount = 0;
        public int maxSounds=6;

        public SoundData(SoundClip clip)
        {
            sound = clip;
            maxSounds = clip.maxSounds;
            soundCount = 0;
        }

        public void SubtractOne()
        {
            soundCount--;
            if (soundCount < 0) soundCount = 0;
        }
    }

    private AudioClip songToPlay;
    private float volumeToPlayAt=1f;

    public void FadeCurrentSong()
    {
        musicSource.DOFade(0f, 1f);
    }
    public void PlaySong(AudioClip song, float volume)
    {
        songToPlay = song;
        volumeToPlayAt = volume;
        musicSource.volume = 0f;
        musicSource.clip = songToPlay;
        musicSource.Play();
        musicSource.DOFade(volumeToPlayAt, 1f);
    }

    [SerializeField]
    private AudioSource windSource;

    private float intendedWindVolume = 0f;
    
    public void AdjustWindVolume(float volume)
    {
        intendedWindVolume = volume;
    }

    private void Update()
    {
        windSource.volume = Mathf.Lerp(windSource.volume, intendedWindVolume, Time.deltaTime);
    }
}

public enum Sounds
{
    BlueNodeActive,
    BlueNodeDeactivate,
    RedRingSwap,
    CannonShoot,
    ItemPickup,
    Crawl,
    Footstep,
    Jump,
    JumpPad,
    Sliding,
    Grapple,
    Death,
    Respawn,
    Portal,
    MenuClick
}

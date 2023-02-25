using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySongOnSceneStart : MonoBehaviour
{
    [SerializeField]
    private AudioClip songClip;
    [SerializeField]
    private float volume=1f;

    private void Start()
    {
        AudioManager.instance.PlaySong(songClip,volume);
    }
}

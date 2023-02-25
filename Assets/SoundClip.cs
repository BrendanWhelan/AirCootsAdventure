using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/SoundClip")]
public class SoundClip : ScriptableObject
{
    public AudioClip sound;
    public Sounds soundType;
    public float volume = 1;
    public int maxSounds = 6;
    public bool randomPitch = false;
    public bool loop = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    public SoundClip sound;

    public void PlayAudio(SoundClip clip)
    {
        gameObject.SetActive(true);
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;
        if (clip.randomPitch)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
        else
        {
            audioSource.pitch = 1f;
        }
        sound = clip;
        if (clip.loop)
        {
            audioSource.loop = true;
            audioSource.volume = clip.volume;
            audioSource.clip = clip.sound;
            audioSource.Play();
        }
        else
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(clip.sound, clip.volume);
            Invoke(nameof(AudioFinished), clip.sound.length);
        }
    }

    public void PlayAudioPositional(SoundClip clip)
    {
        gameObject.SetActive(true);
        audioSource.spatialBlend = 1f;
        audioSource.dopplerLevel = 1f;
        if (clip.randomPitch)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
        }
        else
        {
            audioSource.pitch = 1f;
        }
        sound = clip;
        if (clip.loop)
        {
            audioSource.loop = true;
            audioSource.volume = clip.volume;
            audioSource.clip = clip.sound;
            audioSource.Play();
        }
        else
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(clip.sound, clip.volume);
            Invoke(nameof(AudioFinished), clip.sound.length);
        }
    }
    
    public void StopAudio()
    {
        audioSource.Stop();
        CancelInvoke();
        AudioFinished();
    }

    private void AudioFinished()
    {
        AudioManager.instance.AudioFinished(sound);
        sound = null;
        gameObject.SetActive(false);
    }
}

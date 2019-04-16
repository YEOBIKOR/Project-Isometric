using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioEngine : Single <AudioEngine>
{
    private AudioClip _musicClip;

    private List<AudioSource> _pooledAudios;
    private Transform _pooledObjectsowner;

    public AudioEngine() : base()
    {
        _pooledAudios = new List<AudioSource>();
        _pooledObjectsowner = new GameObject("Sound Objects").GetComponent<Transform>();
    }

    public static AudioSource PlaySound (AudioClip clip, bool loop = false, float volume = 1f, float pitch = 1f, float stereoPan = 0f)
    {
        AudioSource audioSource = Instance.GetAudioObject();

        audioSource.gameObject.name = clip.name;

        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.panStereo = stereoPan;

        audioSource.Play();

        return audioSource;
    }

    public static AudioSource PlayMusic (AudioClip clip, bool loop = true)
    {
        if (clip != Instance._musicClip)
        {
            Instance._musicClip = clip;
            return PlaySound(clip, loop);
        }

        else
            return null;
    }

    private AudioSource GetAudioObject()
    {
        foreach (var pooledAudio in _pooledAudios)
        {
            if (pooledAudio.isPlaying == false)
                return pooledAudio;
        }

        GameObject audioObject = new GameObject();
        audioObject.transform.parent = _pooledObjectsowner;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        _pooledAudios.Add(audioSource);

        return audioSource;
    }
}
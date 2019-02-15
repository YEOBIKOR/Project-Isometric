using System.Collections.Generic;
using UnityEngine;
using Custom;

public class WorldMicrophone
{
    private struct AudioInfo
    {
        public AudioSource audioSource;
        public IPositionable owner;
    }

    private List<AudioInfo> _playingAudios;

    private Vector3 _worldPosition;
    public Vector3 worldPosition
    {
        get
        { return _worldPosition; }
        set
        { _worldPosition = value; }
    }

    private float _viewAngle;
    public float viewAngle
    {
        get
        { return _viewAngle; }
        set
        { _viewAngle = value; }
    }

    public WorldMicrophone()
    {
        _playingAudios = new List<AudioInfo>();
    }

    public void Update(float deltaTime)
    {
        for (int index = 0; index < _playingAudios.Count; index++)
        {
            AudioSource audioSource = _playingAudios[index].audioSource;

            if (!audioSource.isPlaying)
            {
                _playingAudios.RemoveAt(index--);
                break;
            }

            IPositionable owner = _playingAudios[index].owner;

            audioSource.panStereo = GetPan(owner);
            audioSource.volume = GetVolume(owner);
        }
    }

    public float GetPan(IPositionable owner)
    {
        Vector3 delta = owner.worldPosition - _worldPosition;
        return CustomMath.Rotate(new Vector2(delta.x, delta.z), _viewAngle).normalized.x;
    }

    public float GetVolume(IPositionable owner)
    {
        return Mathf.Pow(1.5f, -(owner.worldPosition - _worldPosition).magnitude);
    }

    public void PlaySound(AudioClip clip, IPositionable owner)
    {
        AudioSource audioSource = AudioEngine.PlaySound(clip);

        AudioInfo playingAudio;
        playingAudio.audioSource = audioSource;
        playingAudio.owner = owner;

        _playingAudios.Add(playingAudio);
    }
}
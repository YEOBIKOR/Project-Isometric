using UnityEngine;

public class WorldMicrophone
{
    public void Update(float deltaTime)
    {

    }

    public void PlaySound(AudioClip clip, IPositionable owner)
    {
        AudioEngine.PlaySound(clip);
    }
}
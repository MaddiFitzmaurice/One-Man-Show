using UnityEngine;

[System.Serializable]

public class AudioClipData
{
    public AudioClip clip;
    [Range(0, 1)] public float volume;

    public AudioClipData(AudioClip clip, float volume)
    {
        this.clip = clip;
        this.volume = volume;
    }
}

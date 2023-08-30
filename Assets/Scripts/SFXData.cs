using UnityEngine;

public struct SFXData
{
    public AudioClip Clip { get; private set; }
    public StageDirection Dir { get; private set; }

    public SFXData(AudioClip clip, StageDirection dir)
    {
        Clip = clip;
        Dir = dir;
    }
}

using UnityEngine;

[System.Serializable]
public class SFXData
{
	public AudioClip clip;
	public StageDirection dir;

    public SFXData(AudioClip clip, StageDirection dir)
    {
        this.clip = clip;
        this.dir = dir;
    }
}

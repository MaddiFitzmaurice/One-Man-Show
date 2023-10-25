using UnityEngine;

[System.Serializable]
public class SFXData
{
    public AudioClipData clipData;
	public StageDirection dir;

    public SFXData(AudioClipData clipData, StageDirection dir)
    {
        this.clipData = clipData;
        this.dir = dir;
    }
}

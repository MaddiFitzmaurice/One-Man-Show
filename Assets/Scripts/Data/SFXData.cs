using UnityEngine;

[System.Serializable]
public struct SFXData
{
	[SerializeField] private AudioClip _clip;
	[SerializeField] private StageDirection _dir;

	public AudioClip Clip { get => _clip; }
    public StageDirection Dir { get => _dir; }

    public SFXData(AudioClip clip, StageDirection dir)
    {
        _clip = clip;
        _dir = dir;
    }
}

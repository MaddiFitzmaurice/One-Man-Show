using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
	public AudioMixerGroup mix_group;
	public string volume_param_name;

	public float initial_volume = 0; // In dB 0 is normal

	private void Start()
	{
		EventManager.EventTrigger(EventType.VOLUME_SET, initial_volume);
	}

	void OnEnable()
	{
		EventManager.EventSubscribe(EventType.VOLUME_SET, SetVolume);
	}

	void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.VOLUME_SET, SetVolume);
	}

	void SetVolume(object data)
	{
		if (data == null) return;

		mix_group.audioMixer.SetFloat
		(
			volume_param_name,
			(float)data
		);
	}
}

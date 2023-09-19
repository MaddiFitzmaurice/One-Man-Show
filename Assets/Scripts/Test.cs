using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public SongMeta song;
	public AudioSource clickSource;

	private void Start()
	{
		//SongManager.StartSong(song);

		EventManager.EventSubscribe(EventType.BEAT, OnBeat);
	}

	private void OnDestroy()
	{
		EventManager.EventUnsubscribe(EventType.BEAT, OnBeat);
	}

	void OnBeat(object data)
	{
		if ((int)data < 0) return;

		clickSource.Play();
	}
}

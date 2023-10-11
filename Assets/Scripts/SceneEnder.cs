using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SceneEnder	 : MonoBehaviour
{
	public GameObject win_fader;
	public GameObject death_fader;

	public float min_volume;
	public float max_volume;

	public AnimationCurve volume_curve;

	[Min(0)]
	public float switch_delay = 1.0f;

	public string end_scene;
	public string death_scene;

	private void Start()
	{
		EventManager.EventSubscribe(EventType.SONG_END, OnSongEnd);
		EventManager.EventSubscribe(EventType.PLAYER_DIED, OnPlayerDied);
	}

	private void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.SONG_END, OnSongEnd);
		EventManager.EventUnsubscribe(EventType.PLAYER_DIED, OnPlayerDied);
	}

	private void OnSongEnd(object data)
	{
		win_fader.SetActive(true);
		StartCoroutine(SwitchScene(Time.time, end_scene));
	}

	private void OnPlayerDied(object data)
	{
		death_fader.SetActive(true);
		StartCoroutine(SwitchScene(Time.time, death_scene));
	}

	private IEnumerator SwitchScene(float start_time, string scene)
	{
		while (Time.time - start_time < switch_delay)
		{
			EventManager.EventTrigger
			(
				EventType.VOLUME_SET,
				Mathf.Lerp
				(
					min_volume,
					max_volume,
					volume_curve.Evaluate((Time.time - start_time) / switch_delay)
				)
			);
			yield return null;
		}

		SceneManager.LoadScene(scene);
	}
}

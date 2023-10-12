using System.Collections;
using UnityEngine;

public class SceneEnder	 : MonoBehaviour
{
	public SceneSwitcher winSwitcher;
	public SceneSwitcher deathSwitcher;

	public float minVolume;
	public float maxVolume;

	public AnimationCurve volumeCurve;
	public float muteDelay;

	public string winScene;
	public string deathScene;

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
		winSwitcher.SetScene(winScene);
		StartCoroutine(FadeVolume(Time.time));
	}

	private void OnPlayerDied(object data)
	{
		winSwitcher.SetScene(deathScene);
		StartCoroutine(FadeVolume(Time.time));
	}

	private IEnumerator FadeVolume(float start_time)
	{
		while (Time.time - start_time < muteDelay)
		{
			EventManager.EventTrigger
			(
				EventType.VOLUME_SET,
				Mathf.Lerp
				(
					minVolume,
					maxVolume,
					volumeCurve.Evaluate((Time.time - start_time) / muteDelay)
				)
			);
			yield return null;
		}
	}
}

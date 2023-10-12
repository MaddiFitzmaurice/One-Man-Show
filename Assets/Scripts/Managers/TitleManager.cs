using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SceneSwitcher))]
public class TitleManager : MonoBehaviour
{
	SceneSwitcher switcher;
	[SerializeField]
	float quitDelay = 1f;

	private void Awake()
	{
		// If using the Unity editor or development build, enable debug logs
		Debug.unityLogger.logEnabled = Debug.isDebugBuild;
		switcher = GetComponent<SceneSwitcher>();
	}

	public void QuitGame()
	{
		switcher.StartFade();
#if UNITY_EDITOR
		this.Invoke(() => EditorApplication.isPlaying = false, quitDelay);
#else
		this.Invoke(() => Application.Quit(), quitDelay);
#endif
	}
}

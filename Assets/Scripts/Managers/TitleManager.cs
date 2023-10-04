using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleManager : MonoBehaviour
{
    public void StartGame()
	{
		SceneManager.LoadScene("MainScene");
	}

	public void StartTutorial()
	{
		SceneManager.LoadScene("TutorialScene");
	}

	public void StartCalibration()
	{
		SceneManager.LoadScene("CalibrationScene");
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

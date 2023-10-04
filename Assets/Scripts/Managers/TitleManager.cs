using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
}

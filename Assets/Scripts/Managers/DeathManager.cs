using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
	public string playSceneName;
	public string menuSceneName;

	public void Replay()
	{
		SceneManager.LoadScene(playSceneName);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene(menuSceneName);
	}
}

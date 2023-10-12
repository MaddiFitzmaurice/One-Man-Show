using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	private bool _isSwitching = false;

	[SerializeField]
	GameObject fader;
	[SerializeField, Min(0)]
	float switchDelay;

	public void SetScene(string sceneName)
	{
		if (_isSwitching) return;

		_isSwitching = true;

		StartFade();
		this.Invoke(() => SceneManager.LoadScene(sceneName), switchDelay);
	}

	public void StartFade()
	{
		fader.SetActive(true);
	}
}

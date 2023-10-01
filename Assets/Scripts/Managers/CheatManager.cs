using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CheatManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
		{
            SceneManager.LoadScene("TitleScene");
		}
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("TutorialScene");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}

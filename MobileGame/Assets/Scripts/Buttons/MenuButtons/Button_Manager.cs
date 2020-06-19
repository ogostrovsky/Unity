using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Manager : Singleton<Button_Manager>
{
    public void onSetScene(string sceneName)
    {
        Debug.Log("The scene is on load: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void onGameExit()
    {
        Application.Quit();
    }
}

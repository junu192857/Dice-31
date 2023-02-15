using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private string settingsSceneName;
    [SerializeField] private string tutorialSceneName;

    public void HandleStartClick()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    public void HandleQuitClick()
    {
        Quit();
    }

    public void HandleTutorialClick()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
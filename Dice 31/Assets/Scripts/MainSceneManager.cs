using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private string settingsSceneName;
    [SerializeField] private string tutorialSceneName;
    [SerializeField] private GameObject tableCamera;
    [SerializeField] private Text gameModeText;
    private Camera mainCamera;
    private static readonly int StartGame = Animator.StringToHash("StartGame");

    public void HandleStartClick()
    {
        var animator = tableCamera.GetComponent<Animator>();
        animator.SetTrigger(StartGame);
    }
    
    public void HandleAnimationEnd()
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

    public void SetGameMode() {
        if (GameManager.gameMode == GameMode.Drag)
        {
            gameModeText.text = "One-Click";
            GameManager.gameMode = GameMode.OneClick;
        }
        else {
            gameModeText.text = "Drag & Drop";
            GameManager.gameMode = GameMode.Drag;
        }
    }

    private void Start()
    {
        gameModeText.text = "Drag & Drop";
        GameManager.gameMode = GameMode.Drag;
    }
}
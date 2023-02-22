using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum PauseMenuState
{
    MainMenu,
    SettingsMenu
}

public class PausePanelManager : MonoBehaviour
{
    private PauseMenuState state = PauseMenuState.MainMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;
    
    private void Start()
    {
        ShowMainMenu();
        Time.timeScale = 0;
    }
    
    public void ShowMainMenu()
    {
        state = PauseMenuState.MainMenu;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ShowSettingsMenu()
    {
        state = PauseMenuState.SettingsMenu;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        InitSettingsUI();
    }

    private void InitSettingsUI()
    {
        sfxSlider.value = GameManager.Inst.sm.SFXVolume;
        bgmSlider.value = GameManager.Inst.sm.BGMVolume;
    }
    
    public void UpdateSFXVolume()
    {
        GameManager.Inst.sm.SFXVolume = sfxSlider.value;
    }
    
    public void UpdateBGMVolume()
    {
        GameManager.Inst.sm.BGMVolume = bgmSlider.value;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
    }
    
    public void BackToMain()
    {
        Time.timeScale = 1;
        GameManager.Destroy();
        Destroy(gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene("joongwon_MainScene");
    }

    public void TestSFX()
    {
        Debug.Log("Test SFX - volume: " + GameManager.Inst.sm.SFXVolume);
        GameManager.Inst.sm.LaserShootingSound();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (state)
            {
                case PauseMenuState.MainMenu:
                    ResumeGame();
                    break;
                case PauseMenuState.SettingsMenu:
                    ShowMainMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

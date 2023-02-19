using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SetupSceneManager : MonoBehaviour
{
    public static string[] playerNames =
    {
        "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7", "Player8"
    };

    [SerializeField] private GameObject tableCamera;
    [SerializeField] private GameObject inputs;
    [SerializeField] private Text rollModeText;
    private static readonly int StartGame = Animator.StringToHash("StartGame");
    private static readonly int Back = Animator.StringToHash("Back");

    private void Start()
    {
        for (var i = 0; i < inputs.transform.childCount; i++)
        {
            var input = inputs.transform.GetChild(i).GetComponent<InputField>();
            input.text = playerNames[i];
        }

        rollModeText.text = "Drag & Drop";
        GameManager.gameMode = GameMode.Drag;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected.transform.IsChildOf(inputs.transform))
            {
                var index = selected.transform.GetSiblingIndex();
                if (index == 0)
                    index = inputs.transform.childCount;
                var nextInput = inputs.transform.GetChild(index - 1);
                nextInput.GetComponent<Selectable>().Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected.transform.IsChildOf(inputs.transform))
            {
                var index = selected.transform.GetSiblingIndex();
                if (index == inputs.transform.childCount - 1)
                    index = -1;
                var nextInput = inputs.transform.GetChild(index + 1);
                nextInput.GetComponent<Selectable>().Select();
            }
        }
    }

    public void HandleStartClick()
    {
        playerNames = new string[8];
        for (var i = 0; i < inputs.transform.childCount; i++)
        {
            var text = inputs.transform.GetChild(i).GetComponent<InputField>().text;
            if (text.Length == 0)
                playerNames[i] = "Player" + (i + 1);
            else
                playerNames[i] = text;
        }
        tableCamera.GetComponent<Animator>().SetTrigger(StartGame);
    }

    public void HandleAnimationEnd()
    {
        SceneManager.LoadScene("JunwooUI");
    }

    public void HandleBackToMainAnimationEnd()
    {
        SceneManager.LoadScene("joongwon_MainScene");
    }

    public void HandleBackClick()
    {
        tableCamera.GetComponent<Animator>().SetTrigger(Back);
    }

    public void ChangeRollMode(){
        if (GameManager.gameMode == GameMode.Drag)
        {
            GameManager.gameMode = GameMode.OneClick;
            rollModeText.text = "One-click";
        }
        else {
            GameManager.gameMode = GameMode.Drag;
            rollModeText.text = "Drag & Drop";
        }
    }

}

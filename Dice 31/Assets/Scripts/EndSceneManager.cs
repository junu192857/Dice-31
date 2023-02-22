using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    public static string[] winnerNames;
    public static Sprite[] winnerSprites;
    public static string winnerTeam;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Text winnerText;

    private void Start()
    {
        winnerText.text = winnerTeam.Equals("Red") ? "<color=red>Red</color> Win!" : "<color=blue>Blue</color> Win!";
        for (var i = 0; i < winnerSprites.Length; i++)
        {
            var player = playerPanel.transform.GetChild(i).gameObject;
            player.GetComponent<Image>().sprite = winnerSprites[i];
            player.transform.GetChild(0).GetComponent<Text>().text = winnerNames[i];
        }
    }
    
    public void HandleBackToMain()
    {
        SceneManager.LoadScene("joongwon_MainScene");
    }
    public void HandleBackToPlay() {
        SceneManager.LoadScene("JunwooUI");
    }
}

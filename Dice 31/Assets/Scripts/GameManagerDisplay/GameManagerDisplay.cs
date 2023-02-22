#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerDisplay : EditorWindow
{
    public static bool AutoPlay;
    public static bool AlwaysThrow;

    [MenuItem("Window/UI Toolkit/GameManagerDisplay")]
    public static void ShowExample()
    {
        GetWindow<GameManagerDisplay>();
    }

    private bool[] toggles;
    private string corruptStack;

    private void OnGUI()
    {
        var gameManager = GameManager.Inst;
        if (gameManager is null)
        {
            GUILayout.Box("No GameManager found in the scene.");
            return;
        }

        GUILayout.Label("주사위 소유 현황");
        var players = gameManager.pm.playerInfos;
        if (toggles is null || toggles.Length != players.Count)
            toggles = new bool[players.Count];
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            GUILayout.BeginHorizontal();
            toggles[i] = GUILayout.Toggle(toggles[i], "선택");
            GUILayout.Label(player.playerName);
            if (player.specialDice is null)
                GUILayout.Label("?");
            else
                GUILayout.Label(player.specialDice.diceName ?? "(특수 주사위 없음)");
            player.isBot = GUILayout.Toggle(player.isBot, "Bot");
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("주사위 교환"))
        {
            HandleOnDiceChanged(players, gameManager.pm.activatedPlayer);
        }
        
        GUILayout.BeginHorizontal();
        AutoPlay = GUILayout.Toggle(AutoPlay, "자동 플레이");
        AlwaysThrow = GUILayout.Toggle(AlwaysThrow, "봇은 무지성 굴리기");
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("강제로 엔딩 씬 불러오기"))
        {
            GameManager.Inst.pm.SaveInfoAndLoadEndScene();
        }

        if (GUILayout.Button("전부 봇으로 바꾸기"))
        {
            foreach (var player in players)
            {
                player.isBot = true;
            }
        }
        
        if (GUILayout.Button("전부 사람으로 바꾸기"))
        {
            foreach (var player in players)
            {
                player.isBot = false;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("타락 스택: " + gameManager.pm.corruptStack);
        corruptStack = GUILayout.TextField(corruptStack);
        if (corruptStack != gameManager.pm.corruptStack.ToString())
        {
            if (GUILayout.Button("수정"))
            {
                try
                {
                    gameManager.pm.corruptStack = int.Parse(corruptStack);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    private void HandleOnDiceChanged(List<Player> players, Player activatedPlayer)
    {
        var selectedPlayers = new List<Player>();
        for (var i = 0; i < toggles.Length; i++)
        {
            if (toggles[i]) selectedPlayers.Add(players[i]);
        }

        if (selectedPlayers.Count != 2)
        {
            Debug.Log("주사위 교환은 두 명의 플레이어가 필요합니다.");
            return;
        }

        if (selectedPlayers.Any(player => player == activatedPlayer))
        {
            Debug.Log("주사위 교환은 활성화된 플레이어가 포함될 수 없습니다.");
            return;
        }

        (selectedPlayers[0].specialDice, selectedPlayers[1].specialDice) =
            (selectedPlayers[1].specialDice, selectedPlayers[0].specialDice);
    }
}
#endif
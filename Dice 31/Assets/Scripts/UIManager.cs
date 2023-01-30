using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{














    //테스트 및 프로토타입 배포용 UI를 만들기 위해 임시로 작성한 부분
    public Text MatchRoundCount;
    public Text NumberCount;
    public Text SpecialDiceInfos;
    public Text Player1Info;
    public Text Player2Info;
    public Text Player3Info;
    public Text Player4Info;
    public Text Player5Info;
    public Text Player6Info;
    public Text Player7Info;
    public Text Player8Info;
    public void UpdateUI() {
        MatchRoundCount.text = $"Match {GameManager.Inst.pm.matchCount} Round {GameManager.Inst.pm.roundCount}";
        NumberCount.text = $"Number: {GameManager.Inst.pm.curCount} / {GameManager.Inst.pm.maxCount}";
        SpecialDiceInfos.text = $"Bomb Dice Number: {GameManager.Inst.pm.bombDiceNum}\n" +
                                $"Assassin Dice Trigger: {GameManager.Inst.pm.assassinInfo}\n" +
                                $"Corrupt Stack: {GameManager.Inst.pm.corruptStack}";
        Player1Info.text = $"Player 1\n" +
                           $"{GameManager.Inst.pm.playerInfos[0].team} Team / {GameManager.Inst.pm.playerInfos[0].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[0].specialDice.diceName}";
        Player2Info.text = $"Player 2\n" +
                           $"{GameManager.Inst.pm.playerInfos[1].team} Team / {GameManager.Inst.pm.playerInfos[1].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[1].specialDice.diceName}";
        Player3Info.text = $"Player 3\n" +
                           $"{GameManager.Inst.pm.playerInfos[2].team} Team / {GameManager.Inst.pm.playerInfos[2].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[2].specialDice.diceName}";
        Player4Info.text = $"Player 4\n" +
                           $"{GameManager.Inst.pm.playerInfos[3].team} Team / {GameManager.Inst.pm.playerInfos[3].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[3].specialDice.diceName}";
        Player5Info.text = $"Player 5\n" +
                           $"{GameManager.Inst.pm.playerInfos[4].team} Team / {GameManager.Inst.pm.playerInfos[4].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[4].specialDice.diceName}";
        Player6Info.text = $"Player 6\n" +
                           $"{GameManager.Inst.pm.playerInfos[5].team} Team / {GameManager.Inst.pm.playerInfos[5].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[5].specialDice.diceName}";
        Player7Info.text = $"Player 7\n" +
                           $"{GameManager.Inst.pm.playerInfos[6].team} Team / {GameManager.Inst.pm.playerInfos[6].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[6].specialDice.diceName}";
        Player8Info.text = $"Player 8\n" +
                           $"{GameManager.Inst.pm.playerInfos[7].team} Team / {GameManager.Inst.pm.playerInfos[7].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[7].specialDice.diceName}";
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.Inst.gsm.State == GameState.WaitingForInput)
        UpdateUI();
    }


}

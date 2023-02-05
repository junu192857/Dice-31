using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image NumberGauge;
    public Text NumberText;

    private int formerCurCount;
    private int updatedCurCount;
    private int formerMaxCount;
    private int updatedMaxCount;

    public Toggle NormalDiceToggle;
    public Toggle SpecialDiceToggle;

    //������ �ٸ� ������ �����̴� �ִϸ��̼�
    //TODO: Normal Dice, Plus Dice, Minus Dice�� ���ڰ� �̵��ϸ� �׶� Ʋ��� ��./
    public IEnumerator UpdateGaugeBar(int curCount, int maxCount, float duration) {

        var runTime = 0.0f;

        RectTransform rect = NumberGauge.GetComponent<RectTransform>();
        Image image = NumberGauge.GetComponent<Image>();

        Vector2 curWidth = new Vector2(rect.sizeDelta.x, 60);
        Vector2 targetWidth = new Vector2(480 * curCount / maxCount, 60);

        if (targetWidth.x < 240)
        {
            image.color = Color.green;
        }
        else if (targetWidth.x >= 240 && targetWidth.x < 360)
        {
            image.color = Color.yellow;
        }
        else {
            image.color = Color.red;
        }
        while (runTime < duration) {
            runTime += Time.deltaTime;
            rect.sizeDelta = Vector2.Lerp(curWidth, targetWidth, runTime / duration);
            yield return null;
        }
    }

    //Run whenever new player starts his turn
    public void UpdateDiceSelectPanel() {
        //TODO: ��� ��ư ���� �̹����� �÷��̾��� Ư�� �ֻ����� �°� ����

        NormalDiceToggle.interactable = false;
        NormalDiceToggle.isOn = true;
        SpecialDiceToggle.interactable = true;
        SpecialDiceToggle.isOn = false;
        if (GameManager.Inst.pm.activatedPlayer.specialDice is CorruptedDice)
        {
            SpecialDiceToggle.isOn = true;
            SpecialDiceToggle.interactable = false;
        }
        else if (!GameManager.Inst.pm.activatedPlayer.specialDice.available) {
            SpecialDiceToggle.interactable = false;
        }

        Debug.Log($"{GameManager.Inst.pm.activatedPlayer.specialDice.available}");
    }











    //�׽�Ʈ �� ������Ÿ�� ������ UI�� ����� ���� �ӽ÷� �ۼ��� �κ�
    public Text MatchRoundCount;
    public Text NumberCount;
    public Text SpecialDiceInfos;
    public Text TeamWinCount;
    public Text Player1Info;
    public Text Player2Info;
    public Text Player3Info;
    public Text Player4Info;
    public Text Player5Info;
    public Text Player6Info;
    public Text Player7Info;
    public Text Player8Info;
    public void UpdateUI() {
        /*MatchRoundCount.text = $"Match {GameManager.Inst.pm.matchCount} Round {GameManager.Inst.pm.roundCount}";
        NumberCount.text = $"Number: {GameManager.Inst.pm.curCount} / {GameManager.Inst.pm.maxCount}";
        SpecialDiceInfos.text = $"Bomb Dice Number: {GameManager.Inst.pm.bombDiceNum}\n" +
                                $"Assassin Dice Trigger: {GameManager.Inst.pm.assassinInfo}\n" +
                                $"Corrupt Stack: {GameManager.Inst.pm.corruptStack}";
        TeamWinCount.text = $"Red {GameManager.Inst.pm.winCount["Red"]} : {GameManager.Inst.pm.winCount["Blue"]} Blue";
        Player1Info.text = $"Player 1\n" +
                           $"{GameManager.Inst.pm.playerInfos[0].team} Team / {GameManager.Inst.pm.playerInfos[0].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[0].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[0])}";
        Player2Info.text = $"Player 2\n" +
                           $"{GameManager.Inst.pm.playerInfos[1].team} Team / {GameManager.Inst.pm.playerInfos[1].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[1].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[1])}";
        Player3Info.text = $"Player 3\n" +
                           $"{GameManager.Inst.pm.playerInfos[2].team} Team / {GameManager.Inst.pm.playerInfos[2].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[2].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[2])}";
        Player4Info.text = $"Player 4\n" +
                           $"{GameManager.Inst.pm.playerInfos[3].team} Team / {GameManager.Inst.pm.playerInfos[3].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[3].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[3])}";
        Player5Info.text = $"Player 5\n" +
                           $"{GameManager.Inst.pm.playerInfos[4].team} Team / {GameManager.Inst.pm.playerInfos[4].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[4].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[4])}";
        Player6Info.text = $"Player 6\n" +
                           $"{GameManager.Inst.pm.playerInfos[5].team} Team / {GameManager.Inst.pm.playerInfos[5].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[5].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[5])}";
        Player7Info.text = $"Player 7\n" +
                           $"{GameManager.Inst.pm.playerInfos[6].team} Team / {GameManager.Inst.pm.playerInfos[6].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[6].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[6])}";
        Player8Info.text = $"Player 8\n" +
                           $"{GameManager.Inst.pm.playerInfos[7].team} Team / {GameManager.Inst.pm.playerInfos[7].deadString}\n" +
                           $"Special Dice: {GameManager.Inst.pm.playerInfos[7].specialDice.diceName}\n" +
                           $"{SpecialDiceInfo(GameManager.Inst.pm.playerInfos[7])}";
        */

        UpdateNumberText(GameManager.Inst.pm.curCount, GameManager.Inst.pm.maxCount);
        UpdatePlayerPanel(GameManager.Inst.pm.activatedPlayer);
        
    }


    public void UpdateNumberText(int curCount, int maxCount) { 
        //RectTransform rect = NumberGauge.GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(480 * curCount / maxCount, 60);
        NumberText.text = $"{curCount} / {maxCount}";
    }

    public List<Player> alivePlayers;
    public List<GameObject> playerPanels;
    public GameObject currentPlayerPanel;
    public void UpdatePlayerPanel(Player activatedPlayer)
    {
        int i;
        for(i=0; i < 8; i++)
        {
            if (!alivePlayers[i].alive || alivePlayers[i]==null)
            {
                playerPanels[i].SetActive(true);
                alivePlayers.Remove(alivePlayers[i]);
            }
            if (alivePlayers[i].Equals(activatedPlayer))
            {
                currentPlayerPanel.transform.position = playerPanels[i].transform.position;
            }
        }

        // �����÷��̾� ǥ�ø� ���� - ���� ���� �������� ���ư�

    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!(GameManager.Inst.gsm.State == GameState.Waiting))
        {
            updatedCurCount = GameManager.Inst.pm.curCount;
            updatedMaxCount = GameManager.Inst.pm.maxCount;
            UpdateUI();

            if (formerCurCount != updatedCurCount || formerMaxCount != updatedMaxCount) {
                StartCoroutine(UpdateGaugeBar(updatedCurCount, updatedMaxCount, 0.5f));
                formerCurCount = updatedCurCount;
                formerMaxCount = updatedMaxCount;
            }
        }
    }


}

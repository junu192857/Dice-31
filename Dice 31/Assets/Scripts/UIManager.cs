using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<Image> PlayerImages;

    public List<Sprite> Dice2D;
    public Image NormalDiceImage;
    public Image SpecialDiceImage;

    public Text MatchRoundInfo;
    public Text BombNumberText;
    public Text CorruptedCountText;
    public Image NumberGauge;
    public Text NumberText;
    public Image BombHolder;
    public Image BowImage;
    public Image SwordImage;
    public Image GunImage;
    public Image CorruptedImage;


    public List<Sprite> PlayerStates;

    private int formerCurCount;
    private int updatedCurCount;
    private int formerMaxCount;
    private int updatedMaxCount;

    public Toggle NormalDiceToggle;
    public Toggle SpecialDiceToggle;

    public Button SelectOneButton;
    public Button SelectTwoButton;

    [SerializeField]
    private List<GameObject> Numbers;

    private float scaleDuration = 0.25f;
    private float moveDuration = 0.5f;

    private Color ActivatedColor = new Color(1f, 1f, 1f);
    private Color DeactivatedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);
    public void ShowNumberAnimate(GameObject dice, int number)
    {
        StartCoroutine(ShowNumber(dice, number));
    }
    private IEnumerator ShowNumber(GameObject dice, int number)
    {
        string dicename = dice.GetComponent<Dice>().diceName;
        bool isNormal;
        if (dicename == "Normal Dice")
        {
            isNormal = true;
        }
        else { 
            isNormal = false;
        }

        //GameManager.Inst.gsm.OperateAnimation();

        int numberIndex = CalculateIndex(dicename, number);

        Vector3 screenpoint = Camera.main.WorldToScreenPoint(dice.transform.position) + new Vector3(0, 120, 0);
        Vector3 target = Camera.main.ScreenToWorldPoint(screenpoint);
        GameObject numberSprite = Instantiate(Numbers[numberIndex], target, Quaternion.Euler(90f, 0f, 0f));

        float runtime = 0f;
        while (runtime < scaleDuration)
        {
            runtime += Time.deltaTime;
            numberSprite.transform.localScale = Vector3.Lerp(Vector3.zero, 0.1f * Vector3.one, runtime / scaleDuration);
            yield return null;
        }
        yield return new WaitForSeconds(1);

        if (dicename == "Normal Dice") DiceUtil.rollingNormal = false;
        else DiceUtil.rollingSpecial = false;

        yield return new WaitUntil(() => !DiceUtil.rollingNormal && !DiceUtil.rollingSpecial);
        StartCoroutine(MoveNumber(isNormal, numberSprite));
    }

    private int CalculateIndex(string diceName, int number) {
        bool corrupted = GameManager.Inst.pm.corruptStack == 4;
        switch (diceName) {
            case "Normal Dice":
                return corrupted ? number + 20 : number - 1;
            case "Plus Dice":
            case "On My Own Dice":
                return corrupted ? number + 20 : number + 8;
            case "Minus Dice":
                return corrupted ? number * -1 + 17 : number * -1 + 5;
            case "Extend Dice":
                if (number == 20) return corrupted ? 27 : 14;
                else return corrupted ? number + 20 : number + 8;
            case "Operator Dice":
                if (number == -3) return corrupted ? 20 : 8;
                else return corrupted ? 22 : 10;
            case "JQK Dice":
                if (number == 1) return 15;
                else if (number == 2) return 17;
                else return 16;
            case "Bomb Dice":
                return number + 27;
            case "Assassin Dice":
                return number + 33;
            case "Corrupted Dice":
                return number + 36;
            case "Revival Dice":
                return number + 38;
            default:
                return 0;
            
        }
    }
    private IEnumerator MoveNumber(bool isNormal, GameObject Number) {

        float runtime = 0f;
        Vector3 target;
        switch (Number.tag) {
            case "RealNumber":
                target = Camera.main.ScreenToWorldPoint(new Vector3(720 + 960, 480 + 540, 5));
                break;
            case "Alphabet":
                target = Number.transform.position;
                break;
            case "Corrupted":
                target = Camera.main.ScreenToWorldPoint(new Vector3(-174 + 960, 367 + 540, 5));
                break;
            case "Assassin":
                target = Camera.main.ScreenToWorldPoint(new Vector3(167 + 960, 420 + 540, 5));
                break;
            case "Bomb":
                target = Camera.main.ScreenToWorldPoint(new Vector3(-174 + 960, 420 + 540, 5));
                break;
            case "Revival":
                //TODO
            default:
                target = Number.transform.position;
                break;
                
        }
        Vector3 currentPosition = Number.transform.position;
        Vector3 currentScale = Number.transform.localScale;
        while (runtime < moveDuration) {
            runtime += Time.deltaTime;
            Number.transform.position = Vector3.Lerp(currentPosition, target, runtime / moveDuration);
            Number.transform.localScale = Vector3.Lerp(currentScale, 0.05f * Vector3.one, runtime / moveDuration);
            yield return null;
        }
        if (isNormal) DiceUtil.normalDone = true;
        else DiceUtil.specialDone = true;
        Destroy(Number);
    }
    
    //게이지 바를 서서히 움직이는 애니메이션
    //TODO: Normal Dice, Plus Dice, Minus Dice의 숫자가 이동하면 그때 틀어야 함./
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

        NormalDiceToggle.interactable = false;
        NormalDiceToggle.isOn = true;
        NormalDiceImage.sprite = Dice2D[0];

        SpecialDiceToggle.interactable = true;
        SpecialDiceToggle.isOn = false;
        SpecialDiceImage.sprite = Dice2D[GameManager.Inst.pm.activatedPlayer.specialDice.diceIndex];

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











    public void UpdateUI() {

        UpdateNumberText(GameManager.Inst.pm.curCount, GameManager.Inst.pm.maxCount);
        MatchRoundInfo.text = $"Match {GameManager.Inst.pm.matchCount} Round {GameManager.Inst.pm.roundCount}";
        BombNumberText.text = GameManager.Inst.pm.bombDiceNum == 0 ? "" : $"{GameManager.Inst.pm.bombDiceNum}";
        CorruptedCountText.text = $"{GameManager.Inst.pm.corruptStack}";

        //UpdatePlayerPanel(GameManager.Inst.pm.activatedPlayer);
    }

    public void ResetPlayerImage() {
        for (int i = 0; i < 7; i += 2)
        {
            PlayerImages[i].GetComponent<Image>().sprite = PlayerStates[0];
            PlayerImages[i + 1].GetComponent<Image>().sprite = PlayerStates[10];
        }
    }
    public void PlayerDeactivate(int playerIndex) {
        if (playerIndex % 2 == 0)
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[0];
        }
        else
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[10];
        }
    }
    public void PlayerActivate(int playerIndex) {
        if (playerIndex % 2 == 0)
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[1];
        }
        else {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[11];
        }
    }

    public void PlayerDie(int playerIndex, DeadCause deadCause) {
        int deadIndex = 0;
        switch (deadCause) {
            case DeadCause.Number:
                deadIndex += 2;
                break;
            case DeadCause.Bomb:
                deadIndex += 3;
                break;
            case DeadCause.Assassin:
            case DeadCause.AssassinFail:
                switch (GameManager.Inst.pm.assassinInfo) {
                    case AssassinInfo.Bow:
                        deadIndex += 4;
                        break;
                    case AssassinInfo.Sword:
                        deadIndex += 5;
                        break;
                    case AssassinInfo.Gun:
                        deadIndex += 6;
                        break;
                    default:
                        break;
                }
                break;
            case DeadCause.RevivalFail:
                deadIndex += 7;
                break;
            case DeadCause.Corrupted:
                deadIndex += 8;
                break;
        }
        if (playerIndex % 2 == 1) deadIndex += 10;

        PlayerImages[playerIndex].sprite = PlayerStates[deadIndex];
    }

    public void PlayerRevive(int playerIndex) {
        if (playerIndex % 2 == 0)
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[0];
            Debug.Log("Red Team Member Revived");
        }
        else
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[10];
            Debug.Log("Blue Team Member Revived");
        }
    }
    
    public void UpdateNumberText(int curCount, int maxCount) { 
        //RectTransform rect = NumberGauge.GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(480 * curCount / maxCount, 60);
        NumberText.text = $"{curCount} / {maxCount}";
    }

    /* public List<Player> alivePlayers;
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

    } */
    public void ShowOMOButton() {
        SelectOneButton.gameObject.SetActive(true);
        SelectTwoButton.gameObject.SetActive(true);
    }
    public void HideOMOButton() {
        SelectOneButton.gameObject.SetActive(false);
        SelectTwoButton.gameObject.SetActive(false);
    }
    public void ActivateBow() {
        BowImage.color = ActivatedColor;
    }
    public void ActivateSword() {
        SwordImage.color = ActivatedColor;
    }
    public void ActivateGun() {
        GunImage.color = ActivatedColor;
    }
    public void AssassinFinish() {
        BowImage.color = DeactivatedColor;
        SwordImage.color = DeactivatedColor;
        GunImage.color = DeactivatedColor;
    }
    public void ActivateBomb(int bombNumber) {
        BombHolder.color = ActivatedColor;
        BombNumberText.text = $"{GameManager.Inst.pm.bombDiceNum}";
    }
    public void DeactivateBomb() {
        BombHolder.color = DeactivatedColor;
        BombNumberText.text = "";
    }
    public void ResetUI() {
        //TODO: 경기 시작 및 매치 초기화 때 모든 UI 초기화.
        ResetPlayerImage();
        BombHolder.color = DeactivatedColor;
        BowImage.color = DeactivatedColor;
        SwordImage.color = DeactivatedColor;
        GunImage.color = DeactivatedColor;
        CorruptedImage.color = ActivatedColor;
    }
    void Start()
    {
        ResetUI();
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

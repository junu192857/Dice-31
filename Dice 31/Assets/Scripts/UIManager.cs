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
    public List<Sprite> GaugeBarColors;

    public Text NumberText;
    public bool formerMoveDone;

    public Image DiceSelectPanel;
    public List<Sprite> DiceSelectPanelByTeam;

    public Image BombHolder;
    public Image BowImage;
    public Image SwordImage;
    public Image GunImage;
    public Image CorruptedImage;

    private bool gaugeBarMoving;
    public GameObject laserSpherePrefab;
    public GameObject laserBeamPrefab;
    public List<float> laserBeamRotations;
    public List<Vector3> laserBeamTarget;
    public List<Vector2> bombMovingPlot;
    public GameObject explosionPrefab;
    public GameObject bombPrefab;
    public GameObject bowPrefab;
    public bool arrowShootDone = false;
    public Vector3 arrowTarget;

    public List<Sprite> PlayerStates;

    public Toggle NormalDiceToggle;
    public Toggle SpecialDiceToggle;

    public Button SelectOneButton;
    public Button SelectTwoButton;
    public List<Button> RollDiceButtonByTeam;

    public bool operatorDiceDone = true;

    public GameObject purpleGlow;

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
        Debug.Log(numberIndex);
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
        if (dice.GetComponent<Dice>() is not OperatorDice || !dice.GetComponent<OperatorDice>().delayed)
        {
            StartCoroutine(MoveNumber(isNormal, numberSprite));
        }
        else DiceUtil.specialDone = true;
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
    private IEnumerator MoveNumberLater(bool isNormal, GameObject Number) {
        DiceUtil.specialDone = true;
        operatorDiceDone = false;
        Debug.Log(formerMoveDone);
        //yield return new WaitUntil(() => formerMoveDone);
        Debug.Log("Hello from delayed move");
        float runtime = 0f;
        Vector3 target;
        target = Camera.main.ScreenToWorldPoint(new Vector3(720 + 960, 480 + 540, 5));
        
        Vector3 currentPosition = Number.transform.position;
        Vector3 currentScale = Number.transform.localScale;
        while (runtime < moveDuration)
        {
            runtime += Time.deltaTime;
            Number.transform.position = Vector3.Lerp(currentPosition, target, runtime / moveDuration);
            Number.transform.localScale = Vector3.Lerp(currentScale, 0.05f * Vector3.one, runtime / moveDuration);
            yield return null;
        }
        Destroy(Number);
        GaugeBarCoroutine(GameManager.Inst.pm.curCount, GameManager.Inst.pm.maxCount);
    }
    public void DirectlyMoveNumber(bool isNormal, GameObject Number) {
        StartCoroutine(MoveNumberLater(isNormal, Number));
    }
    private IEnumerator MoveNumber(bool isNormal, GameObject Number) {
        formerMoveDone = false;
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



    public void GaugeBarCoroutine(int curCount, int maxCount) {
        StartCoroutine(UpdateGaugeBar(curCount, maxCount, 0.5f));
        UpdateNumberText(curCount, maxCount);
    }
    //게이지 바를 서서히 움직이는 애니메이션
    private IEnumerator UpdateGaugeBar(int curCount, int maxCount, float duration) {
        gaugeBarMoving = true;
        var runTime = 0.0f;

        RectTransform rect = NumberGauge.GetComponent<RectTransform>();
        Image image = NumberGauge.GetComponent<Image>();

        Vector2 curWidth = new Vector2(rect.sizeDelta.x, 60);
        Vector2 targetWidth = curCount <= maxCount ? new Vector2(480 * curCount / maxCount, 60) : new Vector2(480, 60);

        if (targetWidth.x < 240)
        {
            image.sprite = GaugeBarColors[0];
        }
        else if (targetWidth.x >= 240 && targetWidth.x < 360)
        {
            image.sprite = GaugeBarColors[1];
        }
        else {
            image.sprite = GaugeBarColors[2];
        }  
        while (runTime < duration) {
            runTime += Time.deltaTime;
            rect.sizeDelta = Vector2.Lerp(curWidth, targetWidth, runTime / duration);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        gaugeBarMoving = false;
        formerMoveDone = true;
        operatorDiceDone = true;
    }

    //Run whenever new player starts his turn
    public void UpdateDiceSelectPanel() {
        Player player = GameManager.Inst.pm.activatedPlayer;
        NormalDiceImage.sprite = Dice2D[0];

        SpecialDiceToggle.interactable = true;
        SpecialDiceToggle.isOn = false;
        SpecialDiceImage.sprite = Dice2D[player.specialDice.diceIndex];

        if (player.specialDice is CorruptedDice)
        {
            SpecialDiceToggle.isOn = true;
            SpecialDiceToggle.interactable = false;
        }
        else if (!player.specialDice.available) {
            SpecialDiceToggle.interactable = false;
        }

        if (player.team == Team.Red)
        {
            DiceSelectPanel.sprite = DiceSelectPanelByTeam[0];
        }
        else
        {
            DiceSelectPanel.sprite = DiceSelectPanelByTeam[1];
        }
        GameManager.Inst.um.EnableRollButton();
    }

    public void UpdateUI() {
        MatchRoundInfo.text = $"Match {GameManager.Inst.pm.matchCount} Round {GameManager.Inst.pm.roundCount}";
        BombNumberText.text = GameManager.Inst.pm.bombDiceNum == 0 ? "" : $"{GameManager.Inst.pm.bombDiceNum}";
        CorruptedCountText.text = $"{GameManager.Inst.pm.corruptStack}";

        //UpdatePlayerPanel(GameManager.Inst.pm.activatedPlayer);
    }

    public void ResetPlayerImage() {
        for (int i = 0; i < 7; i += 2)
        {
            PlayerImages[i].GetComponent<Image>().sprite = PlayerStates[0];
            PlayerImages[i + 1].GetComponent<Image>().sprite = PlayerStates[12];
        }
    }
    public void ResetPlayerNames()
    {
        for (int i = 0; i < 8; i++)
        {
            PlayerImages[i].transform.GetChild(0).GetComponent<Text>().text = 
                GameManager.Inst.pm.playerInfos[i].playerName;
        }
    }
    public void PlayerDeactivate(Player player) {
        int index = 0;

        if (player.playerIndex % 2 == 1) index += 12;
        if (player.unDead) index += 9;
        PlayerImages[player.playerIndex].GetComponent<Image>().sprite = PlayerStates[index];
    }
    public void PlayerActivate(Player player) {
        int index = 0;

        if (player.playerIndex % 2 == 1) index += 12;
        if (player.unDead) index += 9;

        PlayerImages[player.playerIndex].GetComponent<Image>().sprite = PlayerStates[index+1];
    }

    public void PlayerDie(int playerIndex, DeadCause deadCause) {
        int deadIndex = 0;
        if (GameManager.Inst.pm.playerInfos[playerIndex].unDead)
        {
            deadIndex += 9;
        }
        else
        {
            switch (deadCause)
            {
                case DeadCause.Number:
                    deadIndex += 2;
                    break;
                case DeadCause.Bomb:
                    deadIndex += 3;
                    break;
                case DeadCause.Assassin:
                case DeadCause.AssassinFail:
                    switch (GameManager.Inst.pm.assassinInfo)
                    {
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
        }
        if (playerIndex % 2 == 1) deadIndex += 12;

        PlayerImages[playerIndex].sprite = PlayerStates[deadIndex];
    }

    public IEnumerator PlayerDieAnimation(int playerIndex, DeadCause deadCause) {
        yield return StartCoroutine(DieAnimate(playerIndex, deadCause));
    }

    private IEnumerator DieAnimate(int playerIndex, DeadCause deadCause) {
        yield return new WaitUntil(() => !gaugeBarMoving && operatorDiceDone);
        float runTime = 0f;
        Debug.Log("Player Died!");
        switch (deadCause)
        {
            case DeadCause.Number:
                Debug.Log("Hello from Number");

                RectTransform rect = NumberGauge.GetComponent<RectTransform>();
                Image image = NumberGauge.GetComponent<Image>();

                Vector2 curWidth = new Vector2(rect.sizeDelta.x, 60);
                Vector2 targetWidth = new Vector2(0, 60);

                GameObject laserSphere = Instantiate(laserSpherePrefab, new Vector3(1.9f, 1f, 1f), Quaternion.identity);
                GameManager.Inst.sm.LaserChargeSound();
                while (runTime < 1f)
                {
                    runTime += Time.deltaTime;
                    laserSphere.transform.localScale = Vector3.Lerp(new Vector3(0f, 0.0001f, 0f), new Vector3(0.1f, 0.0001f, 0.1f), runTime / 1f);
                    rect.sizeDelta = Vector2.Lerp(curWidth, targetWidth, runTime / 1f);
                    yield return null;
                }
                runTime = 0f;
                yield return new WaitForSeconds(0.3f);
                Destroy(laserSphere);

                GameObject laserBeam = Instantiate(laserBeamPrefab, new Vector3(1.9f, 1f, 1f), Quaternion.Euler(90f, 0f, laserBeamRotations[playerIndex]));
                laserBeam.transform.localScale = new Vector3(0.05f, -0.005f, 0.05f);
                Vector3 startPos = laserBeam.transform.position;
                GameManager.Inst.sm.LaserShootingSound();
                while (runTime < 0.3f)
                {
                    runTime += Time.deltaTime;
                    laserBeam.transform.position = (startPos * (0.3f - runTime) + (laserBeamTarget[playerIndex] * runTime)) / 0.3f;
                    yield return null;
                }
                Destroy(laserBeam);
                PlayerDie(playerIndex, deadCause);
                break;
            case DeadCause.Assassin:
            case DeadCause.AssassinFail:
                Debug.Log("Hello from Assassin");
                switch (GameManager.Inst.pm.assassinInfo) {
                    case AssassinInfo.Bow:
                        /*BowImage.transform.GetChild(0).gameObject.SetActive(false);
                        GameObject bow = Instantiate(bowPrefab, new Vector3(0.5f, 1f, 1.14f), Quaternion.identity);
                        bow.transform.rotation = Quaternion.Euler(90, 0, 0);
                        bow.GetComponent<Animator>().Play("ArrowAnimation");
                        BowImage.transform.GetChild(0).gameObject.SetActive(false);
                        arrowTarget = new Vector3(-3.5f, 1f, -0.14f - (playerIndex * 2.46f / 7));
                        yield return new WaitUntil(() => arrowShootDone);*/
                        PlayerDie(playerIndex, deadCause);
                        GameManager.Inst.pm.assassinInfo = AssassinInfo.None;
                        break;
                    case AssassinInfo.Sword:
                        PlayerDie(playerIndex, deadCause);
                        GameManager.Inst.pm.assassinInfo = AssassinInfo.None;
                        break;
                    case AssassinInfo.Gun:
                        PlayerDie(playerIndex, deadCause);
                        GameManager.Inst.pm.assassinInfo = AssassinInfo.None;
                        break;
                    default:
                        Debug.Log("Assassin Error");
                        break;
                }
                break;
            
            case DeadCause.Bomb:
                Debug.Log("Hello from Bomb");
                runTime = 0f;

                BombHolder.transform.GetChild(0).gameObject.SetActive(false);

                GameObject bomb = Instantiate(bombPrefab, new Vector3(-1.12f, 1f, 0.76f), Quaternion.identity);
                while (runTime < 0.5f) {
                    runTime += Time.deltaTime;
                    bomb.transform.localScale = Vector3.Lerp(new Vector3(0.04f, 1f, 0.04f), new Vector3(0.06f, 1f, 0.06f), runTime / 0.5f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.3f);
                runTime = 0f;
                while (runTime < 0.5f) {
                    runTime += Time.deltaTime;
                    float t = -4.76f * runTime - 1.12f;
                    bomb.transform.position = new Vector3(t, 1, bombMovingPlot[playerIndex].x * (t + 1.6f) * (t + 1.6f) + bombMovingPlot[playerIndex].y);
                    yield return null;
                }
                Destroy(bomb);
                GameObject explosion = Instantiate(explosionPrefab, new Vector3(-3.5f, 1f, -0.54f - (playerIndex * 2.46f / 7)), Quaternion.Euler(90, 0, 0));
                explosion.transform.localScale = new Vector3(0.08f, 0.08f, 1f);
                explosion.GetComponent<Animator>().Play("EXPAnimator");
                GameManager.Inst.sm.PlayExplosionSound();
                BombHolder.transform.GetChild(0).gameObject.SetActive(true);
                //Destroy(explosion);
                PlayerDie(playerIndex, deadCause);
                break;
            case DeadCause.Corrupted:
            case DeadCause.RevivalFail:
                PlayerDie(playerIndex, deadCause);
                break;
            default:
                break;
        }
    }


    public void PlayerRevive(int playerIndex) {
        if (playerIndex % 2 == 0)
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[0];
            Debug.Log("Red Team Member Revived");
        }
        else
        {
            PlayerImages[playerIndex].GetComponent<Image>().sprite = PlayerStates[12];
            Debug.Log("Blue Team Member Revived");
        }
    }
    
    public void UpdateNumberText(int curCount, int maxCount) {
        //RectTransform rect = NumberGauge.GetComponent<RectTransform>();
        //rect.sizeDelta = new Vector2(480 * curCount / maxCount, 60);
        if (curCount <= maxCount)
        {
            NumberText.text = $"{curCount} / {maxCount}";
        }
        else NumberText.text = $"{maxCount} / {maxCount}";
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
    public void DisableRollButton() {
        if (GameManager.Inst.pm.activatedPlayer.team == Team.Red)
        {
            RollDiceButtonByTeam[0].gameObject.SetActive(false);
        }
        else {
            RollDiceButtonByTeam[1].gameObject.SetActive(false);
        }
    }
    public void EnableRollButton() {
        if (GameManager.Inst.pm.activatedPlayer.team == Team.Red)
        {
            RollDiceButtonByTeam[0].gameObject.SetActive(true);
            RollDiceButtonByTeam[1].gameObject.SetActive(false);
            RollDiceButtonByTeam[0].interactable = true;
            RollDiceButtonByTeam[0].transform.GetChild(0).gameObject.SetActive(true);
        }
        else {
            RollDiceButtonByTeam[1].gameObject.SetActive(true);
            RollDiceButtonByTeam[0].gameObject.SetActive(false);
            RollDiceButtonByTeam[1].interactable = true;
            RollDiceButtonByTeam[1].transform.GetChild(0).gameObject.SetActive(true);
        }
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
    public void GlowFadeIn() {
        StartCoroutine(GlowOn());
    }
    public void GlowFadeOut() {
        StartCoroutine(GlowOff());
    }
    private IEnumerator GlowOn() {
        var runTime = 0f;
        purpleGlow.SetActive(true);
        while (runTime < 2f) {
            runTime += Time.deltaTime;
            purpleGlow.transform.position = new Vector3(0f, 7.1f - 3 * runTime, 0f);
            yield return null;
        }
    }
    private IEnumerator GlowOff() {
        var runTime = 0f;
        while (runTime < 2f)
        {
            runTime += Time.deltaTime;
            purpleGlow.transform.position = new Vector3(0f, 1f + 3 * runTime, 0f);
            yield return null;
        }
        purpleGlow.SetActive(false);
    }
    public void ResetUI() {
        //TODO: 경기 시작 및 매치 초기화 때 모든 UI 초기화.
        ResetPlayerImage();
        BombHolder.color = DeactivatedColor;
        BowImage.color = DeactivatedColor;
        SwordImage.color = DeactivatedColor;
        GunImage.color = DeactivatedColor;
        CorruptedImage.color = ActivatedColor;
        purpleGlow.SetActive(false);
        arrowShootDone = false;
        formerMoveDone = true;
        operatorDiceDone = true;
    }
    void Start()
    {
        ResetUI();
    }

    void Update()
    {
        UpdateUI();
    }


}

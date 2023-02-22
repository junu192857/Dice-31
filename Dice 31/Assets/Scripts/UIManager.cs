using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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
    public Image playerImage;
    public Text playerText;

    public Image BombHolder;
    public Image BowImage;
    public Image SwordImage;
    public Image GunImage;
    public Image CorruptedImage;

    public bool gaugeBarMoving;
    public GameObject laserSpherePrefab;
    public GameObject laserBeamPrefab;
    public List<float> laserBeamRotations;
    public List<Vector3> laserBeamTarget;
    public List<Vector2> bombMovingPlot;
    public GameObject explosionPrefab;
    public GameObject bombPrefab;
    public GameObject bowPrefab;
    public GameObject swordPrefab;
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
    public bool arrowShootDone = false;
    public Vector3 arrowTarget;
    public GameObject brokenHeartPrefab;
    public GameObject skullPrefab;
    public GameObject RedCorruptingPrefab;
    public GameObject BlueCorruptingPrefab;
    public bool corruptAnimationDone;

    public List<Sprite> PlayerStates;

    public Toggle SpecialDiceToggle;
    [SerializeField] private GameObject specialDiceToggleArrow;

    public GameObject omoSelectUI;
    public List<Button> RollDiceButtonByTeam;

    public bool operatorDiceDone = true;

    public GameObject purpleGlow;

    public Text gameLog;

    [SerializeField]
    private List<GameObject> Numbers;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField] private GameObject normalPleaseArrow;
    [SerializeField] private GameObject specialPleaseArrow;
    [SerializeField] private GameObject rollButtonTooltip;

    private float scaleDuration = 0.25f;
    private float moveDuration = 0.5f;

    private Color ActivatedColor = new Color(1f, 1f, 1f);
    private Color DeactivatedColor = new Color(100 / 255f, 100 / 255f, 100 / 255f);

    public void ShowArrow(Dice dice)
    {
        var diceName = dice.diceName;
        var isNormal = diceName == "Normal Dice";

        //GameManager.Inst.gsm.OperateAnimation();

        Debug.Log($"ShowArrow: {diceName} - isNormal ? {isNormal}");
        var screenPoint = Camera.main.WorldToScreenPoint(dice.transform.position) + new Vector3(0, 60, 0);
        var target = Camera.main.ScreenToWorldPoint(screenPoint);
        var arrow = isNormal ? normalPleaseArrow : specialPleaseArrow;
        arrow.transform.position = target;
        arrow.transform.GetChild(0).GetComponent<Renderer>().enabled = true;
    }
    
    public void HideSpecialPleaseArrow()
    {
        specialPleaseArrow.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
    }
    
    public void HideNormalPleaseArrow()
    {
        normalPleaseArrow.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
    }

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
        if (dice.GetComponent<Dice>() is not OperatorDice || !dice.GetComponent<OperatorDice>().delayed)
        {
            StartCoroutine(MoveNumber(isNormal, numberSprite));
        }
        else DiceUtil.specialDone = true;
    }

    private int CalculateIndex(string diceName, int number) {
        bool corrupted = GameManager.Inst.pm.corruptStack >= 4;
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
        else if (player.specialDice is RevivalDice && GameManager.Inst.pm.allAlive) {
            SpecialDiceToggle.interactable = false;
        }
        else if (!player.specialDice.available)
        {
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
        
        specialDiceToggleArrow.SetActive(SpecialDiceToggle.interactable);
        if (GameManager.gameMode == GameMode.Drag)
        {
            normalPleaseArrow.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
            specialPleaseArrow.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
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
        if (player.unDead && !GameManager.Inst.pm.isNewUnDead) index += 9;
        PlayerImages[player.playerIndex].GetComponent<Image>().sprite = PlayerStates[index];
    }
    public void PlayerActivate(Player player) {
        int index = 0;

        if (player.playerIndex % 2 == 1) index += 12;
        if (player.unDead) index += 9;

        PlayerImages[player.playerIndex].GetComponent<Image>().sprite = PlayerStates[index+1];
        playerImage.sprite = PlayerStates[index + 1];
        playerText.text = player.playerName;
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

                AudioSource sphereaudio = laserSphere.GetComponent<AudioSource>();
                sphereaudio.volume = GameManager.Inst.sm.SFXVolume;
                sphereaudio.Play();

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

                AudioSource beamaudio = laserBeam.GetComponent<AudioSource>();
                beamaudio.volume = GameManager.Inst.sm.SFXVolume;
                beamaudio.Play();

                while (runTime < 0.3f)
                {
                    runTime += Time.deltaTime;
                    laserBeam.transform.position = (startPos * (0.3f - runTime) + (laserBeamTarget[playerIndex] * runTime)) / 0.3f;
                    yield return null;
                }
                Destroy(laserBeam);
                
                if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                yield return new WaitForSeconds(0.5f);
                break;
            case DeadCause.Assassin:
            case DeadCause.AssassinFail:
                Debug.Log("Hello from Assassin");
                switch (GameManager.Inst.pm.assassinInfo) {
                    case AssassinInfo.Bow:
                        BowImage.transform.GetChild(0).gameObject.SetActive(false);
                        arrowShootDone = false;
                        arrowTarget = new Vector3(-3f, 1f, -0.4f - (playerIndex * 2.4f / 7));
                        Vector3 start = new Vector3(0.55f, 1f, 0.80f);
                        Vector3 target = GameManager.Inst.um.arrowTarget;
                        float rotation = Vector2.Angle(new Vector2(-1, 0), new Vector2(target.x - start.x, target.z - start.z));
                        GameObject bow = Instantiate(bowPrefab, start, Quaternion.Euler(90, 0, -45+rotation));
                        yield return new WaitUntil(() => arrowShootDone);
                        if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                        Destroy(bow);
                        BowImage.transform.GetChild(0).gameObject.SetActive(true);
                        GameManager.Inst.pm.assassinInfo = AssassinInfo.None;
                        break;
                    case AssassinInfo.Sword:
                        SwordImage.transform.GetChild(0).gameObject.SetActive(false);
                        GameObject sword = Instantiate(swordPrefab, new Vector3(0.74f, 1f, 0.74f), Quaternion.Euler(90, 0, 0));
                        Vector3 finalTarget = new Vector3(-3f, 1f, -0.4f - (playerIndex * 2.4f/ 7));

                        Vector3 swordStartPos = sword.transform.position;
                        Vector3 targetPos = swordStartPos + new Vector3(0.2f, 0, 0.2f);
                        Vector3 swordStartRotation = new Vector3(90, 0, 0);
                        Vector3 swordTargetRotation = new Vector3(90, 0, 135 + Vector2.Angle(new Vector2(-1, 0), new Vector2(finalTarget.x - targetPos.x, finalTarget.z - targetPos.z)));

                        runTime = 0f;
                        while (runTime < 0.5f) {
                            runTime += Time.deltaTime;
                            sword.transform.position = (runTime * targetPos + (0.5f - runTime) * swordStartPos) / 0.5f;
                            sword.transform.rotation = Quaternion.Euler((runTime * swordTargetRotation + ((0.5f - runTime) * swordStartRotation)) / 0.5f);
                            yield return null;
                        }
                        yield return new WaitForSeconds(0.5f);
                        runTime = 0f;
                        while (runTime < 0.5f) {
                            runTime += Time.deltaTime;
                            sword.transform.position = (runTime * finalTarget + (0.5f - runTime) * targetPos) / 0.5f;
                            yield return null;
                        }
                        Destroy(sword);
                        SwordImage.transform.GetChild(0).gameObject.SetActive(true);
                        if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                        GameManager.Inst.pm.assassinInfo = AssassinInfo.None;
                        break;
                    case AssassinInfo.Gun:
                        GunImage.transform.GetChild(0).gameObject.SetActive(false);
                        GameObject gun = Instantiate(gunPrefab, new Vector3(1.13f, 1f, 0.72f), Quaternion.Euler(90, 0, 0));
                        gun.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);

                        Vector3 curPos = gun.transform.position;
                        Vector3 targetPosition = new Vector3(-3f, 1f, -0.4f - (playerIndex * 2.4f / 7));
                        Vector3 targetRotation = new Vector3(90f, 0f, Vector2.Angle(new Vector2(-1, 0), new Vector2(targetPosition.x - curPos.x, targetPosition.z - curPos.z)));
                        gun.transform.rotation = Quaternion.Euler(targetRotation);
                        yield return new WaitForSeconds(1f);
                        Vector3 bulletStartPos = new Vector3(0.83f, 1f, 0.71f);
                        GameObject bullet = Instantiate(bulletPrefab, bulletStartPos, Quaternion.Euler(targetRotation));
                        runTime = 0f;
                        while (runTime < 0.2f) {
                            runTime += Time.deltaTime;
                            gun.transform.rotation = Quaternion.Euler(Vector3.Lerp(targetRotation, targetRotation + new Vector3(0, 0, -45), runTime / 0.2f));
                            bullet.transform.position = Vector3.Lerp(bulletStartPos, targetPosition, runTime / 0.2f);
                            yield return null;
                        }
                        Destroy(bullet);
                        Destroy(gun);
                        if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                        GunImage.transform.GetChild(0).gameObject.SetActive(true);
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

                AudioSource bombaudio = explosion.GetComponent<AudioSource>();
                bombaudio.volume = GameManager.Inst.sm.SFXVolume;
                bombaudio.Play();

                BombHolder.transform.GetChild(0).gameObject.SetActive(true);
                //Destroy(explosion);
                if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                break;
            case DeadCause.Corrupted:
                corruptAnimationDone = false;
                GameObject skull = Instantiate(skullPrefab, new Vector3(-3.4f, 1f, -0.5f - (playerIndex * 2.47f / 7)), Quaternion.Euler(90, 0, 0));
                runTime = 0f;
                SpriteRenderer skullSprite = skull.GetComponent<SpriteRenderer>();
                while (runTime < 0.7f) {
                    runTime += Time.deltaTime;
                    skullSprite.color = new Color { a = runTime / 0.5f, b = 1, g = 1, r = 1 };
                    yield return null;
                }
                yield return new WaitForSeconds(0.5f);
                runTime = 0f;
                while (runTime < 0.7f)
                {
                    runTime += Time.deltaTime;
                    skullSprite.color = new Color { a = 1-(runTime / 0.5f), b = 1, g = 1, r = 1 };
                    yield return null;
                }
                GameObject corrupting;
                if (playerIndex % 2 == 0)
                {
                    corrupting = Instantiate(RedCorruptingPrefab, new Vector3(-3.481f, 1f, -0.534f - (playerIndex * 2.442f / 7)), Quaternion.Euler(90, 0, 0));
                }
                else 
                {
                    corrupting = Instantiate(BlueCorruptingPrefab, new Vector3(-3.481f, 1f, -0.534f - (playerIndex * 2.442f / 7)), Quaternion.Euler(90, 0, 0));
                }
                corrupting.transform.localScale = new Vector3(0.086f, 0.086f, 1f);
                yield return new WaitUntil(() => corruptAnimationDone);
                Destroy(corrupting);
                PlayerDie(playerIndex, deadCause);
                break;
            case DeadCause.RevivalFail:
                GameObject heart = Instantiate(brokenHeartPrefab, new Vector3(-3.4f, 1f, -0.45f - (playerIndex * 2.4f / 7)), Quaternion.identity);
                heart.transform.localScale = new Vector3(0.05f, 1f, 0.05f);
                SpriteRenderer[] sprites = heart.GetComponentsInChildren<SpriteRenderer>();
                runTime = 0f;
                while (runTime < 0.5f) {
                    runTime += Time.deltaTime;
                    sprites[0].color = new Color { a = runTime / 0.5f, b = 1, g = 1, r=  1 };
                    sprites[1].color = new Color { a = runTime / 0.5f, b = 1, g = 1, r = 1 };
                    yield return null;
                }
                yield return new WaitForSeconds(0.7f);
                runTime = 0f;
                Vector3 start1 = sprites[0].transform.localPosition;
                Vector3 start2 = sprites[1].transform.localPosition;
                while (runTime < 0.5f) {
                    runTime += Time.deltaTime;
                    sprites[0].transform.localPosition = Vector3.Lerp(start1, start1 + new Vector3(0.7f, 0, 2f), runTime / 0.5f);
                    sprites[1].transform.localPosition = Vector3.Lerp(start2, start2 + new Vector3(-0.7f, 0, -2f), runTime / 0.5f);
                    sprites[0].color = new Color { a = 1-(runTime / 0.5f), b = 1, g = 1, r = 1 };
                    sprites[1].color = new Color { a = 1-(runTime / 0.5f), b = 1, g = 1, r = 1 };
                    yield return null;
                }
                Destroy(heart);
                if (GameManager.Inst.pm.isNewUnDead || !GameManager.Inst.pm.playerInfos[playerIndex].unDead) PlayerDie(playerIndex, deadCause);
                break;
            default:
                break;
        }
    }

    public IEnumerator PlayerReviveAnimation(int playerIndex) 
    {
        yield return new WaitUntil(() => !gaugeBarMoving && operatorDiceDone);
        float runTime = 0f;
        GameObject heart = Instantiate(brokenHeartPrefab, new Vector3(-3.5f, 1f, -0.45f - (playerIndex * 2.4f / 7)), Quaternion.identity);
        SpriteRenderer[] sprites = heart.GetComponentsInChildren<SpriteRenderer>();
        Vector3 start1 = sprites[0].transform.localPosition;
        Vector3 start2 = sprites[1].transform.localPosition;
        while (runTime < 0.7f)
        {
            runTime += Time.deltaTime;
            sprites[0].color = new Color { a = runTime / 0.7f, b = 1, g = 1, r = 1 };
            sprites[1].color = new Color { a = runTime / 0.7f, b = 1, g = 1, r = 1 };
            sprites[0].transform.localPosition = Vector3.Lerp(start1 + new Vector3(0.7f, 0, 2f), start1, runTime / 0.7f);
            sprites[1].transform.localPosition = Vector3.Lerp(start2 + new Vector3(-0.7f, 0, -2f), start2, runTime / 0.7f);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(heart);
        PlayerRevive(playerIndex);
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

    public IEnumerator ShowGameLog(string log) {
        var runTime = 0f;
        gameLog.text = log;
        gameLog.rectTransform.anchoredPosition = Vector3.down * 434;
        while (runTime < 0.3f) {
            runTime += Time.deltaTime;
            gameLog.color = new Color { a = runTime / 0.3f, b = 1, g = 1, r = 1 };
            yield return null;
        }
        yield return new WaitForSeconds(2);
        runTime = 0f;
        while (runTime < 2f) {
            runTime += Time.deltaTime;
            gameLog.color = new Color { a = 1 - (runTime / 2f), b = 1, g = 1, r = 1 };
            yield return null;
        }
        gameLog.text = "";
        gameLog.color = new Color { a = 1, b = 1, g = 1, r = 1 };
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
        omoSelectUI.SetActive(true);
        gameLog.text = "1과 2 중 하나를 선택하세요!";
        gameLog.rectTransform.anchoredPosition = Vector3.down * 231;
        gameLog.color = Color.black;
    }
    public void HideOMOButton() {
        omoSelectUI.SetActive(false);
        gameLog.text = "";
        gameLog.color = Color.white;
    }
    public void DisableRollButton() {
        DisableSpecialDiceToggle();
        Debug.Log(GameManager.Inst.gsm.State);
        if (GameManager.Inst.pm.activatedPlayer.team == Team.Red)
        {
            RollDiceButtonByTeam[0].gameObject.SetActive(false);
        }
        else
        {
            RollDiceButtonByTeam[1].gameObject.SetActive(false);
        }
        rollButtonTooltip.SetActive(false);
    }

    public void DisableSpecialDiceToggle()
    {
        SpecialDiceToggle.interactable = false;
        specialDiceToggleArrow.SetActive(false);
    }
    public void EnableRollButton() {
        if (GameManager.Inst.pm.activatedPlayer.isBot || GameManager.gameMode == GameMode.Drag)
        {
            RollDiceButtonByTeam[0].gameObject.SetActive(false);
            RollDiceButtonByTeam[1].gameObject.SetActive(false);
        }
        else {
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

    public void PauseGame()
    {
        Debug.Log("Pause");
        pausePanel.SetActive(true);
    }
    void Start()
    {
        ResetUI();
    }

    void Update()
    {
        UpdateUI();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
}

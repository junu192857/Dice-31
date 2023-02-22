using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    [SerializeField] private Vector3 StoragePosition;
    [SerializeField] private Vector3 NormalDicePosition;
    [SerializeField] private Vector3 SpecialDicePosition;

    public List<GameObject> players;
    public List<Player> playerInfos;

    private int index;
    public int turnDirection = 1;

    //winCount는 n판 m선승 체제에서 팀이 몇 번 이겼냐를 나타냄. maxCount는 m에 해당하는 숫자
    public Dictionary<String, int> winCount;
    private int maxWinCount;

    /* curCount: 한 라운드 내에서 현재 숫자
     * maxCount: 라운드 종료의 기준 숫자. 일반적으로 31, 연장 주사위에 의해 변경 가능, 나중에 설정으로 바꿀 수 있게 할 수도 있음
     * roundCount: Round를 나타내는 숫자. 한 명이 죽을 때마다 1씩 증가
     * matchCount: Match를 나타내는 숫자. 승패가 결정될 때마다 1씩 증가, 실제 경기의 승패는 Match를 기준으로 3판 2선승, 5판 3선승 뭐 이런 느낌
     * Round가 시작될 때, Match가 시작될 때, 경기가 시작될 때 어떤 값들을 초기화시켜야 할 지 정해야 함
     */
    [HideInInspector] public int curCount { get; private set; }
    [HideInInspector] public int maxCount { get; private set; }
    [HideInInspector] public int roundCount;
    [HideInInspector] public int matchCount;

    public Player activatedPlayer;
    //public Player playerInfo;

    [SerializeField] private List<Dice> dicesToRoll;

    //테스트용 UI에서 임시로 사용할 bool 변수
    public bool specialDiceActivated
    {
        get => dicesToRoll.Count == 2;
    }

    private List<Dice> previousDices;

    //[SerializeField] private List<String> specialDiceNames;
    [SerializeField] private List<String> greenDiceNames;
    [SerializeField] private List<String> RedPurpleDiceNames;


    // 폭탄 주사위를 굴렸을 때 1~6 사이의 숫자로 설정됨
    // 누군가가 이 숫자와 같은 숫자를 굴리면 그 사람이 탈락하고 bombDiceNum은 0으로 초기화됨 <~~ EndPlayerTurn에서 처리
    public int bombDiceNum = 0;
    public int corruptStack = 0;
    public int onMyOwnDiceNum = 0;
    public bool Jump = false;
    public AssassinInfo assassinInfo;
    private Player playerToRevive;
    public bool isNewUnDead;
    public GameObject purpleGlow;

    private bool pendingRoundEnd = false;
    private Dictionary<int, DeadCause> deadInfo = new Dictionary<int, DeadCause>();
    private int revivalInfo;
    public bool allAlive;

    [SerializeField] private Toggle specialDiceToggle;


    public void UpdateCurCount(int amount)
    {
        if (corruptStack >= 4)
            curCount += amount * 2;
        else
            curCount += amount;
    }

    public void ExtendMaxCount(int amount)
    {
        if (corruptStack >= 4)
            maxCount += amount * 2;
        else
            maxCount += amount;
    }

    public void UpdatePlayerIndex(int amount)
    {
        index += amount * turnDirection;
        if (index < 0) index += 8;
        index %= 8;
    }


    //게임 플레이 화면에서 Game Start 버튼을 누르면 작동하는 Initiate 함수.
    //게임 시작할 때 필요한 값들을 초깃값으로 세팅
    public void Initiate()
    {
        if (GameManager.Inst.gsm.State == GameState.Waiting || GameManager.Inst.gsm.State == GameState.Gameover)
        {
            Debug.Log(GameManager.gameMode);
            index = -1;
            matchCount = 0;
            winCount["Red"] = 0;
            winCount["Blue"] = 0;
            maxWinCount = 1;
            for (int index = 0; index < players.Count; index++)
            {
                if (index % 2 == 0)
                {
                    playerInfos[index].SetRedTeam();
                }
                else
                {
                    playerInfos[index].SetBlueTeam();
                }
                playerInfos[index].playerName = SetupSceneManager.playerNames[index];
                playerInfos[index].isBot = SetupSceneManager.isBot[index];
            }

            GameManager.Inst.um.ResetPlayerNames();
            ResetMatch();
        }
    }

    //라운드가 바뀔 때마다 초기화시킬 것들
    public void ResetRound()
    {
        if (MatchOver())
        {
            ResetMatch();
        }
        else
        {
            curCount = 0;
            maxCount = 31;
            pendingRoundEnd = false;
            //turnDirection = 1; 라운드가 초기화되어도 진행 방향 초기화가 되지 않는 것이 원래 기획
            roundCount++;
            deadInfo.Clear();
            revivalInfo = -1;
            isNewUnDead = false;
            GameManager.Inst.um.GaugeBarCoroutine(curCount, maxCount);
            
            GameManager.Inst.gsm.WaitForPlayerTurn();
        }

        foreach (var player in playerInfos)
        {
            if (player.specialDice.color == DiceColor.Green)
            {
                player.specialDice.EnableDice();
            }
        }
    }

    public void SaveInfoAndLoadEndScene()
    {
        if (winCount["Red"] > winCount["Blue"])
        {
            EndSceneManager.winnerSprites = new[]
            {
                GameManager.Inst.um.PlayerImages[0].sprite,
                GameManager.Inst.um.PlayerImages[2].sprite,
                GameManager.Inst.um.PlayerImages[4].sprite,
                GameManager.Inst.um.PlayerImages[6].sprite,
            };
            EndSceneManager.winnerNames = new[]
            {
                playerInfos[0].playerName,
                playerInfos[2].playerName,
                playerInfos[4].playerName,
                playerInfos[6].playerName,
            };
            EndSceneManager.winnerTeam = "Red";
        }
        else
        {
            EndSceneManager.winnerSprites = new[]
            {
                GameManager.Inst.um.PlayerImages[1].sprite,
                GameManager.Inst.um.PlayerImages[3].sprite,
                GameManager.Inst.um.PlayerImages[5].sprite,
                GameManager.Inst.um.PlayerImages[7].sprite,
            };
            EndSceneManager.winnerNames = new[]
            {
                playerInfos[1].playerName,
                playerInfos[3].playerName,
                playerInfos[5].playerName,
                playerInfos[7].playerName,
            };
            EndSceneManager.winnerTeam = "Blue";
        }

        SceneManager.LoadScene("End");
    }

    //라운드가 바뀔 때마다 초기화시킬 것들
    private void ResetMatch()
    {
        if (GameOver())
        {
            GameManager.Inst.gsm.OperateGameOver();
            SaveInfoAndLoadEndScene();
        }
        else
        {
            index = -1;
            roundCount = 0;
            bombDiceNum = 0;
            onMyOwnDiceNum = 0;
            assassinInfo = AssassinInfo.None;
            corruptStack = 0;
            turnDirection = 1;
            dicesToRoll.Clear();
            previousDices.Clear();
            matchCount++;
            foreach (var player in playerInfos)
            {
                player.Revive();
                player.unDead = false;
            }
            allAlive = true;
            GameManager.Inst.um.ResetUI();
            AssignDices();
            ResetRound();
        }
        //TODO: 이미 사용한 빨간색 특수 주사위 재활성화
    }

    private bool MatchOver()
    {
        if (RedTeamDead() && !BlueTeamDead())
        {
            winCount["Blue"]++;
            Debug.Log($"Blue Team won match {matchCount}");
            return true;
        }
        else if (BlueTeamDead() && !RedTeamDead())
        {
            winCount["Red"]++;
            Debug.Log($"Red Team won match {matchCount}");
            return true;
        }
        else if (RedTeamDead() && BlueTeamDead())
        {
            Debug.Log($"match {matchCount}: Draw");
            return true;
        }
        else return false;
    }

    private bool GameOver()
    {
        if (winCount["Red"] >= maxWinCount)
        {
            Debug.Log("Red Team Win!");
            return true;
        }
        else if (winCount["Blue"] >= maxWinCount)
        {
            Debug.Log("Blue Team Win!");
            return true;
        }
        else return false;
    }


    /*플레이어 턴 진행
     * 1. 주사위를 굴리기 전 처리가 필요하다면 한다. (연산자 주사위의 2++같은 것)
     * 2. 주사위를 굴린다: 버튼 누르면(또는 주사위를 드래그하면?) 
     * 3. 주사위를 굴린 후 처리를 한다
     * 4. activatedPlayer를 다음 사람으로 넘긴다
    */
    private void AssignDices()
    {
        //모든 플레이어에게 할당된 주사위를 없애고, Normal Dice를 하나씩 새로 할당하는 과정
        GameObject[] dices = GameObject.FindGameObjectsWithTag("Dice");
        foreach (var dice in dices)
        {
            Destroy(dice);
        }

        foreach (var player in playerInfos)
        {
            player.normalDice = null;
            player.specialDice = null;

            GameObject normalDice = Instantiate(Resources.Load("NormalDice")) as GameObject;
            player.normalDice = normalDice.GetComponent<NormalDice>();
        }

        System.Random Rand = new System.Random();
        //var shuffled = specialDiceNames.OrderBy(_ => Rand.Next()).ToList();
        var shuffled2 = greenDiceNames.OrderBy(_ => Rand.Next()).ToList();
        var shuffled3 = RedPurpleDiceNames.OrderBy(_ => Rand.Next()).ToList();

        List<int> evenIndexList = new List<int> { 0, 2, 4, 6 };
        evenIndexList = evenIndexList.OrderBy(_ => Rand.Next()).ToList();
        List<int> oddIndexList = new List<int> { 1, 3, 5, 7 };
        oddIndexList = oddIndexList.OrderBy(_ => Rand.Next()).ToList();

        for (int i = 0; i < 4; i++)
        {
            if (i == 0 || i == 1)
            {
                GameObject redSpecialDice = Instantiate(Resources.Load(shuffled3[i])) as GameObject;
                GameObject greenSpecialDice = Instantiate(Resources.Load(shuffled2[i])) as GameObject;
                //GameObject specialDice = Instantiate(Resources.Load("CorruptedDice")) as GameObject;
                playerInfos[evenIndexList[i]].specialDice = redSpecialDice.GetComponent<Dice>();
                playerInfos[evenIndexList[i]].specialDice.EnableDice();

                playerInfos[oddIndexList[i]].specialDice = greenSpecialDice.GetComponent<Dice>();
                playerInfos[oddIndexList[i]].specialDice.EnableDice();
            }
            else
            {
                GameObject redSpecialDice = Instantiate(Resources.Load(shuffled3[i])) as GameObject;
                GameObject greenSpecialDice = Instantiate(Resources.Load(shuffled2[i])) as GameObject;
                //GameObject specialDice = Instantiate(Resources.Load("CorruptedDice")) as GameObject;
                playerInfos[oddIndexList[i]].specialDice = redSpecialDice.GetComponent<Dice>();
                playerInfos[oddIndexList[i]].specialDice.EnableDice();

                playerInfos[evenIndexList[i]].specialDice = greenSpecialDice.GetComponent<Dice>();
                playerInfos[evenIndexList[i]].specialDice.EnableDice();
            }
        }
        /*GameObject temp = Instantiate(Resources.Load("OperatorDice")) as GameObject;
        GameObject temp2 = Instantiate(Resources.Load("AssassinDice")) as GameObject;
        playerInfos[0].specialDice = temp.GetComponent<Dice>();
        playerInfos[1].specialDice = temp2.GetComponent<Dice>();
        playerInfos[1].specialDice.EnableDice(); */
    }

    public void StartPlayerTurn()
    {
        Debug.Assert(GameManager.Inst.gsm.State == GameState.InPlayerTurn);

        AdvancePlayer();

        if (activatedPlayer.dead)
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
            Debug.Log($"{activatedPlayer.playerName} is already dead; skip");
            return;
        }

        if (Jump) {
            GameManager.Inst.gsm.WaitForPlayerTurn();
            Debug.Log("Former player rolled J from JQk Dice");
            Jump = false;
            return;
        }

        foreach (var dice in previousDices)
        {
            dice.EffectBeforeNextPlayerRoll();
        }

        if (activatedPlayer.alive)
        {
            GameManager.Inst.um.PlayerActivate(activatedPlayer);
        }
        
        activatedPlayer.normalDice.GetComponent<DiceController>().ResetDice();
        activatedPlayer.specialDice.GetComponent<DiceController>().ResetDice();
        GameManager.Inst.um.UpdateDiceSelectPanel();

        LoadDicesToRoll();
        
        if (CountExceeded())
        {
            CurrentPlayerDie(DeadCause.Number);
        }

        if (pendingRoundEnd) {
            previousDices.Clear();
            activatedPlayer.normalDice.transform.position = StoragePosition;
            activatedPlayer.normalDice.currentlyRolling = false;
            activatedPlayer.specialDice.transform.position = StoragePosition;
            activatedPlayer.specialDice.currentlyRolling = false;
            StartCoroutine(OperateDieAndRevivalAnimation());
        }

        else if (GameManager.Inst.gsm.State != GameState.Gameover)
        {
            if (GameManager.gameMode == GameMode.OneClick)
                GameManager.Inst.gsm.WaitForInput();
            else
            {
                GameManager.Inst.gsm.WaitForDrag();
            }
            
            if (activatedPlayer.isBot)
            {
                StartCoroutine(DoBotTurn());
            }
        }
    }

    private IEnumerator DoBotTurn()
    {
        yield return new WaitForSeconds(0.5f);
        if (activatedPlayer.specialDice.available)
        {
            if (DecideBotSpecialDice())
            {
                specialDiceToggle.isOn = true;
                AddSpecialDiceCommand();
            }
        }

        yield return new WaitForSeconds(0.5f);
        InstantlyRollPlayerDice();
    }

    private bool DecideBotSpecialDice()
    {
        #if UNITY_EDITOR
        if (GameManagerDisplay.AlwaysThrow)
        {
            return true;
        }
        #endif
        switch (activatedPlayer.specialDice.diceName)
        {
            case "Plus Dice":
            case "JQK Dice":
            case "On My Own Dice":
                return maxCount - curCount >= 10;
            case "Minus Dice":
            case "Extend Dice":
            case "Operator Dice":
                return maxCount - curCount <= 10;
            case "Assassin Dice":
                return FindNextPlayer().team != activatedPlayer.team;
            case "Bomb Dice":
                return true;
            case "Revival Dice":
                return IsMyTeamDead();
            case "Corrupted Dice":
                return false; // 어차피 무적권 굴려야됨
            default:
                Debug.Log("Unknown special dice: " + activatedPlayer.specialDice.diceName);
                return false;
        }
    }

    private Player FindNextPlayer()
    {
        for (int i = 1; i <= 7; i++)
        {
            int nextIndex = (index + i * turnDirection + 8) % 8;
            if (playerInfos[nextIndex].alive)
            {
                return playerInfos[nextIndex];
            }
        }

        return activatedPlayer;
    }

    bool IsMyTeamDead()
    {
        return playerInfos.Any(player => player.team == activatedPlayer.team && player.dead);
    }

    bool CountExceeded()
    {
        return curCount >= maxCount;
    }

    private void AdvancePlayer()
    {
        UpdatePlayerIndex(1);
        activatedPlayer = playerInfos[index];
        Debug.Log($"{activatedPlayer.playerName}'s turn");
    }

    private void LoadDicesToRoll()
    {
        Dice normalDice = activatedPlayer.normalDice;
        Dice specialDice = activatedPlayer.specialDice;

        normalDice.audioPlayedCount = 0;
        specialDice.audioPlayedCount = 0;

        dicesToRoll.Clear();
        AddDice(normalDice, NormalDicePosition);
        if (specialDice is CorruptedDice)
        {
            AddDice(specialDice, SpecialDicePosition);
        }
        else if (specialDice is RevivalDice) {
            allAlive = playerInfos.Count(player => player.team == activatedPlayer.team && player.alive) == 4;
        }
    }

    private void AddDice(Dice dice, Vector3 position)
    {
        dicesToRoll.Add(dice);
        dice.transform.position = position;
        dice.currentlyRolling = true;
        dice.transform.rotation = Quaternion.Euler(
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f,
            Random.Range(0, 4) * 90f
        );
        dice.GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (GameManager.gameMode == GameMode.Drag)
        {
            GameManager.Inst.um.ShowArrow(dice);
        }
    }

    //인게임에서, 특수 주사위를 굴린다고 check했을 때 작동할 함수
    public void AddSpecialDiceCommand()
    {
        if (GameManager.Inst.gsm.State is not (GameState.WaitingForInput or GameState.WaitingForDrag)) return;
        if (dicesToRoll.Count != 1)
        {
            Debug.Log(dicesToRoll.Count);
            Debug.LogWarning("dicesToRoll.Count should be 1");
            return;
        }

        if (!(activatedPlayer.specialDice.available))
        {
            Debug.LogWarning("This dice has been already used");
            return;
        }

        Debug.Log("add special dice: " + activatedPlayer.specialDice.diceName);
        AddDice(activatedPlayer.specialDice, SpecialDicePosition);
    }

    //인게임에서, 특수 주사위를 굴린다는 check를 해제했을 때 작동할 함수
    public void RemoveSpecialDiceCommand()
    {
        if (GameManager.Inst.gsm.State is not (GameState.WaitingForInput or GameState.WaitingForDrag)) return;
        if (dicesToRoll.Count != 2)
        {
            Debug.Log(dicesToRoll.Count);
            Debug.LogWarning("dicesToRoll.Count should be 2");
            return;
        }

        if (activatedPlayer.specialDice is CorruptedDice)
        {
            Debug.LogWarning("special dice should not be corrupted dice");
            return;
        }

        Debug.Log("remove special dice: " + activatedPlayer.specialDice.diceName);
        dicesToRoll.Remove(activatedPlayer.specialDice);
        activatedPlayer.specialDice.transform.position = StoragePosition;
        activatedPlayer.specialDice.currentlyRolling = false;
        if (GameManager.gameMode == GameMode.Drag)
            GameManager.Inst.um.HideSpecialPleaseArrow();
    }

    public void OnClickToggleButton(Toggle toggle)
    {
        if (toggle.isOn)
        {
            AddSpecialDiceCommand();
        }
        else
        {
            RemoveSpecialDiceCommand();
        }
    }

    // 주사위를 굴리는 버튼을 눌렀을 때 작동할 함수
    public void OnRollPlayerDice()
    {
        if (GameManager.Inst.gsm.State != GameState.WaitingForInput) return;
        GameManager.Inst.gsm.BeginRoll();
        StartCoroutine(RollPlayerDice());
    }

    public void OnClickDice()
    {
        if (GameManager.Inst.gsm.State != GameState.WaitingForDrag) return;
        GameManager.Inst.um.DisableSpecialDiceToggle();
        GameManager.Inst.gsm.BeginRoll();
        StartCoroutine(RollPlayerDice());
    }
    
    //주사위 바로 굴리기 버튼을 눌렀을 때 작동할 함수
    public void InstantlyRollPlayerDice()
    {
        OnRollPlayerDice();
        foreach (Dice dice in dicesToRoll)
        {
            DiceController controller = dice.GetComponent<DiceController>();
            Rigidbody rigidbody = controller.GetComponent<Rigidbody>();
            if (controller.CheckDiceState() && !controller.alreadyRolled)
            {
                var circle = 5f * Random.insideUnitCircle;
                rigidbody.velocity = new Vector3(circle.x, Random.Range(6f, 8f), circle.y);
                rigidbody.angularVelocity = Random.onUnitSphere * 15f;
                controller.ChangeStateToRolling();
            }
        }
    }

    private IEnumerator RollPlayerDice()
    {
        var coroutines = dicesToRoll.ConvertAll(dice => StartCoroutine(dice.Roll()));
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        EndPlayerTurn();
    }

    //주사위 굴리기 이후 처리
    private void EndPlayerTurn()
    {
        activatedPlayer.normalDice.transform.position = StoragePosition;
        activatedPlayer.normalDice.currentlyRolling = false;
        activatedPlayer.specialDice.transform.position = StoragePosition;
        activatedPlayer.specialDice.currentlyRolling = false;

        foreach (var dice in previousDices)
        {
            dice.EffectAfterNextPlayerRoll();
        }

        previousDices.Clear();
        previousDices.AddRange(dicesToRoll);

        if (bombDiceNum != 0)
        {
            if (
                bombDiceNum == activatedPlayer.normalDice.value ||
                (bombDiceNum == activatedPlayer.normalDice.value * 2 && corruptStack >= 4)
            )
            {
                Debug.Log($"Bomb ({bombDiceNum}) exploded");
                CurrentPlayerDie(DeadCause.Bomb);
                bombDiceNum = 0;
                GameManager.Inst.um.DeactivateBomb();
            }
            else
            {
                Debug.Log($"Bomb ({bombDiceNum}) passed");
            }
        }

        foreach (var dice in dicesToRoll)
        {
            dice.EffectAfterCurrentPlayerRoll();
        }
        GameManager.Inst.um.GaugeBarCoroutine(curCount, maxCount);


        if (activatedPlayer.alive)
        {
            GameManager.Inst.um.PlayerDeactivate(activatedPlayer);
        }

        if (CountExceeded())
        {
            CurrentPlayerDie(DeadCause.Number);
        }

        if (playerToRevive != null)
        {
            playerToRevive.Revive();
            playerToRevive = null;
        }

        if (pendingRoundEnd || revivalInfo != -1)
        {
            StartCoroutine(OperateDieAndRevivalAnimation());
        }
        else if (GameManager.Inst.gsm.State != GameState.Gameover)
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
        }
    }

    private IEnumerator OperateDieAndRevivalAnimation() {
        GameManager.Inst.gsm.OperateAnimation();
        foreach (KeyValuePair<int, DeadCause> item in deadInfo) {
            yield return StartCoroutine(GameManager.Inst.um.PlayerDieAnimation(item.Key, item.Value));
        }
        yield return new WaitForSeconds(0.5f);
        if (revivalInfo != -1) {
            yield return StartCoroutine(GameManager.Inst.um.PlayerReviveAnimation(revivalInfo));
            revivalInfo = -1;
        }

        if (pendingRoundEnd)
        {
            ResetRound();
        }
        else if (GameManager.Inst.gsm.State != GameState.Gameover)
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
        }
    }
    private Player Convert(GameObject player)
    {
        return player.GetComponent<Player>();
    }

    public void OperateRevivalDice(bool success)
    {
        if (success)
        {
            List<Player> availablePlayers = playerInfos.FindAll(player =>
                player.team == activatedPlayer.team && player.dead);
            if (availablePlayers.Count == 0)
            {
                Debug.Log("Sorry, but there is no player to revive in your team");
            }
            else
            {
                playerToRevive = availablePlayers[Random.Range(0, availablePlayers.Count)];
                Debug.Log($"You Revived {playerToRevive.playerName}");
                revivalInfo = playerToRevive.playerIndex;
                //GameManager.Inst.um.PlayerRevive(playerInfos.IndexOf(playerToRevive));
            }
        }
        else
        {
            Debug.Log("You failed to Revive your team, so you die");
            CurrentPlayerDie(DeadCause.RevivalFail);
        }
    }

    public void SelectNumberOne()
    {
        if (DiceUtil.WaitingOMO)
        {
            onMyOwnDiceNum = 1;
            GameManager.Inst.um.HideOMOButton();
            DiceUtil.WaitingOMO = false;
        }
    }

    public void SelectNumberTwo()
    {
        if (DiceUtil.WaitingOMO)
        {
            onMyOwnDiceNum = 2;
            GameManager.Inst.um.HideOMOButton();
            DiceUtil.WaitingOMO = false;
        }
    }

    public void OperatorDiceResult() {
        StartCoroutine(OperatorDiceAnimation()); 
    }
    private IEnumerator OperatorDiceAnimation() {
        
        yield return new WaitUntil(() => GameManager.Inst.um.formerMoveDone);
        GameObject Number = GameObject.FindGameObjectWithTag("RealNumber");
        GameManager.Inst.um.DirectlyMoveNumber(false, Number);
    }
    private void Awake()
    {
        dicesToRoll = new List<Dice>();
        previousDices = new List<Dice>();
        players = GameObject.FindGameObjectsWithTag("Player").ToList().OrderBy(player => player.GetComponent<Player>().playerIndex).ToList();
        playerInfos = players.ConvertAll(new Converter<GameObject, Player>(Convert)).ToList();
        winCount = new Dictionary<String, int>()
        {
            { "Red", 0 },
            { "Blue", 0 }
        };
    }

    private void Start()
    {
        GameManager.Inst.gsm.PrepareGame();
        Initiate();
    }

    public void PlayerDie(Player player, DeadCause deadCause)
    {
        if (player.dead)
        {
            Debug.Log($"{player.playerName} is already dead");
            return;
        }

        Debug.Log($"{player.playerName} is dead");
        if (player.specialDice is CorruptedDice &&
            !(player.specialDice.GetComponent<CorruptedDice>().pass) &&
            !player.unDead &&
            corruptStack < 5)
        {
            Debug.Log("Hello?");
            player.MakeUndead();
            isNewUnDead = true;
        }
        else if (player.unDead && deadCause != DeadCause.Corrupted) {
            StartCoroutine(GameManager.Inst.um.ShowGameLog("! 언데드는 타락을 제외한 이유로 사망하지 않습니다 !"));
            pendingRoundEnd = true;
            deadInfo.Add(player.playerIndex, deadCause);
            return;
        }
        else
        {
            player.Die();
        }
        player.deadCause = deadCause;
        player.deadRound = roundCount;
        deadInfo.Add(player.playerIndex, deadCause);
        pendingRoundEnd = true;
    }

    public void CurrentPlayerDie(DeadCause deadCause)
    {
        PlayerDie(activatedPlayer, deadCause);
    }

    private bool RedTeamDead()
    {
        return (!playerInfos.Any(player => player.team == Team.Red && player.alive == true));
    }

    private bool BlueTeamDead()
    {
        return (!playerInfos.Any(player => player.team == Team.Blue && player.alive == true));
    }

    public void Update()
    {
        switch (GameManager.Inst.gsm.State)
        {
            case GameState.BeforePlayerTurn:
                GameManager.Inst.gsm.BeginPlayerTurn();
                StartPlayerTurn();
                break;
        }

        if (GameManager.gameMode == GameMode.OneClick && Input.GetKeyDown(KeyCode.R))
        {
            InstantlyRollPlayerDice();
            GameManager.Inst.um.DisableRollButton();
        }
#if UNITY_EDITOR
        if (GameManager.Inst.gsm.State == GameState.WaitingForInput && GameManagerDisplay.AutoPlay)
        {
            InstantlyRollPlayerDice();
        }
#endif
    }
}
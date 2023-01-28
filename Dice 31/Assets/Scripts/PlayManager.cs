using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> players;
    public List<Player> playerInfos;

    private int index;

    //winCount는 n판 m선승 체제에서 팀이 몇 번 이겼냐를 나타냄. maxCount는 m에 해당하는 숫자
    private Dictionary<String, int> winCount;
    private int maxWinCount;

    /* curCount: 한 라운드 내에서 현재 숫자
     * maxCount: 라운드 종료의 기준 숫자. 일반적으로 31, 연장 주사위에 의해 변경 가능, 나중에 설정으로 바꿀 수 있게 할 수도 있음
     * roundCount: Round를 나타내는 숫자. 한 명이 죽을 때마다 1씩 증가
     * matchCount: Match를 나타내는 숫자. 승패가 결정될 때마다 1씩 증가, 실제 경기의 승패는 Match를 기준으로 3판 2선승, 5판 3선승 뭐 이런 느낌
     * Round가 시작될 때, Match가 시작될 때, 경기가 시작될 때 어떤 값들을 초기화시켜야 할 지 정해야 함
     */
    [HideInInspector]
    public int curCount;
    [HideInInspector]
    public int maxCount;
    [HideInInspector]
    public int roundCount;
    [HideInInspector]
    public int matchCount;


    public Player activatedPlayer;
    //public Player playerInfo;

    [SerializeField]
    private List<Dice> dicesToRoll;

    private List<Func<bool>> dieCheckList;

    [SerializeField]
    private List<String> specialDiceNames;



    // 폭탄 주사위를 굴렸을 때 1~6 사이의 숫자로 설정됨
    // 누군가가 이 숫자와 같은 숫자를 굴리면 그 사람이 탈락하고 bombDiceNum은 0으로 초기화됨 <~~ EndPlayerTurn에서 처리
    public int bombDiceNum = 0;

    //게임 플레이 화면에서 Game Start 버튼을 누르면 작동하는 Initiate 함수.
    //게임 시작할 때 필요한 값들을 초깃값으로 세팅
    public void Initiate() {

        matchCount = 0;
        winCount["Red"] = 0;
        winCount["Blue"] = 0;
        maxWinCount = 1;
        for (int index = 0; index < players.Count; index++) {
            if (index % 2 == 0)
            { 
                playerInfos[index].SetRedTeam();
            }
            else 
            {
                playerInfos[index].SetBlueTeam();
            }
        }
        ResetMatch();
        //TODO: 모든 플레이어의 특수 주사위 활성화
    }

    //라운드가 바뀔 때마다 초기화시킬 것들
    private void ResetRound() {
        if (MatchOver()) {
            ResetMatch();
        }
        else
        {
            curCount = 0;
            maxCount = 31;
            roundCount++;
            GameManager.Inst.gsm.WaitForPlayerTurn();
        }
        //TODO: 이미 사용한 초록색 특수 주사위 재활성화
    }

    //라운드가 바뀔 때마다 초기화시킬 것들
    private void ResetMatch() {
        if (GameOver())
        {
            GameManager.Inst.gsm.OperateGameOver();
            //TODO: 결과창 띄우고, 경기 재시작 버튼 등 누를 수 있게 하기
        }
        else
        {
            index = 0;
            roundCount = 0;
            matchCount++;
            foreach (var player in playerInfos) {
                player.Revive();
            }
            AssignDices();
            ResetRound();
        }
        //TODO: 이미 사용한 빨간색 특수 주사위 재활성화
    }

    private bool MatchOver() {
        if (RedTeamDead())
        {
            winCount["Blue"]++;
            Debug.Log($"Blue Team won match {matchCount}");
            return true;
        }
        else if (BlueTeamDead())
        {
            winCount["Red"]++;
            Debug.Log($"Red Team won match {matchCount}");
            return true;
        }
        else return false;
    }
    private bool GameOver() {
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
    private void AssignDices() {
        //모든 플레이어에게 할당된 주사위를 없애고, Normal Dice를 하나씩 새로 할당하는 과정
        GameObject[] dices = GameObject.FindGameObjectsWithTag("Dice");
        foreach (var dice in dices) {
            Destroy(dice);
        }

        foreach (var player in playerInfos) {
            player.normalDice = null;
            player.specialDice = null;

            GameObject normalDice = Instantiate(Resources.Load("NormalDice")) as GameObject;
            player.normalDice = normalDice.GetComponent<Dice>();
        }

        System.Random Rand = new System.Random();
        var shuffled = specialDiceNames.OrderBy(_ => Rand.Next()).ToList();

        for (int i = 0; i < playerInfos.Count; i++)
        {
            GameObject specialDice = Instantiate(Resources.Load(shuffled[i])) as GameObject;
            playerInfos[i].specialDice = specialDice.GetComponent<Dice>();
        }
    }

    public void StartPlayerTurn() {
        Debug.Assert(GameManager.Inst.gsm.State == GameState.InPlayerTurn);
        
        AdvancePlayer();

        if (activatedPlayer.dead)
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
            Debug.Log($"{activatedPlayer.playerName} is already dead; skip");
            return;
        }
        
        foreach (var dice in dicesToRoll)
        {
            dice.EffectBeforeNextPlayerRoll();
        }

        if (CountExceeded())
        {
            CurrentPlayerDie();
        }

        LoadDicesToRoll();
        UpdateDieCheckList();

        GameManager.Inst.gsm.WaitForInput();
    }
    
    bool CountExceeded() {
        return curCount >= maxCount;
    }

    private void AdvancePlayer()
    {
        activatedPlayer = playerInfos[index];
        Debug.Log($"{activatedPlayer.playerName}'s turn");
        index++;
        index %= 8;
    }

    private void LoadDicesToRoll()
    {
        dicesToRoll.Clear();
        dicesToRoll.Add(activatedPlayer.normalDice);
    }

    private void UpdateDieCheckList()
    {
        // TODO
    }
    

    //인게임에서, 특수 주사위를 굴린다고 check했을 때 작동할 함수
    public void AddSpecialDiceCommand() {
        dicesToRoll.Add(activatedPlayer.specialDice);
    }
    //인게임에서, 특수 주사위를 굴린다는 check를 해제했을 때 작동할 함수
    public void RemoveSpecialDiceCommand() { 
        dicesToRoll.Remove(activatedPlayer.specialDice);
    }


    // 주사위를 굴리는 버튼을 눌렀을 때 작동할 함수
    public void OnRollPlayerDice() {
        if (GameManager.Inst.gsm.State != GameState.WaitingForInput) return;
        GameManager.Inst.gsm.BeginRoll();
        StartCoroutine(RollPlayerDice());
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
        /* (0. 주사위 숫자로 인한 카운트 증가는 주사위들의 Roll 메소드에서 처리했음. 단 예외로 연산자 주사위의 2++을 처리해야 함)
         * 1. curCount >= maxCount일 때 현재 플레이어 사망 처리
         * 2. 주사위의 특수 능력 발동 (대부분의 특수 주사위)
         * 3. 주사위의 특수 능력 발동으로 인한 플레이어 사망 처리(폭탄 주사위, 암살 주사위, 부활 주사위, 타락 주사위)
         * 4. 사망한 플레이어가 있다면 라운드를 초기화하고, 
         */
        foreach (var dice in dicesToRoll)
        {
            dice.EffectAfterCurrentPlayerRoll();
        }
        Debug.Log($"Current Count is {curCount}");
        if (dieCheckList.Exists(shouldDie => shouldDie()))
        {
            CurrentPlayerDie();
        }

        if (GameManager.Inst.gsm.State != GameState.Gameover) 
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
        }
    }

    private Player Convert(GameObject player) {
        return player.GetComponent<Player>();
    }

    private void Awake()
    {
        dicesToRoll = new List<Dice>();
        dieCheckList = new List<Func<bool>> { CountExceeded };
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        playerInfos = players.ConvertAll(new Converter<GameObject, Player>(Convert)).ToList();
        winCount = new Dictionary<String, int>() {
            {"Red", 0 },
            {"Blue", 0 }
        };
    }
    private void Start()
    {
        GameManager.Inst.gsm.PrepareGame();
    }
    private void CurrentPlayerDie()
    {
        Debug.Log($"{activatedPlayer.playerName} is dead");
        activatedPlayer.Die();
        ResetRound();
    }

    private bool RedTeamDead() {
        return (!playerInfos.Any(player => player.team == Team.Red && player.alive == true));
    }

    private bool BlueTeamDead() {
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
    }
}

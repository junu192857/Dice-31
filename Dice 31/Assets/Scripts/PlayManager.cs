using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> players;
    private int index;

    //winCount�� n�� m���� ü������ ���� �� �� �̰�ĸ� ��Ÿ��. maxCount�� m�� �ش��ϴ� ����
    private Dictionary<String, int> winCount;
    private int maxWinCount;

    /* curCount: �� ���� ������ ���� ����
     * maxCount: ���� ������ ���� ����. �Ϲ������� 31, ���� �ֻ����� ���� ���� ����, ���߿� �������� �ٲ� �� �ְ� �� ���� ����
     * roundCount: Round�� ��Ÿ���� ����. �� ���� ���� ������ 1�� ����
     * matchCount: Match�� ��Ÿ���� ����. ���а� ������ ������ 1�� ����, ���� ����� ���д� Match�� �������� 3�� 2����, 5�� 3���� �� �̷� ����
     * Round�� ���۵� ��, Match�� ���۵� ��, ��Ⱑ ���۵� �� � ������ �ʱ�ȭ���Ѿ� �� �� ���ؾ� ��
     */
    [HideInInspector]
    public int curCount;
    [HideInInspector]
    public int maxCount;
    [HideInInspector]
    public int roundCount;
    [HideInInspector]
    public int matchCount;


    public GameObject activatedPlayer;
    public Player playerInfo;

    [SerializeField]
    private List<Dice> dicesToRoll;

    private List<Func<bool>> dieCheckList;

    [SerializeField]
    private List<String> specialDiceNames;



    // ??? ??????? ?????? ?? 1~6 ?????? ????? ??????
    // ???????? ?? ????? ???? ????? ?????? ?? ????? ?????? bombDiceNum?? 0???? ?????? <~~ EndPlayerTurn???? ???
    public int bombDiceNum = 0;

    //���� �÷��� ȭ�鿡�� Game Start ��ư�� ������ �۵��ϴ� Initiate �Լ�.
    //���� ������ �� �ʿ��� ������ �ʱ갪���� ����
    public void Initiate() {
        matchCount = 0;
        winCount["Red"] = 0;
        winCount["Blue"] = 0;
        maxWinCount = 1;
        ResetMatch();
        //TODO: ��� �÷��̾��� ��� Ư�� �ֻ��� Ȱ��ȭ
    }

    //���尡 �ٲ� ������ �ʱ�ȭ��ų �͵�
    private void ResetRound() {
        curCount = 0;
        maxCount = 31;
        roundCount++;
        GameManager.Inst.gsm.WaitForPlayerTurn();
        //TODO: �̹� ����� �ʷϻ� Ư�� �ֻ��� ��Ȱ��ȭ
    }

    //Match�� �ٲ� ������ �ʱ�ȭ�� �͵�
    private void ResetMatch() {
        index = 0;
        roundCount = 0;
        matchCount++;
        AssignDices();
        ResetRound();
        //TODO: �̹� ����� ������ Ư�� �ֻ��� ��Ȱ��ȭ
    }
    


    /*�÷��̾� �� ����
     * 1. �ֻ����� ������ �� ó���� �ʿ��ϴٸ� �Ѵ�. (������ �ֻ����� 2++���� ��)
     * 2. �ֻ����� ������: ��ư ������(�Ǵ� �ֻ����� �巡���ϸ�?) 
     * 3. �ֻ����� ���� �� ó���� �Ѵ�
     * 4. activatedPlayer�� ���� ������� �ѱ��
    */
    private void AssignDices() {
        //��� �÷��̾�� �Ҵ�� �ֻ����� ���ְ�, Normal Dice�� �ϳ��� ���� �Ҵ��ϴ� ����
        foreach (var player in players)
        {
            var children = player.GetComponentsInChildren<Transform>();
            foreach (var child in children) {
                if (child != player.transform) {
                    Destroy(child.gameObject);
                }
            }
            GameObject normalDice = Instantiate(Resources.Load("NormalDice")) as GameObject;
            normalDice.transform.SetParent(player.transform);
        }

        //??? ????????? Special Dice?? ????? ????? ???, ??????? ?????? ????
        System.Random Rand = new System.Random();
        var shuffled = specialDiceNames.OrderBy(_ => Rand.Next()).ToList();

        specialDiceNames = shuffled;

        for (int i = 0; i < players.Count; i++)
        {
            GameObject specialDice = Instantiate(Resources.Load(shuffled[i])) as GameObject;
            specialDice.transform.SetParent(players[i].transform);
        }
    }

    public void StartPlayerTurn() {
        Debug.Assert(GameManager.Inst.gsm.State == GameState.InPlayerTurn);
        
        AdvancePlayer();

        if (playerInfo.dead)
        {
            GameManager.Inst.gsm.WaitForPlayerTurn();
            Debug.Log($"{playerInfo.playerName} is already dead; skip");
            return;
        }
        
        // ???? ????? ???? ??????? ????? ??????.
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
        activatedPlayer = players[index];
        playerInfo = activatedPlayer.GetComponent<Player>();
        Debug.Log($"{playerInfo.playerName}'s turn");
        index++;
        index %= 8;
    }

    private void LoadDicesToRoll()
    {
        dicesToRoll.Clear();
        dicesToRoll.Add(playerInfo.transform.GetChild(0).GetComponent<Dice>());
    }

    private void UpdateDieCheckList()
    {
        // TODO
    }
    

    //????????, ??? ??????? ??????? check???? ?? ????? ???
    public void AddSpecialDiceCommand() {
        dicesToRoll.Add(playerInfo.transform.GetChild(1).GetComponent<Dice>());
    }
    //????????, ??? ??????? ??????? check?? ???????? ?? ????? ???
    public void RemoveSpecialDiceCommand() { 
        dicesToRoll.Remove(playerInfo.transform.GetChild(1).GetComponent<Dice>());
    }


    // ??????? ?????? ????? ?????? ?? ????? ???
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

    //????? ?????? ???? ???
    private void EndPlayerTurn() {
        /* (0. ????? ????? ???? ???? ?????? ????????? Roll ?????? ???????. ?? ????? ?????? ??????? 2++?? ?????? ??)
         * 1. curCount >= maxCount?? ?? ???? ??????? ??? ???
         * 2. ??????? ??? ??? ??? (??????? ??? ?????)
         * 3. ??????? ??? ??? ??????? ???? ??????? ??? ???(??? ?????, ??? ?????, ??? ?????, ??? ?????)
         * 4. ????? ??????? ???? ???? ???????, 
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

        GameManager.Inst.gsm.WaitForPlayerTurn();
    }

    private void Awake()
    {
        dicesToRoll = new List<Dice>();
        dieCheckList = new List<Func<bool>> { CountExceeded };
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        winCount = new Dictionary<String, int>() {
            {"Red", 0 },
            {"Blue", 0 }
        };
    }
    private void Start()
    {
        Debug.Log($"Current Count is {curCount}, Max Count is {maxCount}");
        GameManager.Inst.gsm.PrepareGame();
    }
    private void CurrentPlayerDie()
    {
        Debug.Log($"{playerInfo.playerName} is dead");
        playerInfo.Die();
        Initiate();
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

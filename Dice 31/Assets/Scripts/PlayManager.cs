using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> players;
    private int index;

    /* curCount: ?? ???? ?????? ???? ????
     * maxCount: ???? ?????? ???? ????. ????????? 31, ???? ??????? ???? ???? ????, ????? ???????? ??? ?? ??? ?? ???? ????
     * roundCount: Round?? ??????? ????. ?? ???? ???? ?????? 1?? ????
     * matchCount: Match?? ??????? ????. ?????? ?????? ?????? 1?? ????, ???? ????? ?????? Match?? ???????? 3?? 2????, 5?? 3???? ?? ??? ????
     * Round?? ????? ??, Match?? ????? ??, ??? ????? ?? ?? ?????? ????????? ?? ?? ????? ??
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

    //???? ?????? ????? Game Start ????? ?????? ?????? Initiate ???.
    public void Initiate() {

        index = 0;
        curCount = 0;
        maxCount = 31;
        GameManager.Inst.gsm.WaitForPlayerTurn();

    }

    private void Awake()
    {
        dicesToRoll = new List<Dice>();
        dieCheckList = new List<Func<bool>> { CountExceeded };
        players = GameObject.FindGameObjectsWithTag("Player").ToList();

        //??? ????????? Normal Dice?? ????? ?????? ????
        foreach (var player in players) {
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
    private void Start()
    {
        Debug.Log($"Current Count is {curCount}, Max Count is {maxCount}");
        GameManager.Inst.gsm.PrepareGame();
    }


    /*??????? ?? ????
     * 1. ??????? ?????? ?? ????? ??????? ???. (?????? ??????? 2++???? ??)
     * 2. ??????? ??????: ??? ??????(??? ??????? ?????????) 
     * 3. ??????? ???? ?? ????? ???
     * 4. activatedPlayer?? ???? ??????? ????
    */
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

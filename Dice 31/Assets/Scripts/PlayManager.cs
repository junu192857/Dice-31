using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> players;
    private int index;

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


    public GameObject activatedPlayer;
    public Player playerInfo;

    [SerializeField]
    private List<Action> commands;
    [SerializeField]
    private List<String> specialDiceNames;



    // 폭탄 주사위를 굴렸을 때 1~6 사이의 숫자로 설정됨
    // 누군가가 이 숫자와 같은 숫자를 굴리면 그 사람이 탈락하고 bombDiceNum은 0으로 초기화됨 <~~ EndPlayerTurn에서 처리
    public int bombDiceNum = 0;

    //게임 플레이 화면에서 Game Start 버튼을 누르면 작동하는 Initiate 함수.
    public void Initiate() {

        index = 0;
        curCount = 0;
        maxCount = 31;
        GameManager.Inst.gsm.WaitForPlayerTurn();

    }

    private void Awake()
    {
        commands = new List<Action>();
        players = GameObject.FindGameObjectsWithTag("Player").ToList();

        //모든 플레이어에게 Normal Dice를 하나씩 할당하는 과정
        foreach (var player in players) {
            GameObject normalDice = Instantiate(Resources.Load("NormalDice")) as GameObject;
            normalDice.transform.SetParent(player.transform);
        }

        //모든 플레이어에게 Special Dice를 하나씩 겹치지 않게, 랜덤하게 할당하는 과정
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


    /*플레이어 턴 진행
     * 1. 주사위를 굴리기 전 처리가 필요하다면 한다. (연산자 주사위의 2++같은 것)
     * 2. 주사위를 굴린다: 버튼 누르면(또는 주사위를 드래그하면?) 
     * 3. 주사위를 굴린 후 처리를 한다
     * 4. activatedPlayer를 다음 사람으로 넘긴다
    */
    public void StartPlayerTurn() {

        if (GameManager.Inst.gsm.State != GameState.InPlayerTurn) return;
        else {
            activatedPlayer = players[index];
            playerInfo = activatedPlayer.GetComponent<Player>();
            Debug.Log($"{playerInfo.playerName}'s turn");
            index++;
            index %= 8;

            if (!playerInfo.alive) {
                GameManager.Inst.gsm.WaitForPlayerTurn();
                Debug.Log($"{playerInfo.playerName} is dead");
                return;
            }

            commands.Clear();
            commands.Add(playerInfo.transform.GetChild(0).GetComponent<Dice>().Roll);

            GameManager.Inst.gsm.WaitForInput();
            //TODO: 주사위 굴리기 전 처리
        }
    }

    //인게임에서, 특수 주사위를 굴린다고 check했을 때 작동할 함수
    public void AddSpecialDiceCommand() {
        commands.Add(playerInfo.transform.GetChild(1).GetComponent<Dice>().Roll);
    }
    //인게임에서, 특수 주사위를 굴린다는 check를 해제했을 때 작동할 함수
    public void RemoveSpecialDiceCommand() { 
        commands.Remove(playerInfo.transform.GetChild(1).GetComponent<Dice>().Roll);
    }


    // 주사위를 굴리는 버튼을 눌렀을 때 작동할 함수
    public void RollPlayerDice() {
        if (GameManager.Inst.gsm.State != GameState.WaitingForInput) return;
        foreach (var command in commands) {
            command();
        }
        Debug.Log($"Current Count is {curCount}");
        // TODO: 주사위 굴리는 애니메이션
        // 애니메이션이 완전히 작동된 후에 EndPlayerTurn이 실행되어야 함
        EndPlayerTurn();
    }

    //주사위 굴리기 이후 처리
    public void EndPlayerTurn() {
        /* (0. 주사위 숫자로 인한 카운트 증가는 주사위들의 Roll 메소드에서 처리했음. 단 예외로 연산자 주사위의 2++을 처리해야 함)
         * 1. curCount >= maxCount일 때 현재 플레이어 사망 처리
         * 2. 주사위의 특수 능력 발동 (대부분의 특수 주사위)
         * 3. 주사위의 특수 능력 발동으로 인한 플레이어 사망 처리(폭탄 주사위, 암살 주사위, 부활 주사위, 타락 주사위)
         * 4. 사망한 플레이어가 있다면 라운드를 초기화하고, 
         */

        GameManager.Inst.gsm.WaitForPlayerTurn();
    }

    public void Update()
    {
        if (GameManager.Inst.gsm.State == GameState.BeforePlayerTurn)
        {
            GameManager.Inst.gsm.BeginPlayerTurn();
            StartPlayerTurn();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public List<GameObject> players;
    private int index;

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
    private List<Action> commands;
    [SerializeField]
    private List<String> specialDiceNames;



    // ��ź �ֻ����� ������ �� 1~6 ������ ���ڷ� ������
    // �������� �� ���ڿ� ���� ���ڸ� ������ �� ����� Ż���ϰ� bombDiceNum�� 0���� �ʱ�ȭ�� <~~ EndPlayerTurn���� ó��
    public int bombDiceNum = 0;

    //���� �÷��� ȭ�鿡�� Game Start ��ư�� ������ �۵��ϴ� Initiate �Լ�.
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

        //��� �÷��̾�� Normal Dice�� �ϳ��� �Ҵ��ϴ� ����
        foreach (var player in players) {
            GameObject normalDice = Instantiate(Resources.Load("NormalDice")) as GameObject;
            normalDice.transform.SetParent(player.transform);
        }

        //��� �÷��̾�� Special Dice�� �ϳ��� ��ġ�� �ʰ�, �����ϰ� �Ҵ��ϴ� ����
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


    /*�÷��̾� �� ����
     * 1. �ֻ����� ������ �� ó���� �ʿ��ϴٸ� �Ѵ�. (������ �ֻ����� 2++���� ��)
     * 2. �ֻ����� ������: ��ư ������(�Ǵ� �ֻ����� �巡���ϸ�?) 
     * 3. �ֻ����� ���� �� ó���� �Ѵ�
     * 4. activatedPlayer�� ���� ������� �ѱ��
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
            //TODO: �ֻ��� ������ �� ó��
        }
    }

    //�ΰ��ӿ���, Ư�� �ֻ����� �����ٰ� check���� �� �۵��� �Լ�
    public void AddSpecialDiceCommand() {
        commands.Add(playerInfo.transform.GetChild(1).GetComponent<Dice>().Roll);
    }
    //�ΰ��ӿ���, Ư�� �ֻ����� �����ٴ� check�� �������� �� �۵��� �Լ�
    public void RemoveSpecialDiceCommand() { 
        commands.Remove(playerInfo.transform.GetChild(1).GetComponent<Dice>().Roll);
    }


    // �ֻ����� ������ ��ư�� ������ �� �۵��� �Լ�
    public void RollPlayerDice() {
        if (GameManager.Inst.gsm.State != GameState.WaitingForInput) return;
        foreach (var command in commands) {
            command();
        }
        Debug.Log($"Current Count is {curCount}");
        // TODO: �ֻ��� ������ �ִϸ��̼�
        // �ִϸ��̼��� ������ �۵��� �Ŀ� EndPlayerTurn�� ����Ǿ�� ��
        EndPlayerTurn();
    }

    //�ֻ��� ������ ���� ó��
    public void EndPlayerTurn() {
        /* (0. �ֻ��� ���ڷ� ���� ī��Ʈ ������ �ֻ������� Roll �޼ҵ忡�� ó������. �� ���ܷ� ������ �ֻ����� 2++�� ó���ؾ� ��)
         * 1. curCount >= maxCount�� �� ���� �÷��̾� ��� ó��
         * 2. �ֻ����� Ư�� �ɷ� �ߵ� (��κ��� Ư�� �ֻ���)
         * 3. �ֻ����� Ư�� �ɷ� �ߵ����� ���� �÷��̾� ��� ó��(��ź �ֻ���, �ϻ� �ֻ���, ��Ȱ �ֻ���, Ÿ�� �ֻ���)
         * 4. ����� �÷��̾ �ִٸ� ���带 �ʱ�ȭ�ϰ�, 
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

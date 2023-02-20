using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { 
    Red,
    Blue
}
public enum DeadCause { 
    Number,
    Bomb,
    Assassin,
    AssassinFail,
    RevivalFail,
    Corrupted
}
public class Player : MonoBehaviour
{
    public string playerName;

    //PlayerIndex, DeadCause, DeadRound�� �÷��̾� �������� ����ϴ� ������
    public int playerIndex;
    public DeadCause deadCause;
    public int deadRound;
    public bool alive { get; private set; }
    public bool dead { get => !alive; }

    public bool unDead;


    public bool specialDiceUsed { get; private set; }
    public Team team { get; private set; }
    public bool isBot;

    public NormalDice normalDice;
    public Dice specialDice;

    public void Start()
    {
        playerName = gameObject.name;
        alive = true;
        unDead = false;
    }

    public void Die()
    {
        if (alive)
        {
            alive = false;
            unDead = false;
        }
        
    }

    public void Revive()
    {
        if (!alive)
        {
            alive = true;
            unDead = false;
        }
    }

    public void MakeUndead() {
        if (!unDead) {
            unDead = true;
        }
    }

    public void SetRedTeam() 
    {
        team = Team.Red;
    }

    public void SetBlueTeam()
    { 
        team = Team.Blue;
    }

    public void EnableSpecialDice() {
        if (specialDiceUsed) {
            specialDiceUsed = false;
        }
    }

    public void DisableSpecialDice() {
        if (!specialDiceUsed) {
            specialDiceUsed = true;
        }
    }
}

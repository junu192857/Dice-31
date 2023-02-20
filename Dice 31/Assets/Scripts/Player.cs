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

    //PlayerIndex, DeadCause, DeadRound는 플레이어 툴팁에서 사용하는 변수들
    public int playerIndex;
    public DeadCause deadCause;
    public int deadRound;
    public bool alive { get; private set; }
    public bool dead { get => !alive; }


    public bool specialDiceUsed { get; private set; }
    public Team team { get; private set; }
    public bool isBot;

    public NormalDice normalDice;
    public Dice specialDice;

    public void Start()
    {
        playerName = gameObject.name;
        alive = true;
    }

    public void Die()
    {
        if (alive)
        {
            alive = false;
        }
        
    }

    public void Revive()
    {
        if (!alive)
        {
            alive = true;
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

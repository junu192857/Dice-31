using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { 
    Red,
    Blue
}
public class Player : MonoBehaviour
{
    public string playerName;

    public bool alive { get; private set; }
    public bool dead { get => !alive; }

    public string deadString { get => dead ? "Dead" : "Alive"; }

    public bool specialDiceUsed { get; private set; }
    public Team team { get; private set; }

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

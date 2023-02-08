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

    public Team team { get; private set; }

    public Dice normalDice;
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
}

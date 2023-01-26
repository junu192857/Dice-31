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

    public bool alive;

    private Team team;

    public Dice normalDice;
    public Dice specialDice;

    public void Start()
    {
        playerName = gameObject.name;
        alive = true;
    }
}

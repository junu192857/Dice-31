using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDice : Dice
{
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i => GameManager.Inst.pm.bombDiceNum = i);
    }
    
    private void Start()
    {
        color = Color.Red;
        diceName = "Bomb";
    }
}

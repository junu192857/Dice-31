using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i => value = i);
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        Debug.Log("bomb activated: " + value);
        GameManager.Inst.pm.bombDiceNum = value;
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
    }
}

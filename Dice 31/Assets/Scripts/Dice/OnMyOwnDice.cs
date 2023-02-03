using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMyOwnDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => value = i);
    }

    public override void EffectBeforeNextPlayerRoll()
    {
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        Debug.Log($"You selected {value} from On My Own Dice");
        GameManager.Inst.pm.UpdateCurCount(value);
    }

    private void Awake()
    {
        diceName = "On My Own Dice";
        color = DiceColor.Green;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusDice : Dice
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
        Debug.Log($"You rolled {value} from {diceName}");
        GameManager.Inst.pm.UpdateCurCount(value);
    }

    private void Awake()
    {
        diceName = "Plus Dice";
        color = DiceColor.Green;
    }
}

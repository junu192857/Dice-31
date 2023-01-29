using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i => value = i);
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        // NOP
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        GameManager.Inst.pm.curCount += value;
    }

    public NormalDice() {
        color = Color.Yellow;
        diceName = "Normal Dice";
    }
}

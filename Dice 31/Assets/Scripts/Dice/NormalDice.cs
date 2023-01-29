using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalDice : Dice
{
    public int value { get; private set; }
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
        GameManager.Inst.pm.UpdateCurCount(value);
    }
}

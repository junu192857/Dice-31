using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalDice : Dice
{
    public int value { get; private set; }

    private CheckNum cn;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i =>
        {
            value = i;
            Debug.Log($"You rolled {value} from {diceName}");
        });

    }

    public override void EffectBeforeNextPlayerRoll()
    {
        // NOP
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        GameManager.Inst.pm.UpdateCurCount(value);
    }

    private void Awake()
    {
        diceName = "Normal Dice";
        color = DiceColor.Yellow;
    }
}

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
        Debug.Log($"You rolled {value} from {diceName}");
        GameManager.Inst.pm.curCount += value;
    }

    public void Start() {
        color = Color.Yellow;
        diceName = "NormalDice";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i => value = i == 6 ? 20 : i);
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        GameManager.Inst.pm.ExtendMaxCount(value);
    }

    private void Start()
    {
        color = Color.Green;
        diceName = "Extend";
    }
}

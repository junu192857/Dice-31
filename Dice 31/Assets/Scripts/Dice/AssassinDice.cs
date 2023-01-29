using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinDice : Dice
{
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i =>
        {
        });
    }

    public override void EffectBeforeNextPlayerRoll()
    {
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        // NOP
    }

    private void Start()
    {
        color = Color.Red;
        diceName = "Assassin";
    }
}

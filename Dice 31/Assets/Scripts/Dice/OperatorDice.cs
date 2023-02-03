using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorDice : Dice
{
    private bool pass;
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => DiceOperation(i));
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        if (pass) {
            Debug.Log($"Former player rolled 2++ from {diceName}");
            GameManager.Inst.pm.UpdateCurCount(value);
        }
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        if (!pass) {
            Debug.Log($"You rolled --3 from {diceName}");
            GameManager.Inst.pm.UpdateCurCount(value);
        }
    }
    private void DiceOperation(int input)
    {
        switch (input) {
            case 1:
                pass = false;
                value = -3;
                break;
            case 2:
                pass = true;
                value = 2;
                break;
        }
    }

    private void Awake()
    {
        diceName = "Operator Dice";
        color = DiceColor.Green;
    }
}

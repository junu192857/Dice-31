using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMyOwnDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => { 
            value = i;
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });
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
        koreanDiceName = "�� ����� �ֻ���";
        diceInformation = "1�� 2 �� ���ϴ� ���ڸ� ������� �� �� �ִ� �ֻ���";
        color = DiceColor.Green;
    }
}

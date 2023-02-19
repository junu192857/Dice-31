using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorDice : Dice
{
    private bool pass;
    private int value;
    public bool delayed;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => { 
            DiceOperation(i);
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        if (pass) {
            Debug.Log($"Former player rolled 2++ from {diceName}");
            GameManager.Inst.pm.UpdateCurCount(value);
            GameManager.Inst.pm.OperatorDiceResult();
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
                delayed = false;
                break;
            case 2:
                pass = true;
                value = 2;
                delayed = true;
                break;
        }
    }

    private void Awake()
    {
        diceName = "Operator Dice";
        koreanDiceName = "딜레이 주사위";
        diceInformation = "내가 뽑은 숫자를 다음 사람에게 적용시킬 수 있는 주사위";
        color = DiceColor.Green;
        diceIndex = 5;
    }
    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
}

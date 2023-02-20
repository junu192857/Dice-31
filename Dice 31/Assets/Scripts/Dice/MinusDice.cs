using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinusDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => { 
            DiceOperation(i);
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });
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

    private void DiceOperation(int input) {
        switch (input) {
            case 1:
                value = -1;
                break;
            case 2:
                value = -2;
                break;
            case 3:
                value = -3;
                break;
            default:
                Debug.Log($"Unexpected Input {input}");
                break;
        }
    }

    private void Awake()
    {
        diceName = "Minus Dice";
        koreanDiceName = "마이너스 주사위";
        diceInformation = "0보다 작은 숫자를 가진 주사위";
        color = DiceColor.Green;
        diceIndex = 2;
    }
    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
    protected override void Start(){
        base.Start();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDice : Dice
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
        Debug.Log("bomb activated: " + value);
        GameManager.Inst.pm.bombDiceNum = value;
        GameManager.Inst.um.ActivateBomb(value);
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
    }

    private void Awake()
    {
        diceName = "Bomb Dice";
        koreanDiceName = "폭탄 주사위";
        diceInformation = "특정 숫자가 나오면 터지는 폭탄을 담은 주사위";
        color = DiceColor.Red;
        diceIndex = 8;
    }

    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
}

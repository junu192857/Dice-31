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
        koreanDiceName = "��ź �ֻ���";
        diceInformation = "Ư�� ���ڰ� ������ ������ ��ź�� ���� �ֻ���";
        color = DiceColor.Red;
        diceIndex = 8;
    }

    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
}

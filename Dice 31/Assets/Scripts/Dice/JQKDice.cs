using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JQKDice : Dice
{
    private int value;

    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i =>
        {
            value = i;
            switch (i)
            {
                case 1:
                    Debug.Log("J: jump next player");
                    break;
                case 2:
                    Debug.Log("Q: change turn direction");
                    break;
                case 3:
                    Debug.Log("K: roll again");
                    break;
            }
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        switch (value)
        {
            case 1:
                GameManager.Inst.pm.UpdatePlayerIndex(1);
                break;
            case 2:
                GameManager.Inst.pm.turnDirection = -GameManager.Inst.pm.turnDirection;
                break;
            case 3:
                GameManager.Inst.pm.UpdatePlayerIndex(-1);
                break;
        }
    }

    private void Awake()
    {
        diceName = "JQK Dice";
        koreanDiceName = "JQK �ֻ���";
        diceInformation = "����, ���� ��ȯ, �ٽ� �� ���� 3���� ����� ���� �ֻ���";
        color = DiceColor.Green;
        diceIndex = 4;
    }
    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
}
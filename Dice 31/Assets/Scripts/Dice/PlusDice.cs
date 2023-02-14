using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusDice : Dice
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
        Debug.Log($"You rolled {value} from {diceName}");
        GameManager.Inst.pm.UpdateCurCount(value);
    }

    private void Awake()
    {
        diceName = "Plus Dice";
        koreanDiceName = "플러스 주사위";
        diceInformation = "1부터 3까지의 숫자를 추가로 굴릴 수 있는 주사위";
        color = DiceColor.Green;
    }
}

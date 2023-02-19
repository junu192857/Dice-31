using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalDice : Dice
{
    public int value { get; private set; }

    private CheckNum cn;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i =>
        {
            value = i;
            Debug.Log($"You rolled {value} from {diceName}");
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });

    }

    public override void EffectBeforeNextPlayerRoll()
    {
        // NOP
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        GameManager.Inst.pm.UpdateCurCount(value);
    }

    private void Awake()
    {
        diceName = "Normal Dice";
        koreanDiceName = "일반 주사위";
        diceInformation = "1부터 6까지의 숫자가 있는 평범한 주사위";
        color = DiceColor.Yellow;
        diceIndex = 0;
    }

    protected override void OnCollisionEnter() { 
        base.OnCollisionEnter();
    }
}

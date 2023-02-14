using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => { 
            value = i == 6 ? 20 : i;
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
        });
        
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        Debug.Log($"You rolled {value} from Extend Dice");
        GameManager.Inst.pm.ExtendMaxCount(value);
    }

    private void Awake()
    {
        diceName = "Extend Dice";
        koreanDiceName = "���� �ֻ���";
        diceInformation = "�̹� ������ �ִ� ���ڸ� ������Ű�� �ֻ���";
        color = DiceColor.Green;
    }
}

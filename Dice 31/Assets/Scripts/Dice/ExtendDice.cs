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
        koreanDiceName = "연장 주사위";
        diceInformation = "이번 라운드의 최대 숫자를 증가시키는 주사위";
        color = DiceColor.Green;
        diceIndex = 3;
    }
    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
    protected override void Start(){
        base.Start();
    }
}

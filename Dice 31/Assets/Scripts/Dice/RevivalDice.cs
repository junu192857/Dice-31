using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalDice : Dice
{
    private bool success;
    private string _success { 
        get => success ? "Revive" : "Die";
    }
    
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => DiceOperation(i));
    }

    public override void EffectBeforeNextPlayerRoll()
    {
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        Debug.Log($"You rolled {_success} from {diceName}");
        
        GameManager.Inst.pm.OperateRevivalDice(success);
    }
    
    private void DiceOperation(int input) {
        if (input == 1) {
            success = true;
        }
        else {
            success = false;
        }
    }

    private void Awake()
    {
        diceName = "Revival Dice";
        color = DiceColor.Red;
    }
}

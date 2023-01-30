using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AssassinDice : Dice
{
    private int value;
    private Player diceOwner;
    public override IEnumerator Roll()
    {
        //GameManager.Inst.pm.assassinInfo는 테스트용 UI 작성을 위해 임시로 추가한 것
        return DiceUtil.Roll(diceName, i =>
        {
            value = i;
        switch (value)
        {
            case 1: case 2:
                Debug.Log("next player die if x < 5");
                GameManager.Inst.pm.assassinInfo = "x < 5";
                break;
            case 3: case 4:
                Debug.Log("next player die if x > 2");
                GameManager.Inst.pm.assassinInfo = "x > 2";
                break;
            case 5: case 6:
                Debug.Log("next player die if x % 3 != 0");
                GameManager.Inst.pm.assassinInfo = "x % 3 != 0";
                break;
        }
        });
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        diceOwner = GameManager.Inst.pm.activatedPlayer;
    }

    public override void EffectAfterNextPlayerRoll()
    {
        int x = GameManager.Inst.pm.activatedPlayer.normalDice.value;
        bool assassinResult = false;
        switch (value)
        {
            case 1: case 2:
                assassinResult = x < 5;
                break;
            case 3: case 4:
                assassinResult = x > 2;
                break;
            case 5: case 6:
                assassinResult = x % 3 != 0;
                break;
            default:
                Assert.IsTrue(false, "dice value must be in 1 ~ 6");
                break;
        }

        if (assassinResult)
        {
            Debug.Log("assassin success");
            GameManager.Inst.pm.CurrentPlayerDie();
        }
        else
        {
            Debug.Log("assassin fail");
            GameManager.Inst.pm.PlayerDie(diceOwner);
        }

        GameManager.Inst.pm.assassinInfo = "";
    }
}

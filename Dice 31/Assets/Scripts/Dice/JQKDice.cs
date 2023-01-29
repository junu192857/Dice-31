using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JQKDice : Dice
{
    private int value;

    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(diceName, i =>
        {
            value = i;
            switch (i)
            {
                case 1:
                case 2:
                    Debug.Log("J: jump next player");
                    break;
                case 3:
                case 4:
                    Debug.Log("Q: change turn direction");
                    break;
                case 5:
                case 6:
                    Debug.Log("K: roll again");
                    break;
            }
        });
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        switch (value)
        {
            case 1:
            case 2:
                GameManager.Inst.pm.UpdatePlayerIndex(1);
                break;
            case 3:
            case 4:
                GameManager.Inst.pm.turnDirection = -GameManager.Inst.pm.turnDirection;
                break;
            case 5:
            case 6:
                GameManager.Inst.pm.UpdatePlayerIndex(-1);
                break;
        }
    }

    private void Start()
    {
        color = Color.Green;
        diceName = "JQK";
    }
}
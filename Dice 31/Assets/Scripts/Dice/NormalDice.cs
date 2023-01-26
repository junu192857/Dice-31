using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalDice : Dice
{
    private int value;
    public override IEnumerator Roll()
    {
        Debug.Log("Rolling...");
        yield return new WaitForSeconds(1);
        value = Random.Range(1, 7);
        Debug.Log($"You rolled {value} from Normal Dice");
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        GameManager.Inst.pm.curCount += value;
    }
}

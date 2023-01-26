using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDice : Dice
{
    public override void Roll()
    {
        int value = Random.Range(1, 7);
        Debug.Log($"You rolled {value} from Normal Dice");
        GameManager.Inst.pm.curCount += value;
    }

    private void Start()
    {
        color = Color.Yellow;
    }
}

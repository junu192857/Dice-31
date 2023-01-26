using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendDice : Dice
{
    public override IEnumerator Roll()
    {
        yield break;
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        throw new NotImplementedException();
    }
}

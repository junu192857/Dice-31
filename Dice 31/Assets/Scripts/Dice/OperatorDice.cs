using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorDice : Dice
{
    public override IEnumerator Roll()
    {yield break;
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        throw new NotImplementedException();
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        throw new NotImplementedException();
    }
}

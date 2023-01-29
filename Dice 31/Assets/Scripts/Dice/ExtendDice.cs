using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendDice : Dice
{
    public override IEnumerator Roll()
    {
        yield break;
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        throw new System.NotImplementedException();
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
    }

    private void Start()
    {
        color = Color.Green;
        diceName = "Extend";
    }
}

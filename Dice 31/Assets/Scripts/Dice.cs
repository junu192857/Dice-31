using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dice : MonoBehaviour
{
    public abstract IEnumerator Roll();

    public void EffectBeforeNextPlayerRoll()
    {
    }

    public abstract void EffectAfterCurrentPlayerRoll();
}

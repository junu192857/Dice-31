using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color {
    Yellow,
    Green,
    Red,
    Purple
}

public abstract class Dice : MonoBehaviour
{
    public abstract IEnumerator Roll();

    public abstract void EffectBeforeNextPlayerRoll();
    public abstract void EffectAfterCurrentPlayerRoll();
    public Color color;
    public string diceName;
}

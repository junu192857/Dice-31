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


    public virtual void EffectAfterCurrentPlayerRoll() { }
    public virtual void EffectBeforeNextPlayerRoll() { }
    public virtual void EffectAfterNextPlayerRoll() { }

    public Color color;
    public string diceName;
}

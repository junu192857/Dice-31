using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceColor {
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

    public DiceColor color;
    public string diceName;
    //밑의 두 개는 툴팁에서 사용하는 변수
    public string koreanDiceName;
    public string diceInformation;
    public bool available { get; private set; }

    public void EnableDice() {
        available = true;
    }

    public void DisableDice() {
        available = false;
    }
}

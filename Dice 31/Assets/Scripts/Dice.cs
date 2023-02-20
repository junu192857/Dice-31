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
    public int diceIndex;
    //???? ?? ???? ???????? ?????? ????
    public string koreanDiceName;
    public string diceInformation;

    public bool currentlyRolling = false;
    public bool available { get; private set; }

    public void EnableDice() {
        available = true;
    }

    public void DisableDice() {
        available = false;
    }

    protected virtual void OnCollisionEnter() {
        if (GameManager.Inst.gsm.State == GameState.DiceRolling && currentlyRolling)
        {
            float velocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            if (velocity > 0.01f)
                GameManager.Inst.sm.DiceCollision(velocity);
        }
    }
}

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
    public string koreanDiceName;
    public string diceInformation;

    public AudioSource audioSource;
    public int audioPlayedCount;
    private float former;
    private float after;

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
            after = Time.time;
            if (after - former > 0.1f)
            {
                audioSource.volume = Mathf.Pow(0.9f, audioPlayedCount) * GameManager.Inst.sm.SFXVolume;
                audioSource.Play();
                audioPlayedCount++;
                former = Time.time;
            }
        }
    }
    protected virtual void Start(){
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = GameManager.Inst.sm.SFXList[0];
        audioPlayedCount = 0;
        former = 0;
        after = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedDice : Dice
{
    private bool corrupted;
    private bool pass => !corrupted;
    private Player owner;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => { 
            corrupted = i == 1;
            GameManager.Inst.um.ShowNumberAnimate(gameObject, i);
        });

    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        if (corrupted)
        {
            if (++GameManager.Inst.pm.corruptStack == 5)
            {
                GameManager.Inst.pm.CurrentPlayerDie(DeadCause.Corrupted);
            }
            Debug.Log($"corrupted: {GameManager.Inst.pm.corruptStack}");
        }
        else
        {
            Debug.Log($"corrupt pass: {GameManager.Inst.pm.corruptStack}");
            owner = GameManager.Inst.pm.activatedPlayer;
        }
    }

    public override void EffectBeforeNextPlayerRoll()
    {
        if (pass)
        {
            Player current = GameManager.Inst.pm.activatedPlayer;
            owner.specialDice = current.specialDice;
            current.specialDice = this;
        }
    }

    private void Awake()
    {
        diceName = "Corrupted Dice";
        koreanDiceName = "타락 주사위";
        diceInformation = "타락의 기운을 담고 있는 주사위";
        color = DiceColor.Purple;
    }
}

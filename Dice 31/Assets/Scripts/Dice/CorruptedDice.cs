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
        return DiceUtil.Roll(diceName, i => corrupted = i <= 2);
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        if (corrupted)
        {
            if (++GameManager.Inst.pm.corruptStack == 5)
            {
                GameManager.Inst.gsm.OperateGameOver();
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

    private void Start()
    {
        color = Color.Purple;
        diceName = "Corrupted Dice";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedDice : Dice
{
    private bool corrupted;
    public bool pass => !corrupted;
    private Player owner;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i => {
            corrupted = i == 1;
            //corrupted = true;
            GameManager.Inst.um.ShowNumberAnimate(gameObject, i);
        });

    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        if (corrupted)
        {
            GameManager.Inst.pm.corruptStack++;
            if (GameManager.Inst.pm.corruptStack == 4)
            {
                GameManager.Inst.um.GlowFadeIn();
            }
            else if (GameManager.Inst.pm.corruptStack == 5)
            {
                GameManager.Inst.pm.CurrentPlayerDie(DeadCause.Corrupted);
                GameManager.Inst.um.GlowFadeOut();
            }
            Debug.Log($"corrupted: {GameManager.Inst.pm.corruptStack}");
        }
        else
        {
            Debug.Log($"corrupt pass: {GameManager.Inst.pm.corruptStack}");
            owner = GameManager.Inst.pm.activatedPlayer;
            if (owner.unDead) {
                GameManager.Inst.pm.CurrentPlayerDie(DeadCause.Corrupted);
            }
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
        koreanDiceName = "Ÿ�� �ֻ���";
        diceInformation = "Ÿ���� ����� ��� �ִ� �ֻ���";
        color = DiceColor.Purple;
        diceIndex = 10;
    }

    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }
    protected override void Start(){
        base.Start();
    
    }
}

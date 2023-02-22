using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public enum AssassinInfo
{
    Bow,
    Sword,
    Gun,
    None
}
public class AssassinDice : Dice
{
    private int value;
    private Player diceOwner;
    public override IEnumerator Roll()
    {
        return DiceUtil.Roll(this, diceName, i =>
        {
            value = i;
            GameManager.Inst.um.ShowNumberAnimate(gameObject, value);
            switch (value)
        {
            case 1:
                Debug.Log("next player die if x < 5");
                GameManager.Inst.pm.assassinInfo = AssassinInfo.Bow;
                break;
            case 2:
                Debug.Log("next player die if x > 2");
                GameManager.Inst.pm.assassinInfo = AssassinInfo.Sword;
                break;
            case 3:
                Debug.Log("next player die if x % 3 != 0");
                GameManager.Inst.pm.assassinInfo = AssassinInfo.Gun;
                break;
        }
        });
    }

    public override void EffectAfterCurrentPlayerRoll()
    {
        DisableDice();
        diceOwner = GameManager.Inst.pm.activatedPlayer;
    }

    public override void EffectBeforeNextPlayerRoll() {
        switch (value) {
            case 1:
                GameManager.Inst.um.ActivateBow();
                break;
            case 2:
                GameManager.Inst.um.ActivateSword();
                break;
            case 3:
                GameManager.Inst.um.ActivateGun();
                break;
        }
    }
    public override void EffectAfterNextPlayerRoll()
    {
        int val = GameManager.Inst.pm.activatedPlayer.normalDice.value;
        int x = GameManager.Inst.pm.corruptStack >= 4 ? val * 2 : val;
        bool assassinResult = false;
        switch (value)
        {
            case 1:
                assassinResult = 0 < x && x < 5;
                break;
            case 2:
                assassinResult = 6 >= x && x > 2;
                break;
            case 3:
                assassinResult = x % 3 != 0 && 0 < x && x <= 6;
                break;
            default:
                Assert.IsTrue(false, "dice value must be in 1 ~ 6");
                break;
        }

        if (assassinResult)
        {
            Debug.Log("assassin success");
            GameManager.Inst.pm.CurrentPlayerDie(DeadCause.Assassin);
        }
        else
        {
            Debug.Log("assassin fail");
            GameManager.Inst.pm.PlayerDie(diceOwner, DeadCause.AssassinFail);
        }
        GameManager.Inst.um.AssassinFinish();
    }

    private void Awake()
    {
        diceName = "Assassin Dice";
        koreanDiceName = "암살 주사위";
        diceInformation = "다음 사람을 암살할 수 있는 강력한 주사위";
        color = DiceColor.Red;
        diceIndex = 7;
    }

    protected override void OnCollisionEnter()
    {
        base.OnCollisionEnter();
    }

    protected override void Start(){
        base.Start();
    }
}

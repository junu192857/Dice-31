using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DiceUtil
{
    public static bool rollingNormal = false;
    public static bool rollingSpecial = false;
    public static bool normalDone = true;
    public static bool specialDone = true;

    public static bool WaitingOMO = false;
    public static IEnumerator Roll(Dice dice, String diceName, Action<int> callback)
    {
        if (diceName == "Normal Dice")
        {
            rollingNormal = true;
            normalDone = false;
        }
        else {
            rollingSpecial = true;
            specialDone = false;
        }
        DiceController controller = dice.GetComponent<DiceController>();
        int value;
        Debug.Log("Rolling " + diceName);
        yield return new WaitUntil(() => controller.maxFace != 0);
        if (diceName == "On My Own Dice") {
            WaitingOMO = true;
            if (GameManager.Inst.pm.activatedPlayer.isBot)
            {
                if (Random.value > 0.5f)
                    GameManager.Inst.pm.SelectNumberOne();
                else
                    GameManager.Inst.pm.SelectNumberTwo();
            }
            else
            {
                GameManager.Inst.um.ShowOMOButton();
                yield return new WaitWhile(() => WaitingOMO);
            }
            value = GameManager.Inst.pm.onMyOwnDiceNum;
        }
        else value = controller.maxFace;
        callback(value);
        yield return new WaitUntil(() => normalDone && specialDone);
    }

}
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DiceUtil
{
    public static IEnumerator Roll(Dice dice, String diceName, Action<int> callback)
    {
        DiceController controller = dice.GetComponent<DiceController>();
        int value;
        Debug.Log("Rolling " + diceName);
        yield return new WaitUntil(() => controller.maxFace != 0);
        if (diceName == "On My Own Dice") {
            GameManager.Inst.gsm.WaitForNumberSelect();
            while (GameManager.Inst.gsm.State == GameState.WaitingForNumber) {
                yield return null;
            }
            value = GameManager.Inst.pm.onMyOwnDiceNum;
        }
        else value = controller.maxFace;
        callback(value);
    }
}
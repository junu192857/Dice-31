using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DiceUtil
{
    public static IEnumerator Roll(String diceName, Action<int> callback)
    {
        int value;
        Debug.Log("Rolling " + diceName);
        yield return new WaitForSeconds(0.01f);
        if (diceName == "On My Own") {
            GameManager.Inst.gsm.WaitForNumberSelect();
            while (GameManager.Inst.gsm.State == GameState.WaitingForNumber) {
                yield return null;
            }
            value = GameManager.Inst.pm.onMyOwnDiceNum;
        }
        else value = Random.Range(1, 7);
        callback(value);
    }
}
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DiceUtil
{
    public static IEnumerator Roll(String diceName, Action<int> callback)
    {
        Debug.Log("Rolling " + diceName);
        yield return new WaitForSeconds(0.01f);
        int value = Random.Range(1, 7);
        Debug.Log($"You rolled {value} from Normal Dice");
        callback(value);
    }
}
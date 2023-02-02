using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNum : MonoBehaviour
{
    public GameObject[] DiceFaceList;
    public int[] DiceNumList;

    private int listCount;

    private void Start()
    {
        listCount = DiceFaceList.Length;
    }

    public int GetResultNum()
    {
        int resultNum = 0;
        float highTemp = 0;
        int i;

        for(i = 0; i < listCount; i++)
        {
            var y = DiceFaceList[i].transform.position.y; 
            if (y > highTemp)
            {
                highTemp = y;
                resultNum = i;
            }
        }
        return DiceNumList[resultNum];
    }
}

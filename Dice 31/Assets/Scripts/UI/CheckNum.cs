using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNum : MonoBehaviour
{
    public GameObject[] DiceFaceList;
    public int[] DiceNumList;

    private int listCount;
    private GameObject diceTemp;
    private float highTemp;
    public int ResultNum;

    private void Start()
    {
        listCount = DiceFaceList.Length;
    }

    private int GetResultNum()
    {
        ResultNum = 0;
        highTemp = 0;
        int i;

        for(i=0; i < listCount; i++)
        {
            diceTemp = DiceFaceList[i];
            if (diceTemp.transform.position.y > highTemp)
            {
                highTemp = diceTemp.transform.position.y;
                ResultNum = i;
            }
        }
        return DiceNumList[i];
    }
}

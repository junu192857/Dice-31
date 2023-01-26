using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedDice : Dice
{
    public override void Roll()
    {
        
    }

    private void Start()
    {
        color = Color.Purple;
    }
}

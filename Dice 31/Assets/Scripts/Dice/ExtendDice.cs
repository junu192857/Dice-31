using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendDice : Dice
{
    public override void Roll() { }

    private void Start()
    {
        color = Color.Green;
    }
}

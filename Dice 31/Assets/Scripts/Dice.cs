using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Color {
    Yellow,
    Green,
    Red,
    Purple
}
public abstract class Dice : MonoBehaviour
{
    public Color color;
    public abstract void Roll();
}

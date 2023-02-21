using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corrupting : MonoBehaviour
{
    public void CorruptingDone() {
        GameManager.Inst.um.corruptAnimationDone = true;
    }
}

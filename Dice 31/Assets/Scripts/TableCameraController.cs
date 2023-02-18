using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TableCameraController : MonoBehaviour
{
    public UnityEvent AnimationEndEvent;
    public UnityEvent BackToMainEvent;

    public void HandleAnimationEnd(int i)
    {
        if (i == 0)
            AnimationEndEvent.Invoke();
        else if (i == 1)
            BackToMainEvent.Invoke();
    }
}

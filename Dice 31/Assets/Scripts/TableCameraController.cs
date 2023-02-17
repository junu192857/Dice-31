using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TableCameraController : MonoBehaviour
{
    public UnityEvent AnimationEndEvent;

    public void HandleAnimationEnd()
    {
        AnimationEndEvent.Invoke();
    }
}

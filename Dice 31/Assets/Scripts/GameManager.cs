using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public GameStateManager gsm { get; private set; }
    public UIManager um { get; private set; }
    public PlayManager pm { get; private set; }
    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(this);
            return;
        }

        Inst = this;

        gsm = gameObject.GetComponentInChildren<GameStateManager>();
        um = gameObject.GetComponentInChildren<UIManager>();
        pm = gameObject.GetComponentInChildren<PlayManager>();
    }

}

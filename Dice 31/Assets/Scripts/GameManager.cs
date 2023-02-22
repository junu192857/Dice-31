using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode { 
    Drag,
    OneClick
}
public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }

    public static GameMode gameMode;
    public GameStateManager gsm { get; private set; }
    public UIManager um { get; private set; }
    public PlayManager pm { get; private set; }
    public SoundManager sm {get; private set;}
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
        sm = gameObject.GetComponentInChildren<SoundManager>();
    }

    public static void Destroy()
    {
        if (Inst == null) return;
        Destroy(Inst.gameObject);
        Inst = null;
    }
}

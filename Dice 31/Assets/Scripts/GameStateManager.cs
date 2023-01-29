using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { 
    Waiting,
    BeforePlayerTurn,
    InPlayerTurn,
    WaitingForInput,
    WaitingForNumber,
    DiceRolling,
    Gameover
}

public class GameStateManager : MonoBehaviour
{
    public GameState State { get; private set; }

    public void PrepareGame() {
        State = GameState.Waiting;
    }

    public void WaitForPlayerTurn()
    {
        State = GameState.BeforePlayerTurn;
    }

    public void BeginPlayerTurn() {
        State = GameState.InPlayerTurn;
    }

    public void WaitForInput() {
        State = GameState.WaitingForInput;
    }

    public void BeginRoll()
    {
        State = GameState.DiceRolling;
    }
    
    public void OperateGameOver()
    {
        State = GameState.Gameover;
    }

    public void WaitForNumberSelect() {
        State = GameState.WaitingForNumber;
    }
}

using System;
using UnityEngine;
using Utils;

[DefaultExecutionOrder(-10)]
public sealed class GameManager : Singleton<GameManager>
{
    private Action<GameStates> _eventHandler;
    public GameStates GameState { get; private set; }

    public void Subscribe(Action<GameStates> function)
    {
        _eventHandler += function;
    }

    public void Unsubscribe(Action<GameStates> function)
    {
        _eventHandler -= function;
    }

    public void SetGameState(GameStates state)
    {
        if (GameState == state) return;

        GameState = state;
        _eventHandler.Invoke(GameState);
    }
}

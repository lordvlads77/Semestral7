using System;
using UnityEngine;
using consts;

[DefaultExecutionOrder(-10)]
public sealed class GameManager : Utils.Singleton<GameManager>
{
    [SerializeField] private Action<consts.GameStates> eventHandler;
    [SerializeField] public GameStates gameState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
    }

    public void subscirbe(Action<consts.GameStates> function)
    {
        eventHandler += function;
    }

    public void unsubscirbe(Action<consts.GameStates> function)
    {
        eventHandler -= function;
    }

    public void setGameState(consts.GameStates state)
    {
        if (gameState == state) return;

        gameState = state;
        eventHandler.Invoke(gameState);
    }
}

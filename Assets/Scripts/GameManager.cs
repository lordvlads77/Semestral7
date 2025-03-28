using System;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;
using Utils;

[DefaultExecutionOrder(-10)]
public sealed class GameManager : Singleton<GameManager>
{
    private Action<GameStates> _eventHandler;
    public GameStates GameState { get; private set; }
    
    [Header("Shared Scriptables")]
    public WeaponStats weaponStats;
    public RandomNames randomNames;
    public CanvasPrefabs canvasPrefabs;
    public Canvas NpcCanvas { get; private set; }

    protected override void OnAwake()
    {
        EDebug.Log("GameManager Awake");
        SetGameState(GameStates.Joining);
        if(weaponStats == null)
        {
           weaponStats = Resources.Load<WeaponStats>("Scriptables/DefaultWeaponStats");
        }
    }

    public void Subscribe(Action<GameStates> function)
    {
        _eventHandler += function;
    }

    public void Unsubscribe(Action<GameStates> function)
    {
        _eventHandler -= function;
    }
    
    public Canvas CreateNpcCanvas()
    {
        if (NpcCanvas == null) 
            NpcCanvas = Instantiate(canvasPrefabs.npcCanvas);
        return NpcCanvas;
    }

    public void SetGameState(GameStates state) // Left public for use in a Canvas or what not
    {
        if (GameState == state) return;
        GameState = state;
        _eventHandler.Invoke(GameState);
    }

    public Sprite[] GetRandomSprites()
    {
        var sprite = new List<Sprite>();
        foreach (RandomSprite randSprite in canvasPrefabs.canvasSprites)
        {
            sprite.Add(randSprite.GetRandomSprite());
        }
        return sprite.ToArray();
    }
    
    
}

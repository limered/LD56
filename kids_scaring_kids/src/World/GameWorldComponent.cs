using System;
using System.Collections.Generic;
using Godot;
using NewGameProject.Things;
using NewGameProject.Utils;

namespace NewGameProject.World;

public enum GameState
{
    StartScreen,
    Intro,
    OneEnemy,
    Running,
    EndGame
}

public partial class GameWorldComponent : Node2D
{
    private readonly Dictionary<GameState, List<GameState>> _allowedStates = new()
    {
        { GameState.StartScreen, new List<GameState> { GameState.Intro } },
        { GameState.Intro, new List<GameState> { GameState.OneEnemy } },
        { GameState.OneEnemy, new List<GameState> { GameState.Running } },
        { GameState.Running, new List<GameState> { GameState.EndGame } },
        { GameState.EndGame, new List<GameState> { GameState.StartScreen } }
    };

    private GameState _gameState = GameState.StartScreen;

    private Node2D _visibilityRenderer;
    private ThingsRepository _things;

    public override void _Ready()
    {
        EventBus.Register<GameStateChangeRequestMsg>(ChangeGameState);
        EventBus.Emit(new GameStateChangedMsg { State = GameState.StartScreen });
        _things = GetNode<ThingsRepository>("/root/ThingsRepository");
    }

    private void ChangeGameState(GameStateChangeRequestMsg requestMsg)
    {
        var allowedSates = _allowedStates[_gameState];
        if (allowedSates.Contains(requestMsg.TargetState)) GoToState(requestMsg.TargetState);
    }

    private void GoToState(GameState newState)
    {
        _gameState = newState;
        switch (newState)
        {
            case GameState.StartScreen:
                EventBus.Emit(new GameStateChangedMsg { State = newState });
                break;
            case GameState.Intro:
                EventBus.Emit(new GameStateChangedMsg { State = newState });
                GetTree().CreateTimer(5.0).Timeout += () =>
                    EventBus.Emit(new GameStateChangeRequestMsg { TargetState = GameState.OneEnemy });
                break;
            case GameState.OneEnemy:
                EventBus.Emit(new GameStateChangedMsg { State = newState });
                break;
            case GameState.Running:
                EventBus.Emit(new GameStateChangedMsg { State = newState });
                break;
            case GameState.EndGame:
                EventBus.Emit(new GameStateChangedMsg { State = newState });
                // change to light scene
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public override void _Process(double delta)
    {
        if (_gameState != GameState.Running) return;
        if (_things.Creatures.Count > 0) return;
        EventBus.Emit(new GameStateChangeRequestMsg() { TargetState = GameState.EndGame });
    }
}
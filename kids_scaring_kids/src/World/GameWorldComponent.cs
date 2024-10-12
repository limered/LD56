using System;
using System.Collections.Generic;
using Godot;
using NewGameProject.Utils;
using NewGameProject.Visibility;

namespace NewGameProject.World;

public enum GameState
{
    StartScreen,
    Intro,
    OneEnemy,
    Running,
    EndGame,
}

public partial class GameWorldComponent : Node2D
{
    private GameState _gameState = GameState.StartScreen;
    
    private readonly Dictionary<GameState, List<GameState>> _allowedStates = new ()
    {
        { GameState.StartScreen, new List<GameState> { GameState.StartScreen }},
        { GameState.Intro, new List<GameState> { GameState.OneEnemy }},
        { GameState.OneEnemy, new List<GameState> { GameState.Running }},
        { GameState.Running, new List<GameState> { GameState.EndGame }},
        { GameState.EndGame, new List<GameState> { GameState.StartScreen }},
    };

    private Node2D _visibilityRenderer;

    public override void _Ready()
    {
        EventBus.Emit(new GameStateChangedMsg(){State = GameState.StartScreen});
        EventBus.Register<GameStateChangeRequestMsg>(ChangeGameState);
    }

    private void ChangeGameState(GameStateChangeRequestMsg requestMsg)
    {
        var allowedSates = _allowedStates[_gameState];
        if (allowedSates.Contains(requestMsg.TargetState))
        {
            EventBus.Emit(new GameStateChangedMsg{State = requestMsg.TargetState});
        }
    }

    private void GoToState(GameState newState)
    {
        switch (newState)
        {
            case GameState.StartScreen:
                EventBus.Emit(new GameStateChangedMsg{State = newState});
                // show start screen
                break;
            case GameState.Intro:
                EventBus.Emit(new GameStateChangedMsg{State = newState});
                // turn on light
                // activate movement
                // activate timer to spawn first enemy
                break;
            case GameState.OneEnemy:
                EventBus.Emit(new GameStateChangedMsg{State = newState});
                // spawn first enemy
                break;
            case GameState.Running:
                EventBus.Emit(new GameStateChangedMsg{State = newState});
                // spawn other enemies for some frames
                // maybe: per killed enemy, add some leafs to trees
                break;
            case GameState.EndGame:
                EventBus.Emit(new GameStateChangedMsg{State = newState});
                // change to light scene
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}
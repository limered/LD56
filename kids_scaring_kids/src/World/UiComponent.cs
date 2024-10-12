using Godot;
using NewGameProject.Utils;

namespace NewGameProject.World;

public partial class UiComponent : CanvasLayer
{
    [Export] public ColorRect ConeDisplay { get; set; }
    [Export] public Button StartButton { get; set; }

    public override void _Ready()
    {
        ConeDisplay.SetVisible(true);
        StartButton.SetVisible(true);
        StartButton.ButtonDown += StartButtonOnButtonDown;
    }

    private void StartButtonOnButtonDown()
    {
        StartButton.SetVisible(false);
        EventBus.Emit(new GameStateChangeRequestMsg { TargetState = GameState.Intro });
    }
}
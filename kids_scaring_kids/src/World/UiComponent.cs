using Godot;
using NewGameProject.Things;
using NewGameProject.Utils;

namespace NewGameProject.World;

public partial class UiComponent : CanvasLayer
{
    [Export] public ColorRect ConeDisplay { get; set; }
    [Export] public Button StartButton { get; set; }
    [Export] public RichTextLabel ScoreLabel { get; set; }

    private ThingsRepository _things;

    public override void _Ready()
    {
        ConeDisplay.SetVisible(true);
        StartButton.SetVisible(true);
        StartButton.ButtonDown += StartButtonOnButtonDown;

        _things = GetNode<ThingsRepository>("/root/ThingsRepository");
    }

    private void StartButtonOnButtonDown()
    {
        StartButton.SetVisible(false);
        EventBus.Emit(new GameStateChangeRequestMsg { TargetState = GameState.Intro });
    }

    public override void _Process(double delta)
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        var count = _things.Creatures.Count;
        ScoreLabel.Text = count == 0 ? "" : $"{count}";
    }
}
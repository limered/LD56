using Godot;
using NewGameProject.Things;
using NewGameProject.Utils;

namespace NewGameProject.World;

public partial class UiComponent : CanvasLayer
{
    [Export] public ColorRect ConeDisplay { get; set; }
    [Export] public Button StartButton { get; set; }
    [Export] public RichTextLabel ScoreLabel { get; set; }
    [Export] public RichTextLabel EndLabel { get; set; }

    private ThingsRepository _things;

    public override void _Ready()
    {
        EndLabel.Modulate = new Color(0, 0, 0, 0);
        ConeDisplay.SetVisible(true);
        StartButton.SetVisible(true);
        StartButton.ButtonDown += StartButtonOnButtonDown;

        _things = GetNode<ThingsRepository>("/root/ThingsRepository");
        
        EventBus.Register<GameStateChangedMsg>(GameStateChanged);
    }

    private void GameStateChanged(GameStateChangedMsg obj)
    {
        if (obj.State is GameState.EndGame)
        {
            GetTree().CreateTween()
                .TweenMethod(Callable.From<float>(ModulateEndLabel), 0f, 1f, 2f)
                .SetDelay(2f)
                .SetEase(Tween.EaseType.InOut);
        }
    }

    private void ModulateEndLabel(float value)
    {
        EndLabel.Modulate = new Color(0, 0, 0, value);
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
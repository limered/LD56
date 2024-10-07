using Godot;
using NewGameProject.Animation;

namespace NewGameProject.Player;

public partial class PlayerControlledBrain : Node2D
{
    [Export] public Node2D Root;
    [Export] public LinearAnimationComponent Animation;
    [Export] public AudioStreamPlayer2D Audio;

    private Vector2 _halfScreenSize;

    public override void _Ready()
    {
        _halfScreenSize = GetViewportRect().Size / 2.0f;
    }

    public override void _Process(double delta)
    {
        var direction = Vector2.Zero;
        if (Input.IsActionPressed("Up")) 
            direction += Root.Position.Y <= -_halfScreenSize.Y ? Vector2.Down : Vector2.Up;
        if (Input.IsActionPressed("Left"))
            direction += Root.Position.X <= -_halfScreenSize.X ? Vector2.Right : Vector2.Left;
        if (Input.IsActionPressed("Right"))
            direction += Root.Position.X >= _halfScreenSize.X ? Vector2.Left : Vector2.Right;
        if (Input.IsActionPressed("Down"))
            direction += Root.Position.Y >= _halfScreenSize.Y ? Vector2.Up : Vector2.Down;
        Animation.MovementDirection = direction;
        
        if(direction.Length() > 0.1f)
        {
            if (!Audio.IsPlaying()) Audio.Play();
        }
        else Audio.Stop();

        var mousePosition = GetGlobalMousePosition();
        var lookDirection = (mousePosition - Root.GlobalPosition).Normalized();
        var newAngle = Mathf.Pi + Mathf.Atan2(lookDirection.Y, lookDirection.X);
        Animation.RotationAngle = newAngle;
    }
}
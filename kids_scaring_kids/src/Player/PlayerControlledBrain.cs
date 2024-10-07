using Godot;
using NewGameProject.Animation;

namespace NewGameProject.Player;

public partial class PlayerControlledBrain : Node2D
{
    [Export] public Node2D Root;
    [Export] public LinearAnimationComponent Animation;

    public override void _Process(double delta)
    {
        var direction = Vector2.Zero;
        if(Input.IsActionPressed("Up")) direction += Vector2.Up;
        if(Input.IsActionPressed("Left")) direction += Vector2.Left;
        if(Input.IsActionPressed("Right")) direction += Vector2.Right;
        if(Input.IsActionPressed("Down")) direction += Vector2.Down;
        Animation.MovementDirection = direction;
        
        // check against world size
        

        var mousePosition = GetGlobalMousePosition();
        var lookDirection = (mousePosition - Root.GlobalPosition).Normalized();
        var newAngle = Mathf.Pi + Mathf.Atan2(lookDirection.Y, lookDirection.X);
        Animation.RotationAngle = newAngle;
    }
}
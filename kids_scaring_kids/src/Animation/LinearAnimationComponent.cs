﻿using Godot;

namespace NewGameProject.Animation;

public partial class LinearAnimationComponent : Node
{
    [Export] public Node2D Root;
    [Export] public Sprite2D Sprite;
    [Export] public float Speed;
    
    public Vector2 MovementDirection;
    public float RotationAngle;

    public override void _PhysicsProcess(double delta)
    {
        Root.GlobalPosition += MovementDirection * Speed * (float)delta;

        Sprite.Rotation = RotationAngle;
        Sprite.RotationDegrees -= 90;
    }
}
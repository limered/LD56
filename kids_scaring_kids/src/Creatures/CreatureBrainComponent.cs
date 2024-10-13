using Godot;
using NewGameProject.Animation;
using NewGameProject.Things;
using NewGameProject.Utils;
using NewGameProject.World;

namespace NewGameProject.Creatures;

public partial class CreatureBrainComponent : Node2D
{
    [Export] public LinearAnimationComponent Animation;
    [Export] public AudioStreamPlayer2D Plop;
    [Export] public Node2D Root;
    public bool EyesClosed { get; private set; }
    public bool IsFirstCreep { get; set; }
    public bool HitRegistered { get; set; }
    
    private Vector2 _halfScreenSize;
    private Node2D _hero;
    private StaticOcclusionComponent _lastTree;
    private Vector2? _target;
    private ThingsRepository _things;
    
    private CreatureState _state = CreatureState.Roaming;
    private double _timeToStayHidden;
    private float _timeInLight = 0.5f;
    private bool _isDying;
    private bool _hasDied;

    public override void _Ready()
    {
        _halfScreenSize = GetViewportRect().Size / 2.0f;
        _things = GetNode<ThingsRepository>("/root/ThingsRepository");
        _things.Creatures.Add(Root);
    }

    private void CheckForDeath(float deltaTime)
    {
        if (!HitRegistered) return;
        HitRegistered = false;
        _timeInLight -= deltaTime;
        if (_timeInLight <= 0) _isDying = true;
    }

    public override void _Process(double delta)
    {
        if (_hasDied) return;
        if (_isDying)
        {
            _things.Creatures.Remove(Root);

            var sprite = Root.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            sprite.AnimationFinished += SpriteOnAnimationFinished;
            sprite.Play("default");

            Plop.PitchScale = (float)(1.0f + GD.RandRange(-0.2f, 0.2f));
            Plop.Play();

            _hasDied = true;
            return;
        }

        CheckForDeath((float)delta);

        _hero = _things.Hero;
        if (_hero == null) return;

        var distanceToHero = Root.GlobalPosition.DistanceTo(_hero.GlobalPosition);

        if (distanceToHero > 320)
        {
            _state = CreatureState.Roaming;
            EyesClosed = false;
            if (!_target.HasValue)
            {
                _target = Root.GlobalPosition +
                          (Vector2.Right * GD.RandRange(0, 50)).Rotated((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));
                var x = Mathf.Clamp(_target.Value.X, -_halfScreenSize.X, _halfScreenSize.X);
                var y = Mathf.Clamp(_target.Value.Y, -_halfScreenSize.Y, _halfScreenSize.Y);
                _target = new Vector2(x, y);
            }
            else
            {
                Animation.MovementDirection = (_target.Value - Root.GlobalPosition)
                    .Normalized();
                if (Root.GlobalPosition.DistanceSquaredTo(_target.Value) <= 5f) _target = null;
            }
        }
        else
        {
            if (IsFirstCreep)
            {
                IsFirstCreep = false;
                EventBus.Emit(new GameStateChangeRequestMsg { TargetState = GameState.Running });
            }

            if (_state == CreatureState.Hiding && _target.HasValue)
            {
                if (Root.GlobalPosition.DistanceSquaredTo(_target.Value) <= 2f)
                {
                    EyesClosed = true;
                    Animation.MovementDirection = Vector2.Zero;
                }
                else
                {
                    Animation.MovementDirection = (_target.Value - Root.GlobalPosition)
                        .Normalized();
                }

                _timeToStayHidden -= delta;
                if (_timeToStayHidden <= 0) _state = CreatureState.RunAway;
            }
            else
            {
                _state = CreatureState.Hiding;
                _timeToStayHidden = GD.RandRange(2f, 5f);

                var trees = _things.Nodes;
                var nearest = trees[0];
                var nearestDistance = nearest.GlobalPosition.DistanceSquaredTo(Root.GlobalPosition);
                for (var i = 1; i < trees.Count; i++)
                {
                    var tree = trees[i];
                    var currDistance = tree.GlobalPosition.DistanceSquaredTo(Root.GlobalPosition);
                    if (currDistance < nearestDistance && tree != _lastTree)
                    {
                        nearestDistance = currDistance;
                        nearest = tree;
                    }
                }

                var directionToHero = (Root.GlobalPosition - _hero.GlobalPosition).Normalized();
                _target = nearest.GlobalPosition + directionToHero * 30f;
                _lastTree = nearest;
                Animation.MovementDirection = (_target.Value - Root.GlobalPosition)
                    .Normalized();
            }
        }

        var newX = Mathf.Clamp(Root.GlobalPosition.X, -_halfScreenSize.X, _halfScreenSize.X);
        var newY = Mathf.Clamp(Root.GlobalPosition.Y, -_halfScreenSize.Y, _halfScreenSize.Y);
        Root.GlobalPosition = new Vector2(newX, newY);
    }

    private void SpriteOnAnimationFinished()
    {
        Root.QueueFree();
    }
}
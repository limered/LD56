using System.Collections.Generic;
using Godot;
using NewGameProject.Animation;
using NewGameProject.Creatures;
using NewGameProject.Things;
using NewGameProject.Utils;
using NewGameProject.Visibility;
using NewGameProject.World;

namespace NewGameProject.Player;

public partial class LightCaster : Node2D
{
    private ThingsRepository _repo;
    [Export] public LinearAnimationComponent Animation;
    [Export] public Node2D Root;
    [Export] public int MaxRayLength;

    public override void _Ready()
    {
        _repo = GetNode<ThingsRepository>("/root/ThingsRepository");
        _repo.Hero = Root;
        MaxRayLength = 0;
        EventBus.Register<GameStateChangedMsg>(OnGameChanged);
    }

    private void OnGameChanged(GameStateChangedMsg obj)
    {
        if (obj.State == GameState.Intro)
        {
            var tween = GetTree().CreateTween();
            tween.TweenMethod(Callable.From<int>(AnimateCone), 0, 300, 1.0f)
                .SetEase(Tween.EaseType.InOut);

        }
        else if (obj.State is GameState.EndGame or GameState.StartScreen)
        {
            MaxRayLength = 0;
        }
    }
    
    private void AnimateCone(int val)
    {
        MaxRayLength = val;
    }

    public override void _PhysicsProcess(double delta)
    {
        var spaceState = GetWorld2D().DirectSpaceState;

        const float coneAngle = 0.30f;
        const float steps = 200f;
        const float stepSize = coneAngle / steps;
        var cone = new List<Vector2> { Root.GlobalPosition };
        for (var i = 0; i < steps; i++)
        {
            var angleDiff = stepSize * i - coneAngle * 0.5f;
            var angle = Animation.RotationAngle + Mathf.Pi * angleDiff - Mathf.Pi;
            var ray = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            var query = PhysicsRayQueryParameters2D.Create(Root.Position, ray * MaxRayLength + Root.Position);
            query.CollideWithAreas = true;
            var result = spaceState.IntersectRay(query);
            
            if(result.Count > 0)
            {
                var area = (Area2D)result["collider"].AsGodotObject();
                if (area.IsInGroup("creatures"))
                {
                    var brain = area.GetParent()?.GetNode<CreatureBrainComponent>("CreatureBraincomponent");
                    if(brain != null)
                    {
                        brain.HitRegistered = true;
                    }
                }
                else
                {
                    cone.Add(result["position"].AsVector2());
                }
            }
            else
            {
                cone.Add(ray * MaxRayLength + Root.Position);
            }
        }

        EventBus.Emit(new VisibilityConeChangedEvent { Points = cone });
    }
}
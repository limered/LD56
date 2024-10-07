using System.Collections.Generic;
using Godot;
using NewGameProject.Animation;
using NewGameProject.Things;
using NewGameProject.Utils;
using NewGameProject.Visibility;

namespace NewGameProject.Player;

public partial class LightCaster : Node2D
{
    private ThingsRepository _repo;
    [Export] public LinearAnimationComponent Animation;
    [Export] public Node2D Root;

    public override void _Ready()
    {
        _repo = GetNode<ThingsRepository>("/root/ThingsRepository");
    }

    public override void _PhysicsProcess(double delta)
    {
        var spaceState = GetWorld2D().DirectSpaceState;

        var maxRayLength = 300;
        var coneAngle = 0.20f;
        var steps = 200f;
        var stepSize = coneAngle / steps;
        var cone = new List<Vector2> { Root.GlobalPosition };
        for (var i = 0; i < steps; i++)
        {
            var angleDiff = stepSize * i - coneAngle * 0.5f;
            var angle = Animation.RotationAngle + Mathf.Pi * angleDiff - Mathf.Pi;
            var ray = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            var query = PhysicsRayQueryParameters2D.Create(Root.Position, ray * maxRayLength + Root.Position);
            query.CollideWithAreas = true;
            var result = spaceState.IntersectRay(query);
            
            if(result.Count > 0)
            {
                cone.Add(result["position"].AsVector2());
            }
            else
            {
                cone.Add(ray * maxRayLength + Root.Position);
            }
        }

        EventBus.Emit(new VisibilityConeChangedEvent { Points = cone });
    }
}
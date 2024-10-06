using Godot;

namespace NewGameProject.Things;

public partial class StaticOcclusionComponent : Node2D
{
    [Export] public CollisionShape2D ColliderShape;

    public override void _Ready()
    {
        var repo = GetNode<ThingsRepository>("/root/ThingsRepository");
        repo.RegisterSelf(this);
    }
}
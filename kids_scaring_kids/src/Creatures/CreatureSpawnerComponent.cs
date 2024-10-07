using Godot;
using NewGameProject.Things;

namespace NewGameProject.Creatures;

public partial class CreatureSpawnerComponent : Node
{
    public override void _Ready()
    {
        var creatureScene = GD.Load<PackedScene>("res://scenes/creature.tscn");
        var trees = GetNode<ThingsRepository>("/root/ThingsRepository").Nodes;
        for (var i = 0; i < trees.Count; i++)
        {
            for(var j = 0; j < 3; j++)
            {
                var creature = creatureScene.Instantiate<Node2D>();
                creature.GlobalPosition = trees[i].GlobalPosition;
                AddChild(creature);
            }
        }
    }
}
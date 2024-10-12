using System.Collections.Generic;
using Godot;
using NewGameProject.Things;
using NewGameProject.Utils;
using NewGameProject.World;

namespace NewGameProject.Creatures;

public partial class CreatureSpawnerComponent : Node
{
    private PackedScene _creatureScene;
    private List<StaticOcclusionComponent> _trees;
    [Export] public Node2D Hero;

    public override void _Ready()
    {
        _creatureScene = GD.Load<PackedScene>("res://scenes/creature.tscn");
        _trees = GetNode<ThingsRepository>("/root/ThingsRepository").Nodes;
        // for (var i = 0; i < trees.Count; i++)
        // {
        //     for(var j = 0; j < 3; j++)
        //     {
        //         var creature = creatureScene.Instantiate<Node2D>();
        //         creature.GlobalPosition = trees[i].GlobalPosition;
        //         AddChild(creature);
        //     }
        // }
        
        EventBus.Register<GameStateChangedMsg>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedMsg obj)
    {
        if (obj.State is GameState.OneEnemy)
        {
            if (Hero == null) return;
            var furthest = _trees[0];
            var maxDistance = Hero.GlobalPosition.DistanceTo(furthest.GlobalPosition);
            for (var i = 1; i < _trees.Count; i++)
            {
                var tree = _trees[i];
                var distance = Hero.GlobalPosition.DistanceTo(tree.GlobalPosition);
                if (distance > maxDistance)
                {
                    furthest = tree;
                    maxDistance = distance;
                }
            }
            var creature = _creatureScene.Instantiate<Node2D>();
            creature.GlobalPosition = furthest.GlobalPosition;
            AddChild(creature);
        }
    }
}
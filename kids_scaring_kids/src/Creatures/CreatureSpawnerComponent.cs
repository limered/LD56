using System.Collections.Generic;
using Godot;
using NewGameProject.Things;
using NewGameProject.Utils;
using NewGameProject.World;

namespace NewGameProject.Creatures;

public partial class CreatureSpawnerComponent : Node
{
    private PackedScene _creatureScene;

    private int _lastVal = -1;
    private List<StaticOcclusionComponent> _trees;
    [Export] public Node2D Hero;

    public override void _Ready()
    {
        _creatureScene = GD.Load<PackedScene>("res://scenes/creature.tscn");
        _trees = GetNode<ThingsRepository>("/root/ThingsRepository").Nodes;
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
            creature.GetNode<CreatureBrainComponent>("CreatureBraincomponent").IsFirstCreep = true;
            AddChild(creature);
        }
        else if (obj.State is GameState.Running)
        {
            GD.Print("Running");
            var tween = GetTree().CreateTween();
            tween.TweenMethod(Callable.From<int>(Spawn), 0, _trees.Count - 1, 3.0);
        }
    }


    private void Spawn(int val)
    {
        if (_lastVal == val) return;
        for (var j = 0; j < 4; j++)
        {
            var creature = _creatureScene.Instantiate<Node2D>();
            creature.GlobalPosition = _trees[val].GlobalPosition;
            AddChild(creature);
        }

        _lastVal = val;
    }
}
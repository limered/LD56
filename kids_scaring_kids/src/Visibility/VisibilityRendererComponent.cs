using System.Collections.Generic;
using Godot;
using NewGameProject.Creatures;
using NewGameProject.Things;
using NewGameProject.Utils;

namespace NewGameProject.Visibility;

public partial class VisibilityRendererComponent : Node2D
{
    [Export] public Polygon2D Polygon2D;
    public List<Vector2> Points = new();

    private ThingsRepository _things;

    public override void _Ready()
    {
        EventBus.Register<VisibilityConeChangedEvent>(SetPoints);
        _things = GetNode<ThingsRepository>("/root/ThingsRepository");
    }

    private void SetPoints(VisibilityConeChangedEvent evt)
    {
        Points.Clear();
        Points.AddRange(evt.Points);
    }

    public override void _Draw()
    {
        var creatureClone = _things.Creatures.ToArray();
        for (var i = 0; i < creatureClone.Length; i++)
        {
            var creature = creatureClone[i];
            if(creature.GetNode<CreatureBrainComponent>("CreatureBraincomponent").EyesClosed) continue;
            var left = creature.GlobalPosition - new Vector2(3, 0);
            var right = creature.GlobalPosition + new Vector2(3, 0);
            DrawCircle(left, 2f, new Color(1f, 1f, 1f));
            DrawCircle(right, 2f, new Color(1f, 1f, 1f));
        }
    }

    public override void _Process(double delta)
    {
        if(Polygon2D != null)
        {
            Polygon2D.Polygon = Points.ToArray();
        }
        QueueRedraw();
    }
}
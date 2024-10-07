using System.Collections.Generic;
using Godot;
using NewGameProject.Utils;

namespace NewGameProject.Visibility;

public partial class VisibilityRendererComponent : Node2D
{
    [Export] public Polygon2D Polygon2D;
    public List<Vector2> Points = new();

    public override void _Ready()
    {
        EventBus.Register<VisibilityConeChangedEvent>(SetPoints);
    }

    private void SetPoints(VisibilityConeChangedEvent evt)
    {
        Points.Clear();
        Points.AddRange(evt.Points);
    }

    // public override void _Draw()
    // {
    //     DrawColoredPolygon(Points.ToArray(), new Color(1, 1, 1));
    // }

    public override void _Process(double delta)
    {
        if(Polygon2D != null)
        {
            Polygon2D.Polygon = Points.ToArray();
        }
    }
}
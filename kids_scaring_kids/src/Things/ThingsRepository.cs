using System.Collections.Generic;
using Godot;

namespace NewGameProject.Things;

public partial class ThingsRepository : Node
{
    public readonly List<StaticOcclusionComponent> Nodes = new();
    public Node2D Hero;
    public List<Node2D> Creatures = new();
    
    public void RegisterSelf(StaticOcclusionComponent node)
    {
        Nodes.Add(node);
    }
}
using System.Collections.Generic;
using Godot;

namespace NewGameProject.Visibility;

public struct VisibilityConeChangedEvent
{
    public List<Vector2> Points { get; init; }
}
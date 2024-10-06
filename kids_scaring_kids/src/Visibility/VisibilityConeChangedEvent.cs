using System.Collections.Generic;
using Godot;

namespace NewGameProject.Visibility;

public class VisibilityConeChangedEvent
{
    public List<Vector2> Points { get; set; }
}
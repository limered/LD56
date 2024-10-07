using Godot;

namespace NewGameProject.Visibility;

public partial class FullscreenQuadComponent : ColorRect
{
    [Export] public SubViewport ConeViewport;
    [Export] public ShaderMaterial FullscreenQuadShader;
    [Export] public Node2D Hero; 

    public override void _Ready()
    {
        var tex = ConeViewport.GetTexture();
        FullscreenQuadShader.SetShaderParameter("light_cone_texture", tex);
        
    }

    public override void _Process(double delta)
    {
        FullscreenQuadShader.SetShaderParameter("hero_position", Hero.Position);
    }
}
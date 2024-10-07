using Godot;

namespace NewGameProject.Visibility;

public partial class FullscreenQuadComponent : ColorRect
{
    [Export] public SubViewport ConeViewport;
    [Export] public ShaderMaterial FullscreenQuadShader;

    public override void _Ready()
    {
        var tex = ConeViewport.GetTexture();
        FullscreenQuadShader.SetShaderParameter("light_cone_texture", tex);
    }
}
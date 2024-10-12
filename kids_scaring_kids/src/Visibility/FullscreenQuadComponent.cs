using Godot;
using NewGameProject.Utils;
using NewGameProject.World;

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
        FullscreenQuadShader.SetShaderParameter("cone_radius_pixel", 0);
        EventBus.Register<GameStateChangedMsg>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedMsg obj)
    {
        if (obj.State == GameState.Intro)
        {
            GD.Print("Intro");
            var tween = GetTree().CreateTween();
            tween.TweenMethod(Callable.From<int>(AnimateCone), 0, 30, 0.1f)
                .SetEase(Tween.EaseType.InOut);
        }
        else if (obj.State is GameState.EndGame or GameState.StartScreen)
        {
            FullscreenQuadShader.SetShaderParameter("cone_radius_pixel", 0);
        }
    }

    private void AnimateCone(int val)
    {
        FullscreenQuadShader.SetShaderParameter("cone_radius_pixel", val);
    }

    public override void _Process(double delta)
    {
        FullscreenQuadShader.SetShaderParameter("hero_position", Hero.Position);
    }
}
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
        ModifyAlpha(0f);
        EventBus.Register<GameStateChangedMsg>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedMsg obj)
    {
        if (obj.State == GameState.Intro)
        {
            var tween = GetTree().CreateTween();
            tween.TweenMethod(Callable.From<int>(AnimateCone), 0, 30, 0.1f)
                .SetEase(Tween.EaseType.InOut);
        }
        else if (obj.State is GameState.StartScreen)
        {
            FullscreenQuadShader.SetShaderParameter("cone_radius_pixel", 0);
        }
        else if (obj.State is GameState.EndGame)
        {
            var tween = GetTree().CreateTween();
            tween.TweenMethod(Callable.From<int>(AnimateCone), 30, 2000, 2f)
                .SetEase(Tween.EaseType.In);
            tween.Parallel()
                .TweenMethod(Callable.From<float>(ModifyAlpha), 0f, 1f, 1.5f)
                .SetEase(Tween.EaseType.In);
        }
    }

    private void ModifyAlpha(float alpha)
    {
        FullscreenQuadShader.SetShaderParameter("alpha_mix_factor", alpha);

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
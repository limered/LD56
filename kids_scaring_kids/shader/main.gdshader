shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D light_cone_texture : repeat_disable, filter_nearest_mipmap;
uniform vec2 hero_position;
uniform float cone_radius_pixel;
uniform float cone_feather;

void vertex() {
}

float circle_mask(vec2 _uv, vec2 _center, float _radius, float _feather) {
    float dist = length(_uv - _center);
    return smoothstep(1.0f-_feather, 1.0f , dist/_radius);
}

void fragment() {
	vec2 screen_size = vec2(1152, 648);
	vec2 screen_half_size = screen_size * 0.5f;
	vec4 game_color = textureLod(screen_texture, SCREEN_UV, 0.0);
	vec4 cone_color = textureLod(light_cone_texture, SCREEN_UV, 0.0);
	vec2 screen_pixel = (SCREEN_UV - 0.5) * screen_size;
	float alpha = circle_mask(screen_pixel, hero_position, cone_radius_pixel, cone_feather); 
    COLOR = vec4(cone_color.xyz, alpha) * game_color;// game_color + cone_color * alpha; ////
}

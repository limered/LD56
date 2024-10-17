shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform sampler2D light_cone_texture : repeat_disable, filter_nearest_mipmap;
uniform vec2 hero_position;
uniform float cone_radius_pixel;
uniform float cone_feather;
uniform float alpha_mix_factor;
uniform float rain_strenght;

void vertex() {
}

float circle_mask(vec2 _uv, vec2 _center, float _radius, float _feather) {
    float dist = length(_uv - _center);
    return smoothstep(1.0f-_feather, 1.0f , dist/_radius);
}

float random (vec2 uv) {
    return fract(sin(dot(uv.xy,
        vec2(12.9898,78.233))) * 43758.5453123);
}

vec4 noise(vec2 uv){
	float val = random(uv * TIME);
	val = clamp(val, 0.5, 0.6);
	vec3 noise_color = vec3(1., 1., 1.);
	return vec4(noise_color * val * 1., 1.);
}

vec4 vignette(vec2 _uv){
	float dist = length(_uv - vec2(.5));
	float x = smoothstep(0.9, 0.2, dist);
	return vec4(x, x, x, 1.0);
}

void fragment() {
	vec2 screen_size = vec2(1152, 648);
	vec2 screen_half_size = screen_size * 0.5f;
	vec4 game_color = textureLod(screen_texture, SCREEN_UV, 0.0);
	vec4 cone_color = textureLod(light_cone_texture, SCREEN_UV, 0.0);
	vec2 screen_pixel = (SCREEN_UV - 0.5) * screen_size;
	float alpha = circle_mask(screen_pixel, hero_position, cone_radius_pixel, cone_feather);
	 
	vec4 game_colors = vec4(cone_color.xyz, alpha) * 
		game_color * 
		noise(SCREEN_UV) * 
		vignette(SCREEN_UV);
	vec4 end_screen = vec4(vec3(1.0-alpha), 1.0);
	COLOR = mix(game_colors, end_screen, alpha_mix_factor);
}

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

float line_sdf(vec2 p, vec2 s){
	vec2 d = abs(p) - s;
	return min(max(d.x, d.y), 0.0) + length(max(d, 0.0));
}

float hash(float x){
	return fract(sin(x * 18.34) * 51.78);
}

float hash2(float x){
	return fract(sin(x * 25.42) * 21.24);
}

vec4 rain(vec2 _uv){
	vec4 res = vec4(0.0);
	float size = 20.0;
	
	vec2 aspect = vec2(2.0, 1.0);
	vec2 uv = _uv * size * aspect;
	//uv.y -= TIME * 10.0;
	vec2 gv = fract(uv) - .5;
	
	float x = 0.0;
	float y = sin(TIME + sin(TIME + sin(TIME) * 0.5)) * 0.45;
	vec2 dropPos = (gv - vec2(x, y)) / aspect;
	
	float drop = smoothstep(.05, .03, length(dropPos));
	if(y < 0.0)
		res.xyz += -drop;
	
	return res;
}

void fragment() {
	vec2 screen_size = vec2(1152, 648);
	vec2 screen_half_size = screen_size * 0.5f;
	vec4 game_color = textureLod(screen_texture, SCREEN_UV, 0.0);
	vec4 cone_color = textureLod(light_cone_texture, SCREEN_UV, 0.0);
	vec2 screen_pixel = (SCREEN_UV - 0.5) * screen_size;
	float alpha = circle_mask(screen_pixel, hero_position, cone_radius_pixel, cone_feather);
	 
	vec4 game_colors = (vec4(cone_color.xyz, alpha) * game_color);
	vec4 end_screen = vec4(vec3(1.0-alpha), 1.0);
	COLOR = mix(game_colors, end_screen, alpha_mix_factor) + rain(SCREEN_UV);
}

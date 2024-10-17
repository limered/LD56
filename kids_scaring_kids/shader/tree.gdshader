shader_type canvas_item;

void vertex() {
	VERTEX += cos(TIME * 0.7) * 1.5;
}

void fragment() {
}


shader_type canvas_item;

uniform vec2 scroll_speed = vec2(0.0, -0.03);

void fragment() {
    vec2 moving_uv = UV + (TIME * scroll_speed);
    COLOR = texture(TEXTURE, moving_uv)* .8;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}

shader_type sky;

uniform sampler2D starmap;

uniform vec3 sky_color: source_color;
uniform vec3 star_color: source_color;
uniform vec3 ambient_color: source_color;

uniform float star_factor: hint_range(0.01, 1.0, 0.01);
uniform float starmap_scale;
uniform float pulsate_intensity;

uniform float pan_x;
uniform float pan_y;

void sky() {
	// Called for every visible pixel in the sky background, as well as all pixels
	// in the radiance cubemap.
	if (AT_CUBEMAP_PASS) {
		if (ambient_color != vec3(0)) {
			COLOR = ambient_color;
		} else {
			COLOR = sky_color;
		}
	} else {
		vec2 panned_coords = vec2(SKY_COORDS.x + TIME * pan_x, SKY_COORDS.y + TIME * pan_y);
		vec4 map_value = texture(starmap, vec2(panned_coords.x / starmap_scale, panned_coords.y / starmap_scale));
		float average = (map_value.r + map_value.g + map_value.b) / 3.0;
		float true_star_factor = star_factor + (sin(TIME) - 0.5) * 2.0 * pulsate_intensity;
		if (average > true_star_factor) {
			COLOR = star_color;
		} else {
			COLOR = sky_color;
		}
	}
}

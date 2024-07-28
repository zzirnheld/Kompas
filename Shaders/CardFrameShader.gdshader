shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

instance uniform vec3 Albedo : source_color;
uniform float MetallicShine = 0.5;
uniform float Roughness = 0.1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	ALBEDO = Albedo.rgb;
	METALLIC = MetallicShine;
	ROUGHNESS = Roughness;
}

//Do not impl light() unless you use it

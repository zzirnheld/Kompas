shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

instance uniform vec4 Albedo : source_color;
instance uniform bool Greyscale;
uniform float MetallicShine = 0.5;
uniform float Roughness = 0.1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	ALBEDO = Greyscale ? vec3((Albedo.r + Albedo.g + Albedo.b) / 3.0) : Albedo.rgb;
	METALLIC = MetallicShine;
	ROUGHNESS = Roughness;
}

//Do not impl light() unless you use it

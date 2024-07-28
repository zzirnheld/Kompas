shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx;

instance uniform bool Greyscale;
uniform vec4 albedo : source_color;
uniform sampler2D Texture : source_color,filter_linear_mipmap,repeat_enable;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 albedo_tex = texture(Texture,UV);
	float avg = (albedo_tex.r + albedo_tex.g + albedo_tex.b) / 3.0;
	vec3 rgb = Greyscale ? vec3(avg) : albedo_tex.rgb;
	ALBEDO = albedo.rgb * rgb;
}

//Do not impl light() unless you use it

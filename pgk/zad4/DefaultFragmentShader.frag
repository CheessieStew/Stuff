#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec3 vertexPosition_worldspace;
in vec3 normal_cameraspace;
in vec3 lookingDirection_cameraspace;
// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D myTextureSampler;
uniform vec3 cameraPosition_worldSpace;
uniform mat4 onlyView;
uniform mat4 onlyModel;

void main(){
	

	// Output color = color of the texture at the specified UV
	vec4 texColor = texture( myTextureSampler, UV ).rgba;
	texColor.w = 0.5;
	color = texColor;

	float distanceToCam = length(vertexPosition_worldspace - cameraPosition_worldSpace);
	float mistWeight = exp(-distanceToCam/20);
	if (mistWeight > 1)
		mistWeight = 1;
	color = color * (mistWeight) + vec4(0.4, 0.6, 0.95, 1) * (1-mistWeight);
}
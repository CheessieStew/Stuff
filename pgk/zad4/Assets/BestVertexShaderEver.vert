#version 330 core

// Input vertex data, different for all executions of this shader.
layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec2 vertexUV;
layout(location = 2) in vec3 vertexNormal_modelspace;

// Output data ; will be interpolated for each fragment.
out vec2 UV;
out vec3 vertexPosition_worldspace;
out vec3 normal_cameraspace;
out vec3 lookingDirection_cameraspace;

// Values that stay constant for the whole mesh.
uniform mat4 MVP;
uniform mat4 onlyView;
uniform mat4 onlyModel;

void main(){

	gl_Position =  MVP * vec4(vertexPosition_modelspace,1);
	
	//out1 - UV
	UV = vertexUV;

	//out2 - position in worldspace
	vertexPosition_worldspace = (onlyModel * vec4(vertexPosition_modelspace, 1)).xyz;

	//out3 - looking direction in cameraspace
	vec3 vertexPosition_cameraspace = (onlyView * onlyModel * vec4(vertexPosition_modelspace,1)).xyz;
	lookingDirection_cameraspace = vec3(0,0,0) - vertexPosition_cameraspace;

	//out4 - fragment's normal in cameraspace
	// 	normal_cameraspace = (onlyView * onlyModel * vec4(vertexNormal_modelspace,0)).xyz;
	// THE TUTORIAL SAYS that it's only correct if model matrix does not scale the model
	// and suggests to use it's "inverse transpose". I tried using both
	// inverse(transpose(onlyModel)) and transpose(inverse(onlyModel))
	// but the effect was weird
	// I'm not even sure this way is correct:

	vec4 test1 = onlyModel * vec4(1,0,0,1);
	vec4 test2 = onlyModel * vec4(-1,0,0,1);
	float scaleX = length(test1.xyz-test2.xyz) * 0.5;

	test1 = onlyModel * vec4(0,1,0,1);
	test2 = onlyModel * vec4(0,-1,0,1);
	float scaleY = length(test1.xyz - test2.xyz) * 0.5;
	
	test1 = onlyModel * vec4(0,0,1,1);
	test2 = onlyModel * vec4(0,0,-1,1);
	float scaleZ = length(test1.xyz - test2.xyz) * 0.5;

	mat4 scale = mat4(
		vec4(scaleX, 0, 0, 0),
		vec4(0, scaleY, 0, 0),
		vec4(0, 0, scaleZ, 0),
		vec4(0, 0, 0, 1));

	normal_cameraspace = (onlyView * onlyModel * inverse(scale) * vec4(vertexNormal_modelspace,0)).xyz;

	 

}


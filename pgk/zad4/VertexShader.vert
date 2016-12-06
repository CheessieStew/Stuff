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
	normal_cameraspace = (onlyView * onlyModel * vec4(vertexNormal_modelspace,0)).xyz;

}


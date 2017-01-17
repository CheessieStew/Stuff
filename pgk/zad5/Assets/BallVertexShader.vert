#version 330 core

layout(location = 0) in vec2 vertexCoords;
layout(location = 1) in float vertexHeight;
uniform vec2 cellCoords;
uniform vec3 cameraPosition_worldSpace;
uniform mat4 MVP;
uniform bool info;
const float pi = 3.14159265359;
const float radius = 6371;
out vec2 coords;
out float distanceToCamera;
out float height;
flat out int gridDensity;

void main(){

	vec2 aux = vertexCoords + cellCoords;
	coords = aux;
	height = vertexHeight;
	vec3 res;
	res.y = sin(aux.y / 180 * pi); //cos(aux.y / 360 * 2 * pi);
	res.z = cos(aux.x / 180 * pi) * abs(cos(aux.y / 360 * 2 * pi));
	res.x = sin(aux.x / 180 * pi) * abs(cos(aux.y / 360 * 2 * pi));
	if (info)
		res *= (radius + vertexHeight/1000);
	else
		res *= radius;


	distanceToCamera = distance(cameraPosition_worldSpace,res);

	if (length(cameraPosition_worldSpace) > 16000)
		gridDensity = 8;
	else if (length(cameraPosition_worldSpace) > 12000)
		gridDensity = 4;
	else if (length(cameraPosition_worldSpace) > 8000)
		gridDensity = 2;
	else 
		gridDensity = 1;
	gl_Position = MVP * vec4(res,1);

}


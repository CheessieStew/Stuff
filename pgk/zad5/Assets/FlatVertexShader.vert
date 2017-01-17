#version 330 core

layout(location = 0) in vec2 vertexCoords;
layout(location = 1) in float vertexHeight;
uniform vec2 cellCoords;
uniform bool info;
uniform vec2 lowerBound;
uniform vec2 upperBound;
const float pi = 3.14159265359;

out vec2 coords;
out float distanceToCamera;
out float height;
flat out int gridDensity;

void main(){
	vec2 aux = vertexCoords + cellCoords;
	coords = aux;
	height = vertexHeight;


	float difx = upperBound.x - lowerBound.x;
	float dify = upperBound.y - lowerBound.y;
	float width = abs(cos(aux.y / 180 * pi));

	aux.x -= lowerBound.x - difx * 0.5;

	vec3 res;
	res.y = aux.y; //cos(aux.y / 360 * 2 * pi);
	res.x = aux.x;
	res.z = 0;


	res.y -= lowerBound.y;
	res.x /= difx;
	res.y /= dify;
	res.xy = res.xy * 2 - vec2(2,1);
	res.x *=  width;
	distanceToCamera = 100 * sqrt(dify * difx);


	gridDensity = 1;
	gl_Position = vec4(res,1);

}


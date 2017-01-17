#version 330 core

in float distanceToCamera;
in vec2 coords;
in float height;
flat in int gridDensity;
const float pi = 3.14159265359;

uniform bool info;

// Ouput data
out vec4 color;
void main(){
	float lineThickness;

	lineThickness = distanceToCamera/6000 * 0.3;
	//float a =(coords.x + 180)/360;
	//float b =(coords.y + 90)/180;
	float a = 0.5;
	float xx = coords.x - (floor(coords.x) - int(floor(coords.x)) % gridDensity);
	if (xx > 0.5 * gridDensity) xx = gridDensity-xx;
	float yy = coords.y - (floor(coords.y) - int(floor(coords.y)) % gridDensity);
	if (yy > 0.5 * gridDensity) yy = gridDensity-yy;

	xx *= cos(coords.y / 180 * pi);

	bool xg = int(floor(coords.x)) % 2 == 0;
	bool yg = int(floor(coords.y)) % 2 == 0;
	if (xx < lineThickness  || yy < lineThickness)
		a = 1 - 0.5/lineThickness * min(xx,yy);
	color = vec4(0.7 - a, a, 0.7 - a,1);
	if (info)
	{
		if      (height <= 0  )   color.rgb = vec3(0.,       0.,        1.); //blue
		else if (height < 500)   color.rgb = vec3(0.,       height/500,    0.); //->green
		else if (height < 1000)  color.rgb = vec3(height/500-1, 1.,        0.); //->yellow
		else if (height < 2000)  color.rgb = vec3(1.,       2.-height/1000,0.); //->red
		else                 color.rgb = vec3(1.,1.,1.);                //white
	}
	
	//float sx = (180 + coords.x)/360;
	//float sy = (90 + coords.y)/360;
	//color.rgb = vec3(sx,sy,0);
	//color.rgb = colorize - color.rgb;
}
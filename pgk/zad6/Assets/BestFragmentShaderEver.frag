#version 330 core

// Interpolated values from the vertex shaders
in vec2 UV;
in vec3 vertexPosition_worldspace;
in vec3 normal_cameraspace;
in vec3 lookingDirection_cameraspace;

// Ouput data
out vec4 color;

// Values that stay constant for the whole mesh.
uniform sampler2D albedoTexture;
uniform bool noiseInstead;
uniform vec3 cameraPosition_worldSpace;
uniform mat4 onlyView;
uniform mat4 onlyModel;
uniform vec3 mainLightColor = vec3(1,1,1);
uniform float mainLightIntensity = 1.6f;
uniform vec3 emissiveness = vec3(0.1,0.1,0.1);
uniform float opacity = 0.1;
uniform vec3 specularity = vec3(0.7, 0.7, 0.7);
uniform vec3 tint = vec3(1, 1, 1);
uniform vec3 pointLightPositions[21];
uniform vec3 pointLightColors[21];
uniform float pointLightIntensities[21];
uniform int pointLightsAmmount = 0;
uniform vec3 mistColor = vec3(0.01, 0.02, 0.1);
uniform float mistThickness = 0.66;
uniform vec3 noiseVectors[12];
uniform int permutation[256];
uniform float time;

vec4 PointIllumination(vec4 albedoColor, int i)
{
	vec3 lightPosition_cameraspace = (onlyView * vec4(pointLightPositions[i],1)).xyz;
	vec3 lightDirection_cameraspace = lightPosition_cameraspace + lookingDirection_cameraspace;

	vec3 normal = normalize(normal_cameraspace);
	vec3 lightDir = normalize(lightDirection_cameraspace);
	float diffuse = clamp(dot(normal,lightDir), 0, 1) * 0.6;

	vec3 looking = normalize(lookingDirection_cameraspace);
	vec3 reflection = reflect(-lightDir,normal);
	float specular = clamp(dot(looking,reflection), 0, 1);
	
	float distance = length(pointLightPositions[i] - vertexPosition_worldspace);

	vec4 diffuseColor = vec4(diffuse / (distance * distance) * pointLightIntensities[i] * pointLightColors[i] * albedoColor.rgb * tint, 1);
	diffuseColor.a = opacity * albedoColor.a;

	vec4 specularColor = vec4(pow(specular,5) / (distance * distance) * pointLightIntensities[i] * pointLightColors[i] * specularity, 1);
	specularColor.a = length(specularColor.rgb)/1.732;

	vec4 res;
	res.rgb = diffuseColor.rgb +
		      specularColor.rgb;
	res.a = max(diffuseColor.a, specularColor.a);
	return res;
}

vec4 GlobalIllumination(vec4 albedoColor)
{
	//main light is always directly above the pixel in world space and does not depend on distance
	vec3 lightPosition_cameraspace = (onlyView * vec4(vertexPosition_worldspace + vec3(0,1,0),1)).xyz;
	vec3 lightDirection_cameraspace = lightPosition_cameraspace + lookingDirection_cameraspace;

	vec3 normal = normalize(normal_cameraspace);
	vec3 lightDir = normalize(lightDirection_cameraspace);
	float diffuse = clamp(dot(normal,lightDir), 0, 1);

	vec3 looking = normalize(lookingDirection_cameraspace);
	vec3 reflection = reflect(-lightDir,normal);
	float specular = clamp(dot(looking,reflection), 0, 1);

	vec4 diffuseColor = vec4(diffuse * mainLightIntensity * mainLightColor * albedoColor.rgb * tint, 1);
	diffuseColor.a = opacity * albedoColor.a;

	vec4 specularColor = vec4(pow(specular,5) * mainLightIntensity * mainLightColor * specularity, 1);
	specularColor.a = length(specularColor.rgb)/1.732;

	vec4 res;
	res.rgb = diffuseColor.rgb +
		      specularColor.rgb;
	res.a = max(diffuseColor.a, specularColor.a);
	return res;
}

vec3 RandomVector(int i, int j, int k)
{
	return noiseVectors
		[permutation
			[(i + permutation
				[(j + permutation
					[
						k % 256
					]) % 256
				]) % 256
			] % 12
		];
}

float Smooth(float t)
{
	float tt = abs(t);
	if (tt > 1)
		return 0;
	return 1 + tt * tt * (-3 + 2 * tt);
}

float Perlin(float freq)
{
	float x = (vertexPosition_worldspace.x) * freq + time * 1.8;
	float y = (vertexPosition_worldspace.y) * freq + time * 0.6;
	float z = (vertexPosition_worldspace.z) * freq + time * 1.2;
	int x0 = int(floor(x));
	int y0 = int(floor(y));
	int z0 = int(floor(z));
	float s = 0;
	for (int i = x0; i <= x0+1; i++)
	{
		for (int j = y0; j <= y0+1; j++)
		{
			for (int k = z0; k <= z0+1; k++)
			{
				float u = x - i;
				float v = y - j;
				float w = z - k;
				float dotp = dot(vec3(u,v,w), RandomVector(i,j,k));
				s += Smooth(u) * Smooth(v) * Smooth(w) * dotp;
			}
		}
	}
	return s * 0.5 + 0.5;
}

void main(){


	vec4 albedoColor;
	if (noiseInstead)
	{
		albedoColor = vec4(8,8,8,1);
		albedoColor = vec4(8,8,8,1) * Perlin(.3);
	}
	else 
		albedoColor = texture(albedoTexture, UV );

	vec4 ambientColor = vec4(0.1 * albedoColor.rgb * tint, opacity * albedoColor.a);
	vec4 emissiveColor = vec4(3 * emissiveness * albedoColor.rgb, length(emissiveness));

	vec4 globalIlluminationColor = GlobalIllumination(albedoColor);

	color.rgb = ambientColor.rgb + 
			    globalIlluminationColor.rgb +
				emissiveColor.rgb;
	color.a = max(emissiveColor.a, max(ambientColor.a, globalIlluminationColor.a));
	for (int i = 0; i < pointLightsAmmount; i++)
	{
		float distance = length(pointLightPositions[i] - vertexPosition_worldspace);
		if (distance < pointLightIntensities[i] + 0.6)
		{
			vec4 pointIlluminationColor = PointIllumination(albedoColor, i);
			color.rgb += pointIlluminationColor.rgb;
			color.a = max(color.a, pointIlluminationColor.a);
		}
	}




	float distanceToCam = length(vertexPosition_worldspace - cameraPosition_worldSpace);
	float mistWeight = exp(-distanceToCam * mistThickness);
	if (mistWeight > 1)
		mistWeight = 1;
	color.rgb = color.rgb * (mistWeight) + mistColor * (1-mistWeight);
}
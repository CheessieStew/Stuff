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

	float s = 0;

	int x0 = int(floor(x));
	int y0 = int(floor(y));
	int z0 = int(floor(z));
	int x1 = x0+1;
	int y1 = y0+1;
	int z1 = z0+1;

	float u0 = x - x0;
	float smoothU0 = Smooth(u0);
	float v0 = y - y0;
	float smoothV0 = Smooth(v0);
	float w0 = z - z0;
	float smoothW0 = Smooth(w0);
	float u1 = x - x1;
	float smoothU1 = Smooth(u1);
	float v1 = y - y1;
	float smoothV1 = Smooth(v1);
	float w1 = z - z1;
	float smoothW1 = Smooth(w1);


	float partialV0W0 = smoothU0 * dot(vec3(u0,v0,w0), RandomVector(x0,y0,z0));
	partialV0W0 += smoothU1 * dot(vec3(u1,v0,w0), RandomVector(x1,y0,z0));

	float partialV1W0 = smoothU0 * dot(vec3(u0,v1,w0), RandomVector(x0,y1,z0));
	partialV1W0 += smoothU1 * dot(vec3(u1,v1,w0), RandomVector(x1,y1,z0));

	float partialV0W1 = smoothU0 * dot(vec3(u0,v0,w1), RandomVector(x0,y0,z1));
	partialV0W1 += smoothU1 * dot(vec3(u1,v0,w1), RandomVector(x1,y0,z1));

	float partialV1W1 = smoothU0 * dot(vec3(u0,v1,w1), RandomVector(x0,y1,z1));
	partialV1W1 += smoothU1 * dot(vec3(u1,v1,w1), RandomVector(x1,y1,z1));

	float partialW0 = smoothV0 * partialV0W0 + smoothV1 * partialV1W0;
	float partialW1 = smoothV0 * partialV0W1 + smoothV1 * partialV1W1;

	return (partialW0 * smoothW0 + partialW1 * smoothW1) * 0.5 + 0.5;
}

void main(){


	vec4 albedoColor;
	if (noiseInstead)
	{
		albedoColor = vec4(mistColor * 3, 0.5);
		albedoColor *= (0.25 +  0.5 * Perlin(.1) + 0.25 * Perlin(.2));
	}
	else 
		albedoColor = texture(albedoTexture, UV );

	vec4 ambientColor = vec4(0.1 * albedoColor.rgb * tint, opacity * albedoColor.a);
	vec4 emissiveColor = vec4(3 * emissiveness * albedoColor.rgb, length(emissiveness));

	vec4 globalIlluminationColor;
	if (noiseInstead)
		globalIlluminationColor = albedoColor;
	else globalIlluminationColor = GlobalIllumination(albedoColor);

	color.rgb = ambientColor.rgb + 
			    globalIlluminationColor.rgb +
				emissiveColor.rgb;
	color.a = max(emissiveColor.a, max(ambientColor.a, globalIlluminationColor.a));
	if (!noiseInstead)
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
	if (mistWeight > 1 || noiseInstead)
		mistWeight = 1;
	color.rgb = color.rgb * (mistWeight) + mistColor * (1-mistWeight);
}
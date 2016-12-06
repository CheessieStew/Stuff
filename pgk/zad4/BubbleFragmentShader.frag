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
uniform vec3 mainLightColor = vec3(1,1,1);
uniform float mainLightPower = 1.2f;
uniform vec3 emissiveness = vec3(0.1,0.1,0.1);
uniform float opacity = 0.1;
uniform vec3 specularity = vec3(0.7, 0.7, 0.7);
uniform vec3 tint = vec3(1, 1, 1);

void main(){
	//main light is always directly above the pixel in world space and does not depend on distance
	vec3 lightPosition_cameraspace = (onlyView * vec4(vertexPosition_worldspace + vec3(0,1,0),1)).xyz;
	vec3 lightDirection_cameraspace = lightPosition_cameraspace + lookingDirection_cameraspace;

	vec4 albedoColor = texture(myTextureSampler, UV );
	albedoColor.rgb = albedoColor.rgb;
	//vec3 albedoColor3 = albedoColor.rgb;
	//vec3 ambientColor = vec3(emissiveness,emissiveness,emissiveness) * albedoColor3;
	//vec3 specularColor = specularity;

	vec3 normal = normalize(normal_cameraspace);
	vec3 lightDir = normalize(lightDirection_cameraspace);
	float diffuse = clamp(dot(normal,lightDir), 0, 1);

	vec3 looking = normalize(lookingDirection_cameraspace);
	vec3 reflection = reflect(-lightDir,normal);
	float specular = clamp(dot(looking,reflection), 0, 1);

	vec4 ambientColor = vec4(0.1 * albedoColor.rgb * tint, opacity * albedoColor.a);
	vec4 emissiveColor = vec4(emissiveness * albedoColor.rgb, length(emissiveness));
	vec4 diffuseColor = vec4(diffuse * mainLightPower * mainLightColor * albedoColor.rgb * tint, 1);
	diffuseColor.a = opacity * albedoColor.a * length(diffuseColor.rgb)/1.732;
	vec4 specularColor = vec4(pow(specular,5) * mainLightPower * mainLightColor * specularity, 1);
	specularColor.a = length(specularColor.rgb)/1.732;
	color.rgb = ambientColor.rgb * ambientColor.a +
		    emissiveColor.rgb * emissiveColor.a +
			diffuseColor.rgb * diffuseColor.a +
		    specularColor.rgb * specularColor.a;
	color.a = max(ambientColor.a, max(emissiveColor.a, max(diffuseColor.a, specularColor.a)));

	float distanceToCam = length(vertexPosition_worldspace - cameraPosition_worldSpace);
	float mistWeight = exp(-distanceToCam/15);
	if (mistWeight > 1)
		mistWeight = 1;
	color.rgb = color.rgb * (mistWeight) + vec3(0.1, 0.2, 0.3) * (1-mistWeight);
}
#include "graphics.hpp"
using namespace glm;

int Model3d::ModelsAmm = 0;
int Model3d::CurrentlyLoadedModel = -1;

Light::Light(GameObject &emitter, vec3 offset, float power)
{
	color = normalize(emitter.material.emissive);
	intensity = power;
	vec4 rotatedOffset = emitter.rotation * vec4(offset.x, offset.y, offset.z, 1);
	position = emitter.position + vec3(rotatedOffset.x, rotatedOffset.y, rotatedOffset.z);
}

Light::Light()
{
	color = vec3(1, 1, 1);
	intensity = 100;
}

Model3d::Model3d(const char* path)
{
	modelID = ModelsAmm;
	ModelsAmm++;
	bool res = loadOBJ(path, vertices, uvs, normals);
	if (!res)
		throw "loadOBJ did not succeed";
	glGenBuffers(1, &vertexBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
	glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(vec3), &vertices[0], GL_STATIC_DRAW);

	glGenBuffers(1, &uvBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, uvBuffer);
	glBufferData(GL_ARRAY_BUFFER, uvs.size() * sizeof(vec2), &uvs[0], GL_STATIC_DRAW);

	glGenBuffers(1, &normalBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, normalBuffer);
	glBufferData(GL_ARRAY_BUFFER, normals.size() * sizeof(vec3), &normals[0], GL_STATIC_DRAW);
}

void Model3d::CleanUp()
{
	glDeleteBuffers(1, &vertexBuffer);
	glDeleteBuffers(1, &uvBuffer);
	glDeleteBuffers(1, &normalBuffer);
}

int Model3d::Size()
{
	return vertices.size();
}



GameObject3d::GameObject3d(GameObject & obj, GLuint tex, Model3d & m3d) :
	Texture(tex),
	M3d(m3d),
	Object(obj)
{
}

void GameObject3d::Draw(GLuint shader, const mat4 * view, const mat4 * projection, vec3 camPos)
{
	static GLuint MatrixID = glGetUniformLocation(shader, "MVP"); //mat4
	static GLuint OnlyModelID = glGetUniformLocation(shader, "onlyModel"); //mat4
	static GLuint OnlyViewID = glGetUniformLocation(shader, "onlyView"); //mat4
	static GLuint InvTranModelID = glGetUniformLocation(shader, "inverseTransposeModel");
	static GLuint camPosID = glGetUniformLocation(shader, "cameraPosition_worldSpace"); //vec3
	static GLuint TextureID = glGetUniformLocation(shader, "albedoTexture"); //sampler2d
	static GLuint NoiseID = glGetUniformLocation(shader, "noiseInstead"); //int
	static GLuint MLightColorID = glGetUniformLocation(shader, "mainLightColor"); //vec3
	static GLuint MLightPowerID = glGetUniformLocation(shader, "mainLightPower"); //float
	static GLuint EmissiveID = glGetUniformLocation(shader, "emissiveness"); //vec3
	static GLuint OpacityID = glGetUniformLocation(shader, "opacity"); //float
	static GLuint SpecularID = glGetUniformLocation(shader, "specularity"); //vec3
	static GLuint TintID = glGetUniformLocation(shader, "tint"); //vec3

	vec3 position = Object.position;
	position.x = position.x;
	mat4 translation = translate(mat4(1.f),position);
	mat4 scalem = scale(mat4(1.f), Object.scale);

	mat4 MVP = (*projection) * (*view) * translation * Object.rotation * scalem;
	mat4 OnlyModel = translation * Object.rotation * scalem;
	mat3 InvTranModel = mat3(transpose(inverse(OnlyModel)));
	
	glUniformMatrix4fv(MatrixID, 1, GL_FALSE, &(MVP[0][0]));
	glUniformMatrix4fv(OnlyModelID, 1, GL_FALSE, &OnlyModel[0][0]);
	glUniformMatrix4fv(OnlyViewID, 1, GL_FALSE, &((*view)[0][0]));
	glUniformMatrix3fv(InvTranModelID, 1, GL_FALSE, &InvTranModel[0][0]);


	glUniform3f(camPosID, camPos.x, camPos.y, camPos.z);
	glUniform3f(EmissiveID, Object.material.emissive.x, Object.material.emissive.y, Object.material.emissive.z);
	glUniform1f(OpacityID, Object.material.opacity);
	glUniform3f(TintID, Object.material.tint.x, Object.material.tint.y, Object.material.tint.y);
	glUniform3f(SpecularID, Object.material.specular.x, Object.material.specular.y, Object.material.specular.z);
	if (Texture != -1)
	{
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, Texture);
		glUniform1i(TextureID, 0);
		glUniform1i(NoiseID, 0);
	}
	else glUniform1i(NoiseID, 1);
	
	glEnableVertexAttribArray(0);
	glEnableVertexAttribArray(1);
	glEnableVertexAttribArray(2);

	if (M3d.CurrentlyLoadedModel != M3d.modelID)
	{
		M3d.CurrentlyLoadedModel = M3d.modelID;
		glBindBuffer(GL_ARRAY_BUFFER, M3d.vertexBuffer);
		glVertexAttribPointer(
			0,
			3,
			GL_FLOAT,
			GL_FALSE,
			0,
			(void*)0
			);

		// 2nd attribute buffer : UVs
		glBindBuffer(GL_ARRAY_BUFFER, M3d.uvBuffer);
		glVertexAttribPointer(
			1,
			2,
			GL_FLOAT,
			GL_FALSE,
			0,
			(void*)0
			);

		glBindBuffer(GL_ARRAY_BUFFER, M3d.normalBuffer);
		glVertexAttribPointer(
			2,
			3,
			GL_FLOAT,
			GL_FALSE,
			0,
			(void*)0
			);
	}
	// Draw the triangle !
	glDrawArrays(GL_TRIANGLES, 0, M3d.Size());
	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
	glDisableVertexAttribArray(2);

}

void EnvironmentSetup(GLuint shader, float time, float mainLightIntensity, Light* lights, int lightsAmm, vec3 mistColor, float mistThickness)
{
	GLuint LightPositionsID = glGetUniformLocation(shader, "pointLightPositions"); //vec3[21]
	GLuint LightColorsID = glGetUniformLocation(shader, "pointLightColors"); //vec3[21]
	GLuint LightIntensitiesID = glGetUniformLocation(shader, "pointLightIntensities"); //float[21]
	GLuint LightsAmmountID = glGetUniformLocation(shader, "pointLightsAmmount"); //int
	GLuint MistColorID = glGetUniformLocation(shader, "mistColor"); //vec3
	GLuint MistThicknessID = glGetUniformLocation(shader, "mistThickness"); //float
	GLuint MainLightIntensityID = glGetUniformLocation(shader, "mainLightIntensity"); //float
	GLuint TimeID = glGetUniformLocation(shader, "time"); //float
	static GLuint noiseVectorsID = glGetUniformLocation(shader, "noiseVectors");
	static GLuint permutationID = glGetUniformLocation(shader, "permutation");

	glUniform1f(TimeID, time);

	static const GLfloat vectors[36] =
	{
		1,1,0,-1,1,0,1,-1,0,-1,-1,0,
		1,0,1,-1,0,1,1,0,-1,-1,0,-1,
		0,1,1,0,-1,1,0,1,-1,0,-1,-1
	};
	static const GLint permutation[256] =
	{ 151,160,137,91,90,15,
		131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
		190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
		88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
		77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
		102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
		135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
		5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
		223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
		129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
		251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
		49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
		138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};
	glUniform3fv(noiseVectorsID, 36, &vectors[0]);
	glUniform1iv(permutationID, 256, &permutation[0]);
	static float auxarr[21 * 3];
	for (int i = 0; i < lightsAmm; i++)
	{
		auxarr[3 * i] = lights[i].position.x;
		auxarr[3 * i + 1] = lights[i].position.y;
		auxarr[3 * i + 2] = lights[i].position.z;
	}
	glUniform3fv(LightPositionsID, lightsAmm, auxarr);

	for (int i = 0; i < lightsAmm; i++)
	{
		auxarr[3 * i] = lights[i].color.r;
		auxarr[3 * i + 1] = lights[i].color.g;
		auxarr[3 * i + 2] = lights[i].color.b;
	}
	glUniform3fv(LightColorsID, lightsAmm, auxarr);

	for (int i = 0; i < lightsAmm; i++)
	{
		auxarr[i] = lights[i].intensity;
	}
	glUniform1fv(LightIntensitiesID, lightsAmm, auxarr);

	glUniform1i(LightsAmmountID, lightsAmm);

	glUniform3f(MistColorID, mistColor.r, mistColor.g, mistColor.b);

	glUniform1f(MistThicknessID, mistThickness);

	glUniform1f(MainLightIntensityID, mainLightIntensity);

	glClearColor(mistColor.r, mistColor.g, mistColor.b, 1.0f);
}
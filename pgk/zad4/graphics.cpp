#include "graphics.hpp"

int Model3d::ModelsAmm = 0;
int Model3d::CurrentlyLoadedModel = -1;

Light::Light(GameObject &emitter, glm::vec3 offset, float power)
{
	color = glm::normalize(emitter.material.emissive);
	intensity = power;
	glm::vec4 rotatedOffset = emitter.rotation * glm::vec4(offset.x, offset.y, offset.z, 1);
	position = emitter.position + glm::vec3(rotatedOffset.x, rotatedOffset.y, rotatedOffset.z);
}

Light::Light()
{
	color = glm::vec3(1, 1, 1);
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
	glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(glm::vec3), &vertices[0], GL_STATIC_DRAW);

	glGenBuffers(1, &uvBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, uvBuffer);
	glBufferData(GL_ARRAY_BUFFER, uvs.size() * sizeof(glm::vec2), &uvs[0], GL_STATIC_DRAW);

	glGenBuffers(1, &normalBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, normalBuffer);
	glBufferData(GL_ARRAY_BUFFER, normals.size() * sizeof(glm::vec3), &normals[0], GL_STATIC_DRAW);
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

void GameObject3d::Draw(GLuint shader, const glm::mat4 * view, const glm::mat4 * projection, glm::vec3 camPos)
{
	static GLuint MatrixID = glGetUniformLocation(shader, "MVP"); //mat4
	static GLuint OnlyModelID = glGetUniformLocation(shader, "onlyModel"); //mat4
	static GLuint OnlyViewID = glGetUniformLocation(shader, "onlyView"); //mat4
	static GLuint InvTranModelID = glGetUniformLocation(shader, "inverseTransposeModel");
	static 	GLuint camPosID = glGetUniformLocation(shader, "cameraPosition_worldSpace"); //vec3
	static GLuint TextureID = glGetUniformLocation(shader, "myTextureSampler"); //sampler2d
	static GLuint MLightColorID = glGetUniformLocation(shader, "mainLightColor"); //vec3
	static GLuint MLightPowerID = glGetUniformLocation(shader, "mainLightPower"); //float
	static GLuint EmissiveID = glGetUniformLocation(shader, "emissiveness"); //vec3
	static GLuint OpacityID = glGetUniformLocation(shader, "opacity"); //float
	static GLuint SpecularID = glGetUniformLocation(shader, "specularity"); //vec3
	static GLuint TintID = glGetUniformLocation(shader, "tint"); //vec3

	glm::vec3 position = Object.position;
	position.x = position.x;
	glm::mat4 translation = glm::translate(glm::mat4(1.f),position);
	glm::mat4 scale = glm::scale(glm::mat4(1.f), Object.scale);

	glm::mat4 MVP = (*projection) * (*view) * translation * Object.rotation * scale;
	glm::mat4 OnlyModel = translation * Object.rotation * scale;
	glm::mat3 InvTranModel = glm::mat3(glm::transpose(glm::inverse(OnlyModel)));
	
	glUniformMatrix4fv(MatrixID, 1, GL_FALSE, &(MVP[0][0]));
	glUniformMatrix4fv(OnlyModelID, 1, GL_FALSE, &OnlyModel[0][0]);
	glUniformMatrix4fv(OnlyViewID, 1, GL_FALSE, &((*view)[0][0]));
	glUniformMatrix3fv(InvTranModelID, 1, GL_FALSE, &InvTranModel[0][0]);


	glUniform3f(camPosID, camPos.x, camPos.y, camPos.z);
	glUniform3f(EmissiveID, Object.material.emissive.x, Object.material.emissive.y, Object.material.emissive.z);
	glUniform1f(OpacityID, Object.material.opacity);
	glUniform3f(TintID, Object.material.tint.x, Object.material.tint.y, Object.material.tint.y);
	glUniform3f(SpecularID, Object.material.specular.x, Object.material.specular.y, Object.material.specular.z);
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, Texture);
	glUniform1i(TextureID, 0);

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

void EnvironmentSetup(GLuint shader, float mainLightIntensity, Light* lights, int lightsAmm, glm::vec3 mistColor, float mistThickness)
{
	GLuint LightPositionsID = glGetUniformLocation(shader, "pointLightPositions"); //vec3[21]
	GLuint LightColorsID = glGetUniformLocation(shader, "pointLightColors"); //vec3[21]
	GLuint LightIntensitiesID = glGetUniformLocation(shader, "pointLightIntensities"); //float[21]
	GLuint LightsAmmountID = glGetUniformLocation(shader, "pointLightsAmmount"); //int
	GLuint MistColorID = glGetUniformLocation(shader, "mistColor"); //vec3
	GLuint MistThicknessID = glGetUniformLocation(shader, "mistThickness"); //float
	GLuint MainLightIntensityID = glGetUniformLocation(shader, "mainLightIntensity"); //float

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
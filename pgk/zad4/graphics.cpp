#include "graphics.hpp"

Model3d::Model3d(const char* path)
{	
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

int Model3d::Size()
{
	return vertices.size();
}





GameObject3d::GameObject3d(GameObject & obj, GLuint s, GLuint tex, Model3d & m3d) :
	Texture(tex),
	Shader(s),
	M3d(m3d),
	Object(obj)
{
}

void GameObject3d::Draw(const glm::mat4 * view, const glm::mat4 * projection, glm::vec3 camPos)
{
	glUseProgram(Shader);

	GLuint MatrixID = glGetUniformLocation(Shader, "MVP"); //mat4
	GLuint OnlyModelID = glGetUniformLocation(Shader, "onlyModel"); //mat4
	GLuint OnlyViewID = glGetUniformLocation(Shader, "onlyView"); //mat4
	GLuint camPosID = glGetUniformLocation(Shader, "cameraPosition_worldSpace"); //vec3
	GLuint TextureID = glGetUniformLocation(Shader, "myTextureSampler"); //sampler2d
	GLuint MLightColorID = glGetUniformLocation(Shader, "mainLightColor"); //vec3
	GLuint MLightPowerID = glGetUniformLocation(Shader, "mainLightPower"); //float
	GLuint EmissiveID = glGetUniformLocation(Shader, "emissiveness"); //vec3
	GLuint OpacityID = glGetUniformLocation(Shader, "opacity"); //float
	GLuint SpecularID = glGetUniformLocation(Shader, "specularity"); //vec3
	GLuint TintID = glGetUniformLocation(Shader, "tint"); //vec3
	glm::vec3 position = Object.position;
	position.x = position.x;
	glm::mat4 translation = glm::translate(glm::mat4(1.f),position);
	glm::mat4 scale = glm::scale(glm::mat4(1.f), Object.scale);

	glUniform3f(camPosID, camPos.x, camPos.y, camPos.z);
	glUniform3f(EmissiveID, Object.material.emissive.x, Object.material.emissive.y, Object.material.emissive.z);
	glUniform1f(OpacityID, Object.material.opacity);
	glUniform3f(TintID, Object.material.tint.x, Object.material.tint.y, Object.material.tint.y);
	glUniform3f(SpecularID, Object.material.specular.x, Object.material.specular.y, Object.material.specular.z);
	glm::mat4 MVP = (*projection) * (*view) * translation * Object.rotation * scale;
	glm::mat4 OnlyModel = translation * Object.rotation * scale;
	glUniformMatrix4fv(MatrixID, 1, GL_FALSE, &(MVP[0][0]));
	glUniformMatrix4fv(OnlyModelID, 1, GL_FALSE, &OnlyModel[0][0]);
	glUniformMatrix4fv(OnlyViewID, 1, GL_FALSE, &((*view)[0][0]));

	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, Texture);
	glUniform1i(TextureID, 0);

	glEnableVertexAttribArray(0);
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
	glEnableVertexAttribArray(1);
	glBindBuffer(GL_ARRAY_BUFFER, M3d.uvBuffer);
	glVertexAttribPointer(
		1,       
		2,       
		GL_FLOAT,
		GL_FALSE,
		0,       
		(void*)0 
		);

	glEnableVertexAttribArray(2);
	glBindBuffer(GL_ARRAY_BUFFER, M3d.normalBuffer);
	glVertexAttribPointer(
		2,       
		3,       
		GL_FLOAT,
		GL_FALSE,
		0,       
		(void*)0 
		);

	// Draw the triangle !
	glDrawArrays(GL_TRIANGLES, 0, M3d.Size());
	glDisableVertexAttribArray(0);
	glDisableVertexAttribArray(1);
}

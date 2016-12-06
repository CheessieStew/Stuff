#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include <stdlib.h>
#include <vector>
#include <common/objloader.hpp>
#include <common/texture.hpp>
#include <common/shader.hpp>
#include "model.hpp"

class Model3d
{
	std::vector<glm::vec3> vertices;
	std::vector<glm::vec2> uvs;
	std::vector<glm::vec3> normals;
public:
	GLuint vertexBuffer;
	GLuint uvBuffer;
	GLuint normalBuffer;
	Model3d(const char* path);
	int Size();
};

class GameObject3d
{
public:
	Model3d& M3d;
	GLuint Texture;
	GameObject& Object;
	GLuint Shader;
	GameObject3d(GameObject& obj, GLuint s, GLuint tex, Model3d& m3d);
	void Draw(const glm::mat4 * view, const glm::mat4 * projection, glm::vec3 camPos);
};

#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include <stdlib.h>
#include <vector>
#include <common/objloader.hpp>
#include <common/texture.hpp>
#include "model.hpp"


class Light
{
public:
	glm::vec3 position;
	glm::vec3 color;
	float intensity;
	Light(GameObject &emitter, glm::vec3 offset, float power);
	Light();
};


class Model3d
{
	std::vector<glm::vec3> vertices;
	std::vector<glm::vec2> uvs;
	std::vector<glm::vec3> normals;
public:
	static int ModelsAmm;
	int modelID;
	static int CurrentlyLoadedModel;
	GLuint vertexBuffer;
	GLuint uvBuffer;
	GLuint normalBuffer;
	Model3d(const char* path);
	void CleanUp();
	int Size();
};

class GameObject3d
{
public:
	Model3d& M3d;
	GLuint Texture;
	GameObject& Object;
	GameObject3d(GameObject& obj, GLuint tex, Model3d& m3d);
	void Draw(GLuint shader, const glm::mat4 * view, const glm::mat4 * projection, glm::vec3 camPos);
};

void EnvironmentSetup(GLuint shader, float time, float mainLightIntensity, Light* lights, int lightsAmm, glm::vec3 mistColor, float mistThickness);

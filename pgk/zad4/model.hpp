#pragma once
#include <glm/glm.hpp>
#include <list>
using namespace std;


class Material
{
public:
	glm::vec3 tint;
	glm::vec3 emissive;
	glm::vec3 specular;
	float opacity;
};

class GameObject
{
public:
	glm::vec3 position;
	glm::mat4 rotation;
	glm::vec3 velocity;
	glm::vec3 scale;
	Material material;
	GameObject(glm::vec3 pos);
};

class Bubble : public GameObject
{
public:
	float growRate;
	float maxSize;
	Bubble(glm::vec3 pos, float startScale);
	bool light;
};

class Aquarium
{
public:
	GameObject playerBody;
	GameObject playerHorns;
	GameObject playerEyes;
	GameObject playerBulb;
	GameObject box;
	std::list<Bubble> bubbles;
	float xSize;
	float ySize;
	float zSize;
	float timeBeforeNextBubble;
	Aquarium(float xSize, float ySize, float zSize);
	void Aquarium::Update(double deltaTime, glm::vec3 thrust);
	float playerMaxVelocity;
	float playerAcceleration;
	float playerDecceleration;
	float playerRotateSpeed;
	int maxBubbleLights;
private:
	bool ShouldKill(Bubble & b);
	void SpawnBubble();
	void UpdatePlayer(double deltaTime, glm::vec3 thrust);
	bool PlayerTouchesBox(glm:: vec3 newPlayerPosition);
};
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
	bool firstPersonCamera;
	GameObject playerBody;
	GameObject playerHorns;
	GameObject playerEyes;
	GameObject playerBulb;
	GameObject box;
	std::list<Bubble> bubbles;
	int maxBubbleLights;
	int wounds;
	int points;
	int level;
	float playerYRot;
	float playerXRot;
	float playerTargetXRot;
	float playerTargetYRot;
	float xSize;
	float ySize;
	float zSize;

	Aquarium(float xSize, float ySize, float zSize);
	void Aquarium::Update(double deltaTime, glm::vec3 thrust);
private:
	float finish;
	float timeBetweenBubbles;
	float timeBeforeNextBubble;
	float baseBubbleVelocity;
	int currentBubbleLights;
	float playerMaxVelocity;
	float playerAcceleration;
	float playerDrag;
	float playerRotateSpeed;


	bool ShouldKill(Bubble & b);
	bool PlayerCollission(Bubble & b);
	void SpawnBubble();
	void UpdatePlayer(double deltaTime, glm::vec3 thrust);
	bool PlayerTouchesBox(glm:: vec3 newPlayerPosition);
	void TryNextLevel();
	void Restart();
};
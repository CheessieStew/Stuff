#include <stdlib.h>
#include <algorithm>
#define _USE_MATH_DEFINES
#include<math.h>
#include <glm/gtc/matrix_transform.hpp>
#include "model.hpp"
#define PLAYERRADIUS 1.0f
#define MAXPOINTLIGHTS 13
using namespace std;

GameObject::GameObject(glm::vec3 pos = glm::vec3(0,0,0))
{
	position = pos;
	rotation = glm::mat4(1.0);
	velocity = glm::vec3(0, 0, 0);
	scale = glm::vec3(1, 1, 1);
	material.tint = glm::vec3(1, 1, 1);
	material.opacity = 1;
	material.specular = glm::vec3(0.5, 0.5, 0.5);
}

// player is a sphere with radius 1
// a bubble is a sphere with radius equal to it's scale

Aquarium::Aquarium(float x, float y, float z):
	xSize(x),
	ySize(y),
	zSize(z)
{

	box = GameObject(glm::vec3(x / 2, y / 2, z / 2));
	box.scale = glm::vec3(x / 2, y / 2, z / 2);
	box.material.opacity = 0.3;
	box.material.emissive = glm::vec3(0.2, 0.2, 0.2);
	box.material.specular = glm::vec3(0.8, 0.8, 0.8);

	playerMaxVelocity = 8;
	playerAcceleration = 8;
	playerDrag = .8;
	playerRotateSpeed = 3.4;

	playerBody = GameObject();
	playerEyes = GameObject();
	playerHorns = GameObject();
	playerBulb = GameObject();

	Restart();
	playerBody.material.tint = glm::vec3(0.45, 0.525, 0.47);
	playerBody.material.specular = glm::vec3(0.1, 0.1, 0.1);
	playerHorns.material.tint = glm::vec3(1, 1, 0.8);
	playerHorns.material.specular = glm::vec3(0.7, 0.7, 0.7);
	playerEyes.material.tint = glm::vec3(0, 0, 0);
	playerEyes.material.specular = glm::vec3(0.8, 0.8, 0.8);
	playerBulb.material.emissive = glm::vec3(0.6, 0.5, 0.9) * 0.6f;
	playerBulb.material.opacity = 0.6;
}

void Aquarium::Restart()
{
	printf("\n\nLevel 1. Swim to the other side!\n");
	bubbles.clear();

	playerBody.position = (glm::vec3(xSize / 2, ySize / 2, 1.5));


	playerHorns.position = (glm::vec3(xSize / 2, ySize / 2, 1.5));


	playerEyes.position = (glm::vec3(xSize / 2, ySize / 2, 1.5));


	playerBulb.position = (glm::vec3(xSize / 2, ySize / 2, 1.5));

	playerXRot = 0;
	playerYRot = 0;
	playerTargetXRot = 0;
	playerTargetYRot = 0;
	maxBubbleLights = MAXPOINTLIGHTS - 1;
	currentBubbleLights = 0;
	timeBeforeNextBubble = 0;
	wounds = 0;
	points = 0;
	level = 1;
	timeBetweenBubbles = 0.3;
	baseBubbleVelocity = 1.5;
	finish = zSize;
	for (int i = 0; i < 1000; i++)
		Update(0.017, glm::vec3(0, 0, 0));
}

void Aquarium::TryNextLevel()
{
	if (abs(finish - playerBody.position.z) < PLAYERRADIUS * 4)
	{
		points += level * 10;
		printf("Good job, now swim back! You've beaten level %d.\n",level);
		printf("You have %d points.\n", points);
		finish = zSize - finish;
		level++;
		timeBetweenBubbles *= 0.5;
		if (timeBetweenBubbles < 0.05)
			timeBetweenBubbles = 0.05;
		baseBubbleVelocity *= 1.3;
	}
}

void Aquarium::UpdatePlayer(double deltaTime, glm::vec3 thrust)
{
	if (glm::length(playerBody.velocity) > 0)
	{
		if (glm::length(playerBody.velocity) < 0.1)
			playerBody.velocity = glm::vec3(0, 0, 0);
		else
			playerBody.velocity *= (1 - deltaTime * playerDrag) ;
	}
	if (glm::length(thrust) > 0)
	{
		playerBody.velocity += playerAcceleration * thrust * (float)deltaTime;
		if (glm::length(playerBody.velocity) > playerMaxVelocity)
		{
			playerBody.velocity *= playerMaxVelocity / glm::length(playerBody.velocity);
		}
	}
	glm::vec3 newPlayerPosition = playerBody.position + playerBody.velocity * (float)deltaTime;


		//BTW I've only just found out that all the
		// nice constructors like vec4(someVec3, 1) that work in GLSL also work here 
		// (well, there's no .xyz and vec3(someVec4) instead, but still
		// if only I knew earlier...
	

	// rotate the player's model to the desired forward vector.
	// we want to do it nicely, so we split the desired and current forward vector into:
	// horizontal (flat Y)
	// vertical (flat X)

	if (!firstPersonCamera)
	{
		glm::vec3 horizontalVelocity = playerBody.velocity;
		horizontalVelocity.y = 0;
		if (glm::length(horizontalVelocity) > 0.1)
		{
			horizontalVelocity = glm::normalize(horizontalVelocity);
			playerTargetYRot = -atan2(horizontalVelocity.z, horizontalVelocity.x) + M_PI / 2;
		}

		glm::vec3 verticalVelocity = playerBody.velocity;
		verticalVelocity.x = 0;
		if (glm::length(verticalVelocity) > 0.1)
		{
			if (verticalVelocity.z < 0)
				verticalVelocity.z *= -1;
			verticalVelocity = glm::normalize(verticalVelocity);
			playerTargetXRot = -atan2(verticalVelocity.y, verticalVelocity.z);
		}
	}

	float angleDiff = playerTargetYRot - playerYRot;
	if (angleDiff > 2 * M_PI)
		angleDiff -= 2 * M_PI;
	if (angleDiff <= M_PI)
		playerYRot += angleDiff * deltaTime * playerRotateSpeed;
	else
		playerYRot += (angleDiff - 2 * M_PI) * deltaTime * playerRotateSpeed;

	playerXRot += (playerTargetXRot - playerXRot) * deltaTime * playerRotateSpeed;

	playerBody.rotation = glm::rotate(glm::mat4(1.0), playerYRot, glm::vec3(0, 1, 0));
	playerBody.rotation *= glm::rotate(glm::mat4(1.0), playerXRot, glm::vec3(1, 0, 0));

	
	if (!PlayerTouchesBox(newPlayerPosition))
	{
		playerBody.position = newPlayerPosition;
	}
	playerHorns.position = playerBody.position;
	playerHorns.rotation = playerBody.rotation;
	playerEyes.position = playerBody.position;
	playerEyes.rotation = playerBody.rotation;
	playerBulb.position = playerBody.position;
	playerBulb.rotation = playerBody.rotation;
}


void Aquarium::Update(double deltaTime, glm::vec3 thrust)
{
	UpdatePlayer(deltaTime, thrust);
	for (list<Bubble>::iterator i = bubbles.begin(); i != bubbles.end();)
	{
		bool kill = ShouldKill(*i);
		bool collission = PlayerCollission(*i);
		if (collission)
		{
			playerBody.velocity += glm::normalize(playerBody.position - i->position) * 8.0f;
			if (i->light)
			{
				points += level;
				printf("Pop! You have %d points.\n", points);
			}
			else
			{
				wounds++;
				printf("OUCH! You have %d wounds.\n", wounds);
				if (wounds > 3)
				{
					printf("That's lethal, sorry. You finished with %d points.\n", points);
					Restart();
					return;
				}
			}
		}

		if (kill || collission)
		{
			if (i->light)
				currentBubbleLights--;
			i = bubbles.erase(i);
		}
		else
		{
			if (i->scale.x < i->maxSize)
				i->scale += (i->growRate) * deltaTime;
			else
				i->scale += (i->growRate) * deltaTime / 5;
			i->position += i->velocity * (float)deltaTime;
			i++;
		}
	}
	//printf("update %f\n", timeBeforeNextBubble);
	timeBeforeNextBubble -= deltaTime;
	
	TryNextLevel();

	if (timeBeforeNextBubble <= 0)
	{
		SpawnBubble();
		timeBeforeNextBubble = timeBetweenBubbles;
	}
}

void Aquarium::SpawnBubble()
{
	static glm::vec3 colors[]
	{
		glm::vec3( 0.5, 0.05, 0.05),
		glm::vec3(0.05,  0.5, 0.05),
		glm::vec3(0.05, 0.05,  0.5),
		glm::vec3(0.05,  0.5,  0.5),
		glm::vec3( 0.5, 0.05,  0.5),
		glm::vec3( 0.5,  0.5, 0.05),
	};

	float maxSize = 2 * (float)rand() / RAND_MAX + 0.5;
	float x = (float)rand() / RAND_MAX;
	float z = (float)rand() / RAND_MAX;

	x = x * (xSize - maxSize * 2) + maxSize;
	z = z * (zSize - PLAYERRADIUS * 9 * 2) + PLAYERRADIUS * 9;

	int color = rand() % 6;
	Bubble newBubble = Bubble(glm::vec3(x, .25, z), .5);
	newBubble.material.opacity = 0.3;
	newBubble.material.tint = glm::vec3(0.25, 0.25, 0.25) + colors[color] * 1.5f;
	newBubble.material.specular = glm::vec3(0.5, 0.5, 0.5) + colors[color] * 0.2f;
	newBubble.material.emissive = colors[color] * 0.05f;
	newBubble.maxSize = maxSize;
	newBubble.growRate = 0.09 + (float)rand() / RAND_MAX / 2;
	newBubble.velocity = glm::vec3(0, baseBubbleVelocity + 2*(float)rand() / RAND_MAX, 0);
	//printf("Spawning bubble %f %f size %f grow %f until %f \n", x, z, newBubble.scale.z, newBubble.growRate, newBubble.maxSize);
	newBubble.light = false;
	if (currentBubbleLights < maxBubbleLights && rand() % 4 == 0)
	{
		currentBubbleLights++;
		newBubble.light = true;
		newBubble.material.emissive *= 20;
		newBubble.material.specular += glm::vec3(0.2, 0.2, 0.2);
	}
	bubbles.push_back(newBubble);
}


#pragma region boringFunctions
bool Aquarium::PlayerTouchesBox(glm::vec3 newPlayerPosition)
{
	if (abs(newPlayerPosition.x - xSize) < PLAYERRADIUS)
	{
		playerBody.velocity.x = -1;
		return true;
	}
	if (abs(newPlayerPosition.x) < PLAYERRADIUS)
	{
		playerBody.velocity.x = 1;
		return true;
	}
	if (abs(newPlayerPosition.y - ySize) < PLAYERRADIUS)
	{
		playerBody.velocity.y = -1;
		return true;
	}
	if (abs(newPlayerPosition.y) < PLAYERRADIUS)
	{
		playerBody.velocity.y = 1;
		return true;
	}
	if (abs(newPlayerPosition.z - zSize) < PLAYERRADIUS)
	{
		playerBody.velocity.z = -1;
		return true;
	}
	if (abs(newPlayerPosition.z) < PLAYERRADIUS)
	{
		playerBody.velocity.z = 1;
		return true;
	}
	return false;
}

bool Aquarium::ShouldKill(Bubble & b)
{
	return abs(b.position.y - ySize) <= b.scale.z;
}

bool Aquarium::PlayerCollission(Bubble & b)
{
	return glm::length(b.position - playerBody.position) <= PLAYERRADIUS + b.scale.x;
}

Bubble::Bubble(glm::vec3 pos, float s)
	:GameObject(pos)
{
	scale = glm::vec3(s, s, s);
}
#pragma endregion

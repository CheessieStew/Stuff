#include <stdlib.h>
#include <algorithm>
#define _USE_MATH_DEFINES
#include<math.h>
#include <glm/gtc/matrix_transform.hpp>
#include "model.hpp"
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

// player is a sphere with radius 0.8
// bubble is a shpere with radius equal to it's scale

Aquarium::Aquarium(float x, float y, float z):
	xSize(x),
	ySize(y),
	zSize(z)
{
	timeBeforeNextBubble = 1;
	box = GameObject(glm::vec3(x / 2, y / 2, z / 2));
	box.scale = glm::vec3(x / 2, y / 2, z / 2);
	box.material.opacity = .2;
	player = GameObject(glm::vec3(x / 2, y / 2, 3));
	playerMaxVelocity = 5;
	playerAcceleration = 17;
	playerDecceleration = 8;
	playerRotateSpeed = 1.1;
}

void Aquarium::UpdatePlayer(double deltaTime, glm::vec3 thrust)
{
	if (glm::length(thrust) == 0 && glm::length(player.velocity) > 0)
	{
		//printf("slowing down");
		if (glm::length(player.velocity) < 0.1)
			player.velocity = glm::vec3(0, 0, 0);
		else
			player.velocity *= (1 - deltaTime);
	}
	else if (glm::length(thrust) > 0)
	{
		player.velocity += playerAcceleration * thrust * (float)deltaTime;
		if (glm::length(player.velocity) > playerMaxVelocity)
		{
			player.velocity *= playerMaxVelocity / glm::length(player.velocity);
		}
	}
	glm::vec3 newPlayerPosition = player.position + player.velocity * (float)deltaTime;
	glm::vec3 horizontalVelocity = glm::vec3(player.velocity.x, 0, player.velocity.z);
	glm::vec3 mov3 = glm::normalize(horizontalVelocity);
	glm::vec4 playerMovement = (glm::vec4(mov3.x, mov3.y, mov3.z, 1));
	glm::vec4 playerForward = player.rotation * glm::vec4(0, 0, 1, 1);
	glm::vec3 fwd3 = glm::normalize(glm::vec3(playerForward.x, 0, playerForward.z));

	if (glm::length(horizontalVelocity) != 0 && glm::dot(mov3, fwd3) < 0.999999)
	{
		float angle = -acos(glm::dot(mov3, fwd3));
		glm::vec3 axis = glm::cross(mov3, fwd3);

		// let's say we want to rotate by up to playerRotateSpeed per second
		// and we also don't want to rotate further than needed
		float sgn = angle / abs(angle);
		float possibleAngle = playerRotateSpeed * deltaTime;
		if (possibleAngle > abs(angle))
			possibleAngle = angle;
		else
			possibleAngle *= sgn;
		player.rotation *= glm::rotate(glm::mat4(1.0), possibleAngle, axis);
	}

	// this was supposed to make the fish "want to" have it's dorsal fin directed towards top
	// back when it was turning towards it's actual velocity, not just "flat" velocity with y reduced to 0,
	// but some weird NaN stuff was happening
	//glm::vec3 top3 = glm::vec3(0, 1, 0);
	//glm::vec4 playerTop = player.rotation * glm::vec4(0, 1, 0, 1);
	//glm::vec3 ptop3 = glm::normalize(glm::vec3(playerTop.x, playerTop.y, playerTop.z));
	//if (glm::dot(ptop3, top3) != 1)
	//{
	//	float angle = -acos(glm::dot(ptop3, top3));
	//	glm::vec3 axis = glm::cross(ptop3, top3);
	//	printf("angle = %f, dot = %f (playertop %f %f %f %f)\n", angle, glm::dot(ptop3, top3), playerTop.x, playerTop.y, playerTop.z, playerTop.w);
	//
	//	printf("axis = %f %f %f\n", axis.x, axis.y, axis.z);
	//	float constant = 2;
	//	if (abs(angle) > 0.5)
	//	{
	//		constant *= angle / 4;
	//	}
	//	player.rotation *= glm::rotate(glm::mat4(1.0), angle * constant *(float)deltaTime, axis);
	//}
	
	if (!PlayerTouchesBox(newPlayerPosition))
	player.position = newPlayerPosition;
}

bool Aquarium::PlayerTouchesBox(glm::vec3 newPlayerPosition)
{
	if (abs(newPlayerPosition.x - box.scale.x * 2) < 0.8)
	{
		player.velocity.x = -1;
		return true;
	}
	if (abs(newPlayerPosition.x) < 0.8)
	{
		player.velocity.x = 1;
		return true;
	}
	if (abs(newPlayerPosition.y - box.scale.y * 2) < 0.8)
	{
		player.velocity.y = -1;
		return true;
	}
	if (abs(newPlayerPosition.y) < 0.8)
	{
		player.velocity.y = 1;
		return true;
	}
	if (abs(newPlayerPosition.z - box.scale.z * 2) < 0.8)
	{
		player.velocity.z = -1;
		return true;
	}
	if (abs(newPlayerPosition.z) < 0.8)
	{
		player.velocity.z = 1;
		return true;
	}
	return false;
}

void Aquarium::Update(double deltaTime, glm::vec3 thrust)
{
	UpdatePlayer(deltaTime, thrust);
	for (list<Bubble>::iterator i = bubbles.begin(); i != bubbles.end(); )
		if (ShouldKill(*i))
			i = bubbles.erase(i);
		else
		{
			if (i->scale.x < i->maxSize)
				i->scale += (i->growRate) * deltaTime;
			else
				i->scale += (i->growRate) * deltaTime / 5;
			i->position += i->velocity * (float)deltaTime;
			i++;
		}
	//printf("update %f\n", timeBeforeNextBubble);
	timeBeforeNextBubble -= deltaTime;
	if (timeBeforeNextBubble <= 0)
	{
		SpawnBubble();
		timeBeforeNextBubble = .1;
	}
}

bool Aquarium::ShouldKill(Bubble & b)
{
	return abs(b.position.y - box.scale.y * 2) <= b.scale.z;
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

	float x = (float)rand() / RAND_MAX;
	float z = (float)rand() / RAND_MAX;

	x = (x * 2) * (box.scale.x - 2) + 2;
	z = (z * 2) * (box.scale.z - 8) + 8;

	int color = rand() % 6;
	Bubble newBubble = Bubble(glm::vec3(x, .25, z), .5);
	newBubble.material.opacity = 0.2;
	newBubble.material.tint = glm::vec3(0.25, 0.25, 0.25) + colors[color] * 1.5f;
	newBubble.material.specular = glm::vec3(0.5, 0.5, 0.5) + colors[color] * 0.2f;
	newBubble.material.emissive = colors[color] * 0.2f;
	newBubble.maxSize = (float)rand() / RAND_MAX + 0.5;
	newBubble.growRate = 0.09 + (float)rand() / RAND_MAX / 2;
	newBubble.velocity = glm::vec3(0, 1.5 + 2*(float)rand() / RAND_MAX, 0);
	//printf("Spawning bubble %f %f size %f grow %f until %f \n", x, z, newBubble.scale.z, newBubble.growRate, newBubble.maxSize);

	bubbles.push_back(newBubble);
}

Bubble::Bubble(glm::vec3 pos, float s)
	:GameObject(pos)
{
	scale = glm::vec3(s, s, s);
}

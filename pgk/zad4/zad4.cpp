
#include <stdio.h>
#include <stdlib.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <ctime>
#include "model.hpp"
#include "graphics.hpp"
using namespace glm;
#include <common/shader.hpp>


void ScrollCallback(GLFWwindow* window, double xoff, double yoff);
double xscroll = 0; //irrelevant
double yscroll = 0;

void KeyCallback(GLFWwindow* window, int key, int code, int action, int mods);
vec3 thrust = vec3(0, 0, 0);
bool pause = false;

bool CompareBubbles(const Bubble& first, const Bubble& second);
glm::vec3 cameraPosition;

int main(int argc, char * argv[]) {

#pragma region parseArgs

#pragma endregion

	GLFWwindow* window;

// straight from tutorial
#pragma region GL_init

	// Initialise GLFW
	if (!glfwInit())
	{
		fprintf(stderr, "Failed to initialize GLFW\n");
		getchar();
		return -1;
	}

	glfwWindowHint(GLFW_SAMPLES, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE); // To make MacOS happy; should not be needed
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

	// Open a window and create its OpenGL context
	window = glfwCreateWindow(1024, 768, "Tutorial 02 - Red triangle", NULL, NULL);
	if (window == NULL) {
		fprintf(stderr, "Failed to open GLFW window. If you have an Intel GPU, they are not 3.3 compatible. Try the 2.1 version of the tutorials.\n");
		getchar();
		glfwTerminate();
		return -1;
	}
	glfwMakeContextCurrent(window);

	// Initialize GLEW
	glewExperimental = true; // Needed for core profile
	if (glewInit() != GLEW_OK) {
		fprintf(stderr, "Failed to initialize GLEW\n");
		getchar();
		glfwTerminate();
		return -1;
	}

#pragma endregion


	// Ensure we can capture the escape key being pressed below
	glfwSetInputMode(window, GLFW_STICKY_KEYS, GL_TRUE);
	// Hide the mouse and enable unlimited mouvement
	glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

	// Set the mouse at the center of the screen
	glfwPollEvents();
	glfwSetCursorPos(window, 1024 / 2, 768 / 2);

	// bright Background
	glClearColor(0.01, 0.02, 0.1, 1.0f);


	glEnable(GL_LINE_SMOOTH);
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);
	glEnable(GL_CULL_FACE);
	glEnable(GL_BLEND); glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	//glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	//glEnable(GL_BLEND);
	GLuint VertexArrayID;
	glGenVertexArrays(1, &VertexArrayID);
	glBindVertexArray(VertexArrayID);

	glm::mat4 projection = glm::perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);


	double timer = glfwGetTime();
	double deltaTime = 0;
	srand(time(NULL));
	int seed = rand();

	GLuint DefaultTexture(loadDDS("default.DDS"));
	GLuint BubbleTexture(loadDDS("bubble.DDS"));

	Model3d FishModel(Model3d("anglerfish_body.obj"));
	Model3d BulbModel(Model3d("anglerfish_bulb.obj"));
	Model3d HornsModel(Model3d("anglerfish_horns.obj"));
	Model3d EyesModel(Model3d("anglerfish_eyes.obj"));

	Model3d BubbleModel(Model3d("bubble.obj"));
	Model3d AquariumModel(Model3d("aquarium.obj"));

	GLuint FishShader(LoadShaders("VertexShader.vert", "FishFragmentShader.frag"));
	GLuint DefaultShader(LoadShaders("VertexShader.vert", "DefaultFragmentShader.frag"));
	GLuint BestShaderEver(LoadShaders("VertexShader.vert", "BubbleFragmentShader.frag"));

	vec3 playerLightRelative = vec3(0.062, 0.48, 1.49);

	Aquarium aquarium = Aquarium(30,30,60);
	aquarium.maxBubbleLights = 20;
	Light pointLights[21];
	GameObject3d player3d = GameObject3d(aquarium.playerBody, BestShaderEver, DefaultTexture, FishModel);
	GameObject3d playerHorns3d = GameObject3d(aquarium.playerHorns, BestShaderEver, DefaultTexture, HornsModel);
	GameObject3d playerEyes3d = GameObject3d(aquarium.playerEyes, BestShaderEver, DefaultTexture, EyesModel);
	GameObject3d playerBulb3d = GameObject3d(aquarium.playerBulb, BestShaderEver, DefaultTexture, BulbModel);
	GameObject3d aquarium3d = GameObject3d(aquarium.box, BestShaderEver, DefaultTexture, AquariumModel);
	float camspeed = 2;
	float rotX = 0;
	float rotY = 0;
	float zoomout = 5;

	vec3 OutsideCamPos = vec3(-40, 15, 30);

	glfwSetScrollCallback(window, ScrollCallback);
	glfwSetKeyCallback(window, KeyCallback);
	do {
		double newTimer = glfwGetTime();
		deltaTime = newTimer - timer;
		timer = newTimer;
		zoomout -= 0.3 * yscroll;
		//if (zoomout < 2)
		//	zoomout = 2;
		yscroll = 0;

#pragma region cameraSetup
		double xpos, ypos;
		glfwGetCursorPos(window, &xpos, &ypos);
		if (xpos > 50 && xpos < 950 && ypos > 50 && ypos < 799)
		{
			rotX += camspeed * sin(0.001 * float(1024 / 2 - xpos));
			rotY += camspeed * sin(0.001 * float(768 / 2 - ypos));
			if (rotY > 0.45 * M_PI)
				rotY = 0.45 * M_PI;
			if (rotY < -0.45 * M_PI)
				rotY = -0.45 * M_PI;
			glfwSetCursorPos(window, 1024 / 2, 768 / 2);
		}
		vec4 CamRelative = vec4(0, 0, -zoomout, 1);
		CamRelative = rotate(mat4(1.0), rotY, vec3(1, 0, 0)) * CamRelative;
		CamRelative = rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * CamRelative;
		cameraPosition = aquarium.playerBody.position + vec3(CamRelative.x, CamRelative.y, CamRelative.z);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glm::mat4 view = glm::lookAt(
			cameraPosition,
			aquarium.playerBody.position,
			glm::vec3(0, 1, 0)
			);
#pragma endregion

		vec4 thrust4 = vec4(thrust.x, thrust.y, thrust.z, 1);
		thrust4 = rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * thrust4;
		if (!pause)
			aquarium.Update(deltaTime, vec3(thrust4.x, thrust4.y, thrust4.z));

#pragma region draw
		pointLights[0] = Light(playerBulb3d.Object, playerLightRelative, 20);
		int lightsnumber = 1;
		player3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		playerBulb3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		playerHorns3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		aquarium3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		playerEyes3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		aquarium.bubbles.sort(CompareBubbles);

		//Bubble indicator = Bubble(player3d.Object.position + playerLightRelative, 0.2);
		//indicator.material.emissive = vec3(0.7, 0.7, 0.7);
		//GameObject3d indicator3d = GameObject3d(indicator, BestShaderEver, DefaultTexture, BubbleModel);
		//indicator3d.Draw(&view, &projection, cameraPosition);
		for (list<Bubble>::iterator i = aquarium.bubbles.begin(); i != aquarium.bubbles.end(); i++)
		{
			//printf("size %f \n", (*i).scale.z);
			GameObject3d bubble3d = GameObject3d((*i), BestShaderEver, BubbleTexture, BubbleModel);
			bubble3d.Draw(&view, &projection, cameraPosition, pointLights, lightsnumber);
		}
		glfwSwapBuffers(window);
		glfwPollEvents();
	
#pragma endregion

#pragma region getInput

#pragma endregion


	}
	while (glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS &&
		glfwWindowShouldClose(window) == 0);


	glDeleteVertexArrays(1, &VertexArrayID);
	glfwTerminate();

	return 0;
}


void ScrollCallback(GLFWwindow* window, double xoff, double yoff)
{
	static double xbase = 0;
	static double ybase = 0;
	xscroll = xoff;
	yscroll = yoff;
}

void KeyCallback(GLFWwindow* window, int key, int code, int action, int mods)
{
	static bool up = false;
	static bool down = false;
	static bool left = false;
	static bool right = false;
	static bool forward = false;
	static bool backward = false;

	if (action == GLFW_PRESS)
		switch (key)
		{
		case GLFW_KEY_F1:
			pause = !pause;
			break;
		case GLFW_KEY_SPACE:
			up = true;
			if (!down)
				thrust.y = 1;
			break;
		case GLFW_KEY_LEFT_CONTROL:
			down = true;
			if (!up)
				thrust.y = -1;
			break;
		case GLFW_KEY_D:
			left = true;
			if (!right)
				thrust.x = -1;
			break;
		case GLFW_KEY_A:
			right = true;
			if (!left)
				thrust.x = 1;
			break;
		case GLFW_KEY_W:
			forward = true;
			if (!backward)
				thrust.z = 1;
			break;
		case GLFW_KEY_S:
			backward = true;
			if (!forward)
				thrust.z = -1;
			break;
		}
	if (action == GLFW_RELEASE)
		switch (key)
		{
		case GLFW_KEY_SPACE:
			up = false;
			if (down)
				thrust.y = -1;
			else thrust.y = 0;
			break;
		case GLFW_KEY_LEFT_CONTROL:
			down = false;
			if (up)
				thrust.y = 1;
			else thrust.y = 0;
			break;
		case GLFW_KEY_D:
			left = false;
			if (right)
				thrust.x = 1;
			else thrust.x = 0;
			break;
		case GLFW_KEY_A:
			right = false;
			if (left)
				thrust.x = -1;
			else thrust.x = 0;
			break;
		case GLFW_KEY_W:
			forward = false;
			if (backward)
				thrust.z = -1;
			else thrust.z = 0;
			break;
		case GLFW_KEY_S:
			backward = false;
			if (forward)
				thrust.z = 1;
			else thrust.z = 0;
			break;
		}
}

bool CompareBubbles(const Bubble& first, const Bubble& second)
{
	float d1 = glm::length(first.position - cameraPosition);
	float d2 = glm::length(second.position - cameraPosition);
	return (d1 > d2);
}

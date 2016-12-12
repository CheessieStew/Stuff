
#include <stdio.h>
#include <stdlib.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <ctime>
#include "model.hpp"
#include "graphics.hpp"
using namespace glm;
#include <../common/shader.hpp>
#define WINDOWWIDTH 1280
#define WINDOWHEIGHT 1024

void ScrollCallback(GLFWwindow* window, double xoff, double yoff);
double xscroll = 0; //irrelevant
double yscroll = 0;

void KeyCallback(GLFWwindow* window, int key, int code, int action, int mods);
vec3 thrust = vec3(0, 0, 0);
bool pause = false;
bool firstPersonCamera = false;

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
	window = glfwCreateWindow(WINDOWWIDTH, WINDOWHEIGHT, "Glou Glou", NULL, NULL);
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
	glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);

	glClearColor(0.01, 0.02, 0.1, 1.0f);


	glEnable(GL_LINE_SMOOTH);
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);
	glEnable(GL_CULL_FACE);
	glEnable(GL_BLEND); 
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);


	GLuint VertexArrayID;
	glGenVertexArrays(1, &VertexArrayID);
	glBindVertexArray(VertexArrayID);

	glm::mat4 projection = glm::perspective(45.0f, ((float)WINDOWWIDTH)/WINDOWHEIGHT, 0.1f, 100.0f);


	double timer = glfwGetTime();
	double deltaTime = 0;
	srand(time(NULL));
	int seed = rand();

	GLuint DefaultTexture(loadDDS("Assets/default.DDS"));
	GLuint BubbleTexture(loadDDS("Assets/bubble.DDS"));

	Model3d FishModel(Model3d("Assets/anglerfish_body.obj"));
	Model3d BulbModel(Model3d("Assets/anglerfish_bulb.obj"));
	Model3d HornsModel(Model3d("Assets/anglerfish_horns.obj"));
	Model3d EyesModel(Model3d("Assets/anglerfish_eyes.obj"));

	Model3d BubbleModel(Model3d("Assets/bubble.obj"));
	Model3d AquariumModel(Model3d("Assets/aquarium.obj"));

	GLuint BestShaderEver(LoadShaders("Assets/BestVertexShaderEver.vert", "Assets/BestFragmentShaderEver.frag"));

	//it's the relative position of the bulb to origin in model space
	vec3 playerLightRelative = vec3(0.062, 0.32, 1.49);

	Aquarium aquarium = Aquarium(16,24,90);
	aquarium.firstPersonCamera = false;
	Light* pointLights = new Light[aquarium.maxBubbleLights + 1]; // 1 for the player's light
	int maxLights = aquarium.maxBubbleLights + 1;
	GameObject3d player3d = GameObject3d(aquarium.playerBody, DefaultTexture, FishModel);
	GameObject3d playerHorns3d = GameObject3d(aquarium.playerHorns, DefaultTexture, HornsModel);
	GameObject3d playerEyes3d = GameObject3d(aquarium.playerEyes, DefaultTexture, EyesModel);
	GameObject3d playerBulb3d = GameObject3d(aquarium.playerBulb, DefaultTexture, BulbModel);
	GameObject3d aquarium3d = GameObject3d(aquarium.box, DefaultTexture, AquariumModel);
	float camspeed = 2;
	float rotX = 0;
	float rotY = 0;
	float zoomout = 5;
	float firstPersonZoomOut = 1;
	//vec3 OutsideCamPos = vec3(-40, 15, 30);

	glfwSetScrollCallback(window, ScrollCallback);
	glfwSetKeyCallback(window, KeyCallback);

	printf("\n  WASD, ctrl, space - movement\n  mouse - camera movement\n  tab - change camera mode \n  f1 - pause\n  esc - quit\n");

	//main loop
	do {
		double newTimer = glfwGetTime();
		deltaTime = newTimer - timer;
		timer = newTimer;


#pragma region cameraSetup
		glm::mat4 view;
		if (!firstPersonCamera)
		{
			if (aquarium.firstPersonCamera)
				aquarium.firstPersonCamera = false;
			zoomout -= 0.3 * yscroll;
			//if (zoomout < 3)
			//	zoomout = 3;
			yscroll = 0;

			double xpos, ypos;
			glfwGetCursorPos(window, &xpos, &ypos);
			glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);

			rotX += camspeed * sin(0.001 * float(WINDOWWIDTH / 2 - xpos));
			rotY -= camspeed * sin(0.001 * float(WINDOWHEIGHT / 2 - ypos));
			if (rotY > 0.45 * M_PI)
				rotY = 0.45 * M_PI;
			if (rotY < -0.45 * M_PI)
				rotY = -0.45 * M_PI;

			vec4 CamRelative = vec4(0, 0, -zoomout, 1);
			CamRelative = rotate(mat4(1.0), rotY, vec3(1, 0, 0)) * CamRelative;
			CamRelative = rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * CamRelative;
			cameraPosition = aquarium.playerBody.position + vec3(CamRelative.x, CamRelative.y, CamRelative.z);
		}
		else // first person camera is a two-stage job
			// we tell the model where we want to look
		   // and the model tells us where we CAN look
		{
			if (!aquarium.firstPersonCamera)
				aquarium.firstPersonCamera = true;
			yscroll = 0; // we don't use the scroll yet,
			// but we should pretend we do

			double xpos, ypos;
			glfwGetCursorPos(window, &xpos, &ypos);
			glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);
			if (!pause)
			{
				aquarium.playerTargetYRot += camspeed * sin(0.001 * float(WINDOWWIDTH / 2 - xpos));
				aquarium.playerTargetXRot -= camspeed * sin(0.001 * float(WINDOWHEIGHT / 2 - ypos));
				aquarium.playerTargetXRot = clamp(aquarium.playerTargetXRot, -0.48f * (float)M_PI, 0.48f * (float)M_PI);
			}
		
			//view = glm::lookAt(
			//	cameraPosition,
			//	aquarium.playerBody.position,
			//	vec3(0, 1, 0));
		}
#pragma endregion

		vec3 rotatedThrust;
		if (firstPersonCamera)
		{
			rotatedThrust = vec3(aquarium.playerBody.rotation * vec4(thrust, 1));
		}
		else
			rotatedThrust = vec3(rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * vec4(thrust, 1));
		if (!pause)
			aquarium.Update(deltaTime, rotatedThrust);

		// camera setup part 2
		if (firstPersonCamera)
		{
			float rotX = aquarium.playerXRot;
			float rotY = aquarium.playerYRot;

			cameraPosition = aquarium.playerBody.position;
			//cameraPosition = vec3(aquarium.xSize/2, aquarium.ySize, 0);
			vec3 pointToLookAt = vec3(0, 0, 1);
			pointToLookAt = vec3(aquarium.playerBody.rotation * vec4(pointToLookAt, 1));
			//printf("point to look at %f %f %f\n", pointToLookAt.x, pointToLookAt.y, pointToLookAt.z);
			view = glm::lookAt(
				cameraPosition,
				cameraPosition + pointToLookAt,
				vec3(0, 1, 0));
		}
		else
		{
			view = glm::lookAt(
				cameraPosition,
				aquarium.playerBody.position,
				vec3(0, 1, 0));
		}

#pragma region draw
		vec3 mistColor = vec3(0.01, 0.05, 0.07) * (8.0f/aquarium.level);
		//mistColor = (mistColor * 3.0f + vec3(0.6, 0.05, 0.02) * (float)aquarium.wounds)/(3.0f+aquarium.wounds);
		if (aquarium.wounds > 0)
		{
			mistColor = vec3(0.5 + 0.15 * aquarium.wounds, 0.4 - 0.1 * aquarium.wounds, 0.6 - 0.2 * aquarium.wounds)
				* length(mistColor);
		}
		float mistThickness = 0.03 + 0.015 * (aquarium.level > 4 ? 4 : aquarium.level) + 0.03 * aquarium.wounds;

		float mainLightIntensity = 1.6 - 0.2 * aquarium.level;
		if (mainLightIntensity < 0.2)
			mainLightIntensity = 0.2;

		pointLights[0] = Light(playerBulb3d.Object, playerLightRelative, 15);
		int lightsnumber = 1;
		for (list<Bubble>::iterator i = aquarium.bubbles.begin(); i != aquarium.bubbles.end(); i++)
		{
			if (i->light)
			{
				pointLights[lightsnumber] = Light((*i), glm::vec3(0, 0, 0), 13);
				lightsnumber++;
			}
		}
		glUseProgram(BestShaderEver);
		EnvironmentSetup(BestShaderEver, mainLightIntensity, pointLights, lightsnumber, mistColor, mistThickness);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		aquarium3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		player3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		playerEyes3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		playerBulb3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		playerHorns3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		aquarium.bubbles.sort(CompareBubbles);

		for (list<Bubble>::iterator i = aquarium.bubbles.begin(); i != aquarium.bubbles.end(); i++)
		{
			//printf("size %f \n", (*i).scale.z);
			GameObject3d bubble3d = GameObject3d((*i), BubbleTexture, BubbleModel);
			bubble3d.Draw(BestShaderEver, &view, &projection, cameraPosition);
		}

#pragma endregion
		glfwSwapBuffers(window);
		glfwPollEvents();
	}
	while (glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS &&
		glfwWindowShouldClose(window) == 0);


	AquariumModel.CleanUp();
	FishModel.CleanUp();
	BulbModel.CleanUp();
	HornsModel.CleanUp();
	EyesModel.CleanUp();
	BubbleModel.CleanUp();
	glDeleteProgram(BestShaderEver);
	glDeleteTextures(1, &DefaultTexture);
	glDeleteTextures(1, &BubbleTexture);
	glDeleteVertexArrays(1, &VertexArrayID);
	delete[] pointLights;
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
		case GLFW_KEY_TAB:
			firstPersonCamera = !firstPersonCamera;
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

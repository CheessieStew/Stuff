
#include <stdio.h>
#include <stdlib.h>
#define _USE_MATH_DEFINES

#include <math.h>
#include <algorithm>
#include <ctime>
#include "terrain.hpp"
#include "grid.hpp"
#include <glm/gtc/matrix_transform.hpp>
using namespace std;

using namespace glm;
#include <common/shader.hpp>
#define WINDOWWIDTH 1280
#define WINDOWHEIGHT 1024

void ScrollCallback(GLFWwindow* window, double xoff, double yoff);
double xscroll = 0; //irrelevant
double yscroll = 0;

void KeyCallback(GLFWwindow* window, int key, int code, int action, int mods);
int chosenLoD = 3;
vec3 thrust = vec3(0, 0, 0);
bool pause = false;
bool firstPersonCamera = false;
bool ballMode = false;


glm::vec3 cameraPosition;

int main(int argc, char * argv[]) {


	GLFWwindow* window;
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
	window = glfwCreateWindow(WINDOWWIDTH, WINDOWHEIGHT, "Earth Stuff", NULL, NULL);
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
#pragma region parseArgs

	int filesToLoad = 0;
	bool showFPS = false;
	float lowX = 180, lowY = 90,
		highX = -180, highY = -90;
	vector<TerrainChunk> chunks;
	for (int i = 1; i < argc; i++)
	{
		if (strcmp(argv[i], "-showFPS") == 0)
		{
			printf("Show fps requested\n");
			showFPS = true;
		}
		else
		{
			char name[180];
			int k = 0;
			for (char* a = argv[i]; *a != 0; a++)
			{
				if (*a == '\\')
				{
					name[k] = '/';
				}
				else
					name[k] = *a;
				k++;
			}
			name[k] = 0;
			try
			{
				TerrainChunk chunk(name);
				if (chunk.xStart + 1 > highX)
					highX = chunk.xStart + 1;
				if (chunk.xStart < lowX)
					lowX = chunk.xStart;
				if (chunk.yStart + 1 > highY)
					highY = chunk.yStart + 1;
				if (chunk.yStart < lowY)
					lowY = chunk.yStart;
				chunks.push_back(chunk);
			}
			catch (...)
			{
				printf("Can't load file: %s\n", argv[i]);
				return 0;
			}
			
		}
	}
	if (highX < lowX)
	{
		highX = 45;
		lowX = -0;
	}
	if (highY < lowY)
	{
		highY = 45;
		lowY = -0;
	}
	printf("Files loaded: %d.\n", chunks.size());
	sort(chunks.begin(), chunks.end());
	for (int i = 0; i < chunks.size(); i++)
		printf("%d %d\n", chunks[i].xStart, chunks[i].yStart);
#pragma endregion

	// Ensure we can capture the escape key being pressed below
	glfwSetInputMode(window, GLFW_STICKY_KEYS, GL_TRUE);
	// Hide the mouse and enable unlimited movement
	glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_DISABLED);

	// Set the mouse at the center of the screen
	glfwPollEvents();
	glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);

	glClearColor(0.2, 0.2, 0.6, 1.0f);
	GLuint BallShader(LoadShaders("Assets/BallVertexShader.vert", "Assets/FragmentShader.frag"));
	GLuint FlatShader(LoadShaders("Assets/FlatVertexShader.vert", "Assets/FragmentShader.frag"));


	glEnable(GL_LINE_SMOOTH);
	glEnable(GL_DEPTH_TEST);
	glDepthFunc(GL_LESS);
	glEnable(GL_CULL_FACE);
	glEnable(GL_BLEND); 
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	GLuint VertexArrayID;
	glGenVertexArrays(1, &VertexArrayID);
	glBindVertexArray(VertexArrayID);

	glfwSetScrollCallback(window, ScrollCallback);
	glfwSetKeyCallback(window, KeyCallback);
	GLuint BallMatrixID = glGetUniformLocation(BallShader, "MVP"); //mat4
	GLuint BallCamPosID = glGetUniformLocation(BallShader, "cameraPosition_worldSpace"); //vec3
	GLuint BallInfoID = glGetUniformLocation(BallShader, "info"); //bool
	GLuint BallCoordsID = glGetUniformLocation(BallShader, "cellCoords"); //vec2

	GLuint FlatLowerBoundID = glGetUniformLocation(FlatShader, "lowerBound"); //vec2
	GLuint FlatUpperBoundID = glGetUniformLocation(FlatShader, "upperBound"); //vec2
	GLuint FlatInfoID = glGetUniformLocation(FlatShader, "info"); //bool
	GLuint FlatCoordsID = glGetUniformLocation(FlatShader, "cellCoords"); //vec2
	//printf("d");

	Grid grid = Grid(1201, 3);
	glm::mat4 MVP;
	glm::mat4 view;
	glm::mat4 projection = glm::perspective(45.0f, ((float)WINDOWWIDTH) / WINDOWHEIGHT, 0.1f, 100000.0f);

	double timer = glfwGetTime();
	double deltaTime = 0;
	int autoLoD = 3;
	int curLoD = 3;
	float averageWeight = 0.6f;
	float averageFPS = 0;
	float secondsSinceLastChance = 0;

	float camspeed = 19;
	float rotX = 0;
	float rotY = 0;
	bool firstPersonDisabled = true;
	vec3 firstPersonForward;
	float firstPersonX = 0;
	float firstPersonY = 0;
	float zoomout = 10000;
	const char* autostr = "(auto)";
	const char* noautostr = "";


	//main loop
	do {
		double newTimer = glfwGetTime();
		deltaTime = newTimer - timer;
		secondsSinceLastChance += deltaTime;
		timer = newTimer;
		float fps = 1 / deltaTime;
		if (deltaTime > 0)
			averageFPS = averageWeight * fps + (1 - averageWeight) * averageFPS;

		curLoD = chosenLoD;
		if (curLoD == 0)
		{
			if (secondsSinceLastChance >= 1)
			{
				secondsSinceLastChance = 0;
				if (averageFPS <= 20 && autoLoD < 3)
					autoLoD++;
				if (averageFPS > 40 && autoLoD > 1)
					autoLoD--;
			}
			curLoD = autoLoD;
		}

		if (showFPS)
		{
			printf("FPS %f, average %f, LoD %d %s\n", fps, averageFPS, curLoD, chosenLoD == 0 ? autostr : noautostr);
		}



		camspeed = 8000 + pow(zoomout - 6372, 1.4) * 4;

#pragma region cameraSetup
		if (ballMode)
		{
			if (!firstPersonCamera)
			{
				if (!firstPersonDisabled)
				{
					firstPersonDisabled = true;
				}
				zoomout -= 0.04 * camspeed * yscroll * deltaTime;
				if (zoomout < 6372)
					zoomout = 6372;
				if (zoomout > 15000)
					zoomout = 15000;
				yscroll = 0;
				//printf("Zoomout %f\n", zoomout);
				double xpos, ypos;
				glfwGetCursorPos(window, &xpos, &ypos);
				glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);

				rotX += camspeed * 0.00005 * sin(0.001 * float(WINDOWWIDTH / 2 - xpos)) * deltaTime;
				rotY += camspeed * 0.00005 * sin(0.001 * float(WINDOWHEIGHT / 2 - ypos)) * deltaTime;
				if (rotY > 0.495 * M_PI)
					rotY = 0.495 * M_PI;
				if (rotY < -0.495 * M_PI)
					rotY = -0.495 * M_PI;

				vec4 CamRelative = vec4(0, 0, zoomout, 1);
				CamRelative = rotate(mat4(1.0), rotY, vec3(1, 0, 0)) * CamRelative;
				CamRelative = rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * CamRelative;
				cameraPosition = vec3(CamRelative);
				view = glm::lookAt(
					cameraPosition,
					vec3(0, 0, 0),
					vec3(0, 1, 0));
			}
			else
			{
				if (firstPersonDisabled)
				{
					firstPersonDisabled = false;


					firstPersonX = 0;
					firstPersonY = 0;
				}
				double xpos, ypos;
				glfwGetCursorPos(window, &xpos, &ypos);
				glfwSetCursorPos(window, WINDOWWIDTH / 2, WINDOWHEIGHT / 2);

				vec3 rotThrust = vec3(rotate(mat4(1.0), firstPersonX, vec3(0, 1, 0)) * vec4(thrust, 1));

				firstPersonX += 2 * sin(0.001 * float(WINDOWWIDTH / 2 - xpos));
				firstPersonY -= 2 * sin(0.001 * float(WINDOWHEIGHT / 2 - ypos));

				rotY -= camspeed * 0.000002 * rotThrust.z * deltaTime;
				rotX -= camspeed * 0.000002 * rotThrust.x * deltaTime / cos(rotY);
				yscroll = 0;


				if (firstPersonY < -0.99 * M_PI)
				{
					firstPersonY = -0.99 * M_PI;
				}
				if (firstPersonY > -0.01 * M_PI)
				{
					firstPersonY = -0.01 * M_PI;
				}
				zoomout += camspeed * 0.03 * rotThrust.y * deltaTime;
				if (zoomout < 6372)
					zoomout = 6372;
				if (zoomout > 15000)
					zoomout = 15000;

				vec4 CamRelative = vec4(0, 0, zoomout, 1);
				CamRelative = rotate(mat4(1.0), rotY, vec3(1, 0, 0)) * CamRelative;
				CamRelative = rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * CamRelative;
				cameraPosition = vec3(CamRelative);



				firstPersonForward = vec3(0, 0.001, -1);
				firstPersonForward = vec3(rotate(mat4(1.0), rotY, vec3(1, 0, 0)) * vec4(firstPersonForward, 1));
				firstPersonForward = vec3(rotate(mat4(1.0), rotX, vec3(0, 1, 0)) * vec4(firstPersonForward, 1));
				firstPersonForward = vec3(rotate(mat4(1.0), firstPersonX, cameraPosition) * vec4(firstPersonForward, 1));
				vec3 right = cross(firstPersonForward, -normalize(cameraPosition));
				firstPersonForward = vec3(rotate(mat4(1.0), firstPersonY, right) * vec4(firstPersonForward, 1));
				vec3 lookAtp = cameraPosition + firstPersonForward;// -normalize(cameraPosition);
				view = lookAt(
					cameraPosition,
					lookAtp,
					cameraPosition);
			}
		}
		else
		{
			float zzz = - yscroll * deltaTime * 8;
			yscroll = 0;
			if (highX + zzz > lowX - zzz &&
				highY + zzz > lowY - zzz)
			{
				highX += zzz;
				highY += zzz;
				lowX -= zzz;
				lowY -= zzz;
			}
			highX -= 14 * thrust.x * deltaTime;
			lowX -= 14 * thrust.x * deltaTime;
			highY += 14 * thrust.z * deltaTime;
			lowY += 14 * thrust.z * deltaTime;

		}
#pragma endregion
	
#pragma region draw
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		if (ballMode)
		{
			glUseProgram(BallShader);
			MVP = projection * view;

			glUniformMatrix4fv(BallMatrixID, 1, GL_FALSE, &(MVP[0][0]));
			glUniform3f(BallCamPosID, cameraPosition.x, cameraPosition.y, cameraPosition.z);
			glUniform1i(BallInfoID, 0);

		}
		else
		{
			glUseProgram(FlatShader);
			glUniform1i(FlatInfoID, 0);
			glUniform2f(FlatLowerBoundID, lowX, lowY);
			glUniform2f(FlatUpperBoundID, highX, highY);
		}

		grid.BindLoDNoDetail();
		glEnableVertexAttribArray(0);

		grid.BindVertices();


		int chunki = 0;
		//printf("----\n");
		for (int i = -180; i < 180; i++)
		{
			for (int j = -90; j <= 90; j++)
			{
				if (ballMode)
					glUniform2f(BallCoordsID, i, j);
				else
					glUniform2f(FlatCoordsID, i, j);
				
				if (chunki < chunks.size() && 
					chunks[chunki].xStart == i && 
					chunks[chunki].yStart == j)
				{
					grid.BindLoD(curLoD - 1);
					glEnableVertexAttribArray(1);
					if (ballMode)
						glUniform1i(BallInfoID, 1);
					else
						glUniform1i(FlatInfoID, 1);

					chunks[chunki].BindHeight();
					glDrawElements(GL_TRIANGLES, grid.HowManyElements(), GL_UNSIGNED_INT, (void*)(0));

					grid.BindLoDNoDetail();
					glDisableVertexAttribArray(1);
					if (ballMode)
						glUniform1i(BallInfoID, 0);
					else
						glUniform1i(FlatInfoID, 0);
					chunki++;
				}
				else
				{
					glDrawElements(GL_TRIANGLES, grid.HowManyElements(), GL_UNSIGNED_INT, (void*)(0));
				}
			}
		}
		glDisableVertexAttribArray(0);

#pragma endregion
		glfwSwapBuffers(window);
		glfwPollEvents();
	}
	while (glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS &&
		glfwWindowShouldClose(window) == 0);


	glDeleteProgram(BallShader);
	glDeleteVertexArrays(1, &VertexArrayID);
	grid.CleanUp();
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
		case GLFW_KEY_0:
			chosenLoD = 0;
			break;
		case GLFW_KEY_1:
			chosenLoD = 1;
			break;
		case GLFW_KEY_2:
			chosenLoD = 2;
			break;
		case GLFW_KEY_3:
			chosenLoD = 3;
			break;
		case GLFW_KEY_F1:
			ballMode = !ballMode;
			break;
		case GLFW_KEY_TAB:
			if (ballMode)
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


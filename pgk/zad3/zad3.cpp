#include <stdio.h>
#include <stdlib.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <ctime>
#include "model.hpp"
#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
using namespace glm;
#include <common/shader.hpp>

GLfloat* CircleVertices(int howMany, float radius = 1);

float RepeatableRandom(int source)
{
	static int m = 29 * 29 * 67;
	static int a = 29 * 67 + 1;
	static int b = 139;
	static int prev = source % m;
	if (source != 0)
	{
		prev = source % m;
	}
	
	prev = (a*prev + b) % m;
	float res = prev;
	res /= m;
	return (res);
}

int main(int argc, char * argv[]) {

#pragma region parseArgs

	int m, n, k, w;
	if (argc != 5)
	{
		printf("Usage: %s [board width] [board height] [number of colors(1-6)] [number of patterns (1-3)]\n", argv[0]);
		printf("Starting using default values.\n");
		m = 5, n = 4, k = 4, w = 3;
	}
	else
	{
		m = atoi(argv[1]);
		n = atoi(argv[2]);
		k = atoi(argv[3]);
		w = atoi(argv[4]);
	}
	printf("Width = %d, height = %d, colors = %d, patterns = %d \n\n", m, n, k, w);
	printf("Use arrows to move the cursor, space to select, esc to quit\n\n");



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

	// Ensure we can capture the escape key being pressed below
	glfwSetInputMode(window, GLFW_STICKY_KEYS, GL_TRUE);

	glEnable(GL_LINE_SMOOTH);
#pragma endregion

	// Dark blue background
	glClearColor(0.0f, 0.0f, 0.4f, 0.0f);

	GLuint VertexArrayID;
	glGenVertexArrays(1, &VertexArrayID);
	glBindVertexArray(VertexArrayID);

	GLuint programID = LoadShaders("VertexShader.vertexshader", "FragmentShader.fragmentshader");
	glUseProgram(programID);

	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable(GL_BLEND);


	GLint uniformColor = glGetUniformLocation(programID, "cardColor");
	GLint uniformGradient = glGetUniformLocation(programID, "gradientWeight");
	GLint uniformTranslation = glGetUniformLocation(programID, "translation");
	GLint uniformScale = glGetUniformLocation(programID, "scale");
	GLint uniformRotation = glGetUniformLocation(programID, "rotation");




	// could do this via shader, but it's kinda easier, considering that the
	// facetted appearance of background is desired
	static const GLfloat HexBgrndData[] = {
		-1.0f,  0.0f, //t1
		-0.5f,  1.0f,
		 0.0f,  0.5f, 
		-0.5f,  1.0f, //t2
		 0.5f,  1.0f,
		 0.0f,  0.5f,
		 0.5f,  1.0f, //t3
		 1.0f,  0.0f,
		 0.0f,  0.5f,
		 0.0f,  0.5f, //t4
		 0.0f, -0.5f,
		-1.0f,  0.0f,
		 0.0f,  0.5f, //t5
		 1.0f,  0.0f, 
		 0.0f, -0.5f,	
		 1.0f,  0.0f, //t6
		 0.5f, -1.0f,
		 0.0f, -0.5f,
		 0.5f, -1.0f, //t7
		-0.5f, -1.0f,
		 0.0f, -0.5f,
		-0.5f, -1.0f, //t8
		-1.0f,  0.0f,
		 0.0f, -0.5f,
	};


	GLuint backgroundBuffer;
	glGenBuffers(1, &backgroundBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, backgroundBuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(HexBgrndData), HexBgrndData, GL_STATIC_DRAW);


	Board& board = Board(
		120,  // xgrid
		90,   // ygrid
		22,   // how far should walls be
		1.0f, // ball radius
		10,   // brick width
		3,    // brick height
		9,    // number of bricks horizontally
		6,    // number of bricks vertically
		60,   // brick wall center X 
		60,   // brick wall center Y
		15,    // paddle width
		4);  // paddle height
	board.paddlePosition.x = board.gridX / 2;
	board.paddlePosition.y = 2;
	board.ball.angularVelocity = 1;
	board.ball.position.x = 10;
	board.ball.position.y = board.bricksCenter.y;

	board.ball.position.x = board.bricksCenter.x - (board.bricksHorizontally / 2.0 ) * board.brickSize.x;
	board.ball.position.y = board.bricksCenter.y - (board.bricksVertically / 2.0 ) * board.brickSize.y;
	board.ball.position.y *= 3.0/4.0;
	board.ball.position.x *= 3.0 / 4.0;
	board.ball.velocity.x = board.ball.position.x;
	board.ball.velocity.y = board.ball.position.y;

	static const GLfloat paddleData[] =
	{
		- board.paddleSize.x / 2, - board.paddleSize.y / 2,
		  board.paddleSize.x / 2, - board.paddleSize.y / 2,
		  board.paddleSize.x / 2,   board.paddleSize.y / 2,
		- board.paddleSize.x / 2,   board.paddleSize.y / 2
	};
	GLuint paddleBuffer;
	glGenBuffers(1, &paddleBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, paddleBuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(paddleData), paddleData, GL_STATIC_DRAW);

	int ballDetail = 16;
	static const GLfloat* ballData = CircleVertices(ballDetail,board.ball.radius);
	GLuint ballBuffer;
	glGenBuffers(1, &ballBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, ballBuffer);
	glBufferData(GL_ARRAY_BUFFER, 2*(ballDetail+1)*sizeof(GLfloat), ballData, GL_STATIC_DRAW);

	static const GLfloat barData[] =
	{
		ballData[0], ballData[1],
		ballData[2], ballData[3],
		ballData[ballDetail], ballData[ballDetail + 1],
		ballData[ballDetail + 2], ballData[ballDetail + 3]
	};
	GLuint barBuffer;
	glGenBuffers(1, &barBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, barBuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(barData), barData, GL_STATIC_DRAW);

	static const GLfloat brickData[] =
	{
		-board.brickSize.x / 2 - 0.2f, -board.brickSize.y / 2 - 0.2f, //outline (0,4)
		-board.brickSize.x / 2 - 0.2f,  board.brickSize.y / 2 + 0.2f,
		 board.brickSize.x / 2 + 0.2f,  board.brickSize.y / 2 + 0.2f,
		 board.brickSize.x / 2 + 0.2f, -board.brickSize.y / 2 - 0.2f,
		-board.brickSize.x / 2 + 0.2f, -board.brickSize.y / 2 + 0.2f, //brick (4,4)
		-board.brickSize.x / 2 + 0.2f,  board.brickSize.y / 2 - 0.2f,
		 board.brickSize.x / 2 - 0.2f,  board.brickSize.y / 2 - 0.2f,
		 board.brickSize.x / 2 - 0.2f, -board.brickSize.y / 2 + 0.2f
	};
	GLuint brickBuffer;
	glGenBuffers(1, &brickBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, brickBuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(brickData), brickData, GL_STATIC_DRAW);

	static const GLfloat wallsData[] = 
	{
		 0.0f,								0.0f, //t1
		board.wallsClosing - 0.5,					0.0f,
		 0.0f,								board.gridY/2.0 - 0.5,
		 0.0f,								board.gridY, //t2 
		board.wallsClosing - 0.5,					board.gridY,
		 0.0f,								board.gridY/2.0 + 0.5,
		board.gridX,						0.0f, //t3
		board.gridX - board.wallsClosing + 0.5,	0.0f,
		board.gridX,						board.gridY / 2.0 - 0.5,
		board.gridX,						board.gridY, //t4 
		board.gridX - board.wallsClosing + 0.5,	board.gridY,
		board.gridX,						board.gridY / 2.0 + 0.5,
		0.0f,								0.0f, //t1bgr
		board.wallsClosing + 0.5,					0.0f,
		0.0f,								board.gridY / 2.0 + 0.5,
		0.0f,								board.gridY, //t2 bgr
		board.wallsClosing + 0.5,					board.gridY,
		0.0f,								board.gridY / 2.0 - 0.5,
		board.gridX,						0.0f, //t3 bgr
		board.gridX - board.wallsClosing - 0.5,		0.0f,
		board.gridX,						board.gridY / 2.0 + 0.5,
		board.gridX,						board.gridY, //t4 bgr
		board.gridX - board.wallsClosing - 0.5,	board.gridY,
		board.gridX,						board.gridY / 2.0 - 0.5,

	};
	GLuint wallsBuffer;
	glGenBuffers(1, &wallsBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, wallsBuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(wallsData), wallsData, GL_STATIC_DRAW);


	bool waitingForInput = false;
	bool pause = true;
	int bgrndRows = 32;

	double timer = glfwGetTime();
	double deltaTime = 0;
	srand(time(NULL));
	int seed = rand();
	// main loop
	do {

		deltaTime = glfwGetTime() - timer;
		timer = glfwGetTime();
		if (!pause) board.Update(deltaTime);
		glClear(GL_COLOR_BUFFER_BIT);
		glUseProgram(programID);

		//drawBackground
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, backgroundBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
		glUniform1f(uniformRotation, 0);
		glUniform2f(uniformScale, bgrndRows*1.25f, bgrndRows);
		glUniform1f(uniformGradient, 0.25f + (1+sin(timer))/8);
		for (int row = -1; row < bgrndRows*2; row++)
		{
			for (int col = -1; col < bgrndRows; col++)
			{
				glUniform2f(uniformTranslation,3.0f*col+(row%2 ? 0.0f : 1.5f), row);
				for (int i = 0; i < 8; i++)
				{
					glUniform4f(uniformColor, 0.1f + (0.5f * i) / 8, 0.1f + (0.5f * i) / 8, .8, 0.5f);
					glDrawArrays(GL_TRIANGLES, 3 * i, 3);
				}
			}
		}
		glDisableVertexAttribArray(0);

		//drawWalls
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, wallsBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);

		glUniform1f(uniformRotation, 0);
		glUniform2f(uniformScale, 120, 90);
		glUniform2f(uniformTranslation, 0, 0);
		glUniform1f(uniformGradient, 0.1);
		glUniform4f(uniformColor, 0.05, 0.05, 0.05, 0.9);
		glDrawArrays(GL_TRIANGLES, 12, 12);
		glUniform1f(uniformGradient, 0.25f + (1 + sin(timer)) / 8);
		glUniform4f(uniformColor, 0.2, 0.2, 0.6, 0.9);
		glDrawArrays(GL_TRIANGLES, 0, 12);
		glDisableVertexAttribArray(0);

		//drawBricks
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, brickBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
		glUniform1f(uniformRotation, 0);
		glUniform2f(uniformScale, 120, 90);
		RepeatableRandom(seed);
		for (int i = 0; i < board.bricksVertically; i++)
		{
			for (int j = 0; j < board.bricksHorizontally; j++)
			{
				if (!board.bricks[i*board.bricksHorizontally + j])
				{
					RepeatableRandom(0);
					RepeatableRandom(0);
					RepeatableRandom(0);
					continue;
				}
				
				glUniform2f(
					uniformTranslation,
					board.bricksCenter.x + (-board.bricksHorizontally / 2.0f + j + 0.5)*board.brickSize.x,
					board.bricksCenter.y + (- board.bricksVertically / 2.0f + i + 0.5)*board.brickSize.y);
				glUniform4f(uniformColor, 0.1, 0.1, 0.1, 1);
				glUniform1f(uniformGradient, 0.25 + (1 + sin(timer)) * 3.0/ 8.0);
				glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
				glUniform4f(uniformColor, 
					RepeatableRandom(0),
					RepeatableRandom(0),
					RepeatableRandom(0),
					1);
				glUniform1f(uniformGradient, .1);
				glDrawArrays(GL_TRIANGLE_FAN, 4, 4);
			}
		}
		glDisableVertexAttribArray(0);


		//drawPaddle
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, paddleBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
		glUniform2f(uniformScale, 120, 90);
		glUniform1f(uniformGradient, 0.2);
		glUniform4f(uniformColor, 0.1, 0.1, 0.1, 1);
		glUniform2f(uniformTranslation, board.paddlePosition.x, board.paddlePosition.y);
		glUniform1f(uniformRotation, 0);
		glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
		glDisableVertexAttribArray(0);

		//drawBall
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, ballBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
		glUniform2f(uniformScale, 120, 90);
		glUniform1f(uniformGradient, 0.8f);
		glUniform1f(uniformRotation, board.ball.rotation);
		glUniform4f(uniformColor, 0.9, 0.9, 0.9, 0.9);
		glUniform2f(uniformTranslation, board.ball.position.x, board.ball.position.y);
		glDrawArrays(GL_TRIANGLE_FAN, 0, (ballDetail + 1) * 2);
		glDisableVertexAttribArray(0);

		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, barBuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);
		glUniform1f(uniformGradient, 0.3);
		glUniform4f(uniformColor, 0.1, 0.1, 0.1, 0.6);
		glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
		glDisableVertexAttribArray(0);





		//input
		Vector2 nvel = board.paddleVelocity;
		if ((glfwGetKey(window, GLFW_KEY_SPACE)
			| glfwGetKey(window, GLFW_KEY_LEFT)
			| glfwGetKey(window, GLFW_KEY_RIGHT))
			== GLFW_RELEASE)
		{
			waitingForInput = true;
		}
		if ((glfwGetKey(window, GLFW_KEY_LEFT)
			| glfwGetKey(window, GLFW_KEY_RIGHT))
			== GLFW_RELEASE)
		{
			nvel.x = 0;
		}
		if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS && waitingForInput)
		{
			waitingForInput = false;
			pause = !pause;
		}
		if (glfwGetKey(window, GLFW_KEY_RIGHT) == GLFW_PRESS && (waitingForInput || board.paddleVelocity.x <= 0))
			
		{
			waitingForInput = false;
			nvel.x += 40;
		}
		if (glfwGetKey(window, GLFW_KEY_LEFT) == GLFW_PRESS && (waitingForInput || board.paddleVelocity.x >= 0))

		{
			waitingForInput = false;
			nvel.x -= 40;
		}
		board.paddleVelocity = nvel;
		glfwSwapBuffers(window);
		glfwPollEvents();

	}
	while (glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS &&
		glfwWindowShouldClose(window) == 0);


	glDeleteBuffers(1, &backgroundBuffer);
	glDeleteVertexArrays(1, &VertexArrayID);
	glDeleteProgram(programID);
	glfwTerminate();

	return 0;
}

GLfloat* CircleVertices(int howMany, float radius)
{
	GLfloat* res = (GLfloat*)malloc(sizeof(GLfloat)*2*howMany+2);
	float maxAngle = 2 * M_PI;
	for (int i = 0; i <= howMany; i++)
	{
		float angle = (maxAngle * i) / howMany;
		res[2 * i] = sin(angle)*radius;
		res[2 * i + 1] = cos(angle)*radius;
		printf("%f %f\n", res[2 * i], res[2 * i + 1]);
	}
	return res;
}
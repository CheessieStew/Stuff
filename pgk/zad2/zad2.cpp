#include <stdio.h>
#include <stdlib.h>
#include "model.hpp"
#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
using namespace glm;
#include <common/shader.hpp>

void drawCard(int unColor, int unGradient, bool revealed, Card & card);

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

	//create and populate the board
	Board* board;
	try
	{
		board = new Board(m, n, k, w);
	}
	catch (char* ex)
	{
		printf("Could not create a board. %s\n", ex);
		printf("Usage: %s [board width] [board height] [number of colors(1-6)] [number of patterns (1-3)]\n", argv[0]);
		system("PAUSE");
		return -1;
	}

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

	//define all the shapes
	static const GLfloat g_vertex_buffer_data[] = {
		-0.9f, -0.9f, //square (0, 4)
		0.9f, -0.9f,
		0.9f,  0.9f,
		-0.9f,  0.9f,
		-0.1f, -0.8f, //line vertical (4, 4)
		-0.1f,  0.8f,
		0.1f,  0.8f,
		0.1f, -0.8f,
		-0.8f,  0.1f, //line horizontal (8, 4)
		0.8f,  0.1f,
		0.8f, -0.1f,
		-0.8f, -0.1f,
		0.0f,  0.8f, //triangle (12, 3)
		-0.8f, -0.8f,
		0.8f, -0.8f,
		-1.0f, -1.0f, //bigger square (15, 4)
		1.0f, -1.0f,
		1.0f,  1.0f,
		-1.0f,  1.0f,

	};

	GLint uniformColor = glGetUniformLocation(programID, "cardColor");
	GLint uniformGradient = glGetUniformLocation(programID, "gradientWeight");
	GLint uniformTranslation = glGetUniformLocation(programID, "translation");
	GLint uniformScale = glGetUniformLocation(programID, "scale");

	GLuint vertexbuffer;
	glGenBuffers(1, &vertexbuffer);
	glBindBuffer(GL_ARRAY_BUFFER, vertexbuffer);
	glBufferData(GL_ARRAY_BUFFER, sizeof(g_vertex_buffer_data), g_vertex_buffer_data, GL_STATIC_DRAW);

	//setScale, just once
	glUniform2f(uniformScale, float(m), float(n));

	// currently selected card
	int cursor = n / 2 * m + m / 2;

	// currently revealed cards
	int revealed1 = -1,
		revealed2 = -1;

	// turn number
	int turn = 0;

	// number of matched pairs
	int score = 0;

	bool waitingForInput = false;

	printf("TURN %d, SCORE %d\n", turn / 3, score);

	// main loop
	do {
		if (score == m*n / 2)
		{
			printf("Starting new game.\n");
			board->reset();
			turn = 0;
			score = 0;
		}

		glClear(GL_COLOR_BUFFER_BIT);
		glUseProgram(programID);
		glEnableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, vertexbuffer);
		glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, (void*)0);

		// frame around selected card
		glUniform4f(uniformColor, 1, 1, 1, .5);
		glUniform2f(uniformTranslation, float(cursor % m), float(cursor / m));
		glDrawArrays(GL_TRIANGLE_FAN, 15, 4);

		//draw the board
		for (int i = 0; i < m*n; i++)
		{
			glUniform2f(uniformTranslation, float(i % m), float(i / m));
			drawCard(
				uniformColor,
				uniformGradient,
				i == revealed1 || i == revealed2,
				(*board)[i]);
		}

		glDisableVertexAttribArray(0);

		//input
		if ((glfwGetKey(window, GLFW_KEY_RIGHT)
			| glfwGetKey(window, GLFW_KEY_LEFT)
			| glfwGetKey(window, GLFW_KEY_UP)
			| glfwGetKey(window, GLFW_KEY_DOWN)
			| glfwGetKey(window, GLFW_KEY_SPACE))
			== GLFW_RELEASE)
		{
			waitingForInput = true;
		}
		if (waitingForInput)
		{
			if ((glfwGetKey(window, GLFW_KEY_RIGHT)
				| glfwGetKey(window, GLFW_KEY_LEFT)
				| glfwGetKey(window, GLFW_KEY_UP)
				| glfwGetKey(window, GLFW_KEY_DOWN)
				| glfwGetKey(window, GLFW_KEY_SPACE))
				!= GLFW_RELEASE)
				waitingForInput = false;

			//move cursor
			if (glfwGetKey(window, GLFW_KEY_RIGHT) == GLFW_PRESS)
				cursor = (cursor / m * m + (cursor + 1) % m);
			if (glfwGetKey(window, GLFW_KEY_LEFT) == GLFW_PRESS)
				cursor = (cursor / m * m + (m + cursor - 1) % m);
			if (glfwGetKey(window, GLFW_KEY_UP) == GLFW_PRESS)
				cursor = (cursor + m) % (m*n);
			if (glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_PRESS)
				cursor = (cursor - m + m*n) % (m*n);

			//select
			if (glfwGetKey(window, GLFW_KEY_SPACE) == GLFW_PRESS)
			{
				//remove the cards or turn them face down again
				if (revealed1 >= 0 && revealed2 >= 0)
				{
					if ((*board)[revealed1] == (*board)[revealed2])
					{
						(*board)[revealed1].removed = true;
						(*board)[revealed2].removed = true;
						score++;
					}
					revealed1 = -1;
					revealed2 = -1;
					turn++;
					printf("TURN %d, SCORE %d\n", turn / 3, score);
				}
				//reveal the selected card
				else if (!(*board)[cursor].removed && cursor != revealed1 && cursor != revealed2)
				{
					//reveal the card
					if (revealed1 == -1)
						revealed1 = cursor;
					else revealed2 = cursor;
					turn++;
				}
			}
		}

		glfwSwapBuffers(window);
		glfwPollEvents();

	}
	while (glfwGetKey(window, GLFW_KEY_ESCAPE) != GLFW_PRESS &&
		glfwWindowShouldClose(window) == 0);

	delete board;

	glDeleteBuffers(1, &vertexbuffer);
	glDeleteVertexArrays(1, &VertexArrayID);
	glDeleteProgram(programID);
	glfwTerminate();

	return 0;
}


void drawCard(int unColor, int unGradient, bool revealed, Card & card)
{
	if (card.removed)
	{
		return;
	}
	if (!revealed)
	{

		glUniform1f(unGradient, 0.25);
		glUniform4f(unColor, .5, .5, .5, .5);
		glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
		return;
	}

	glUniform1f(unGradient, 0.5);
	switch (card.color)
	{
	case 0:
		glUniform4f(unColor, 1, 0, 0, 1);
		break;
	case 1:
		glUniform4f(unColor, 0, 1, 0, 1);
		break;
	case 2:
		glUniform4f(unColor, 0, 0, 1, 1);
		break;
	case 3:
		glUniform4f(unColor, 1, 1, 0, 1);
		break;
	case 4:
		glUniform4f(unColor, 1, 0, 1, 1);
		break;
	case 5:
		glUniform4f(unColor, 0, 1, 1, 1);
		break;
	default:
		printf("smth wrong");
	}
	glDrawArrays(GL_TRIANGLE_FAN, 0, 4);
	glUniform4f(unColor, 0, 0, 0, 1);

	// the guidelines specified that patterns should be drawn with GL_LINES
	// but it looks bad
	switch (card.pattern)
	{
	case 0:
		glDrawArrays(GL_TRIANGLE_FAN, 8, 4);
		break;
	case 1:
		glDrawArrays(GL_TRIANGLE_FAN, 4, 4);
		glDrawArrays(GL_TRIANGLE_FAN, 8, 4);
		break;
	case 2:
		glDrawArrays(GL_TRIANGLE_FAN, 12, 3);
		break;
	default:
		printf("smth wrong");
	}
}

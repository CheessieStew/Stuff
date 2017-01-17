#pragma once
#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
#include <vector>


class Grid
{
	int Density;
	int MaxLoD;
	int CurrentLoD;
	GLuint vertexBuffer;
	GLuint* indexBuffers;
public:
	void BindVertices();
	void BindLoD(int LoD);
	void BindLoDNoDetail();
	int HowManyElements();
	Grid(int density, int numberOfLoDs);
	void CleanUp();
};

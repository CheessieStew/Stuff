#pragma once
#include <GL/glew.h>
#include <glfw3.h>
#include <glm/glm.hpp>
#include <cstring>
#include <stdio.h>
#include <fstream>


class TerrainChunk
{
	short* heights;
public:
	int xStart;
	int yStart;
	GLuint heightBuffer;
	TerrainChunk(const char* path);
	void CleanUp();
	void BindHeight();

	bool operator < (const TerrainChunk& chunk) const;

};



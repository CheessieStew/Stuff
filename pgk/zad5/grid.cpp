#include "grid.hpp"
using namespace glm;
using namespace std;

void Grid::BindVertices()
{
	glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, 0);
}

void Grid::BindLoD(int LoD)
{
	if (LoD > MaxLoD)
		throw "LoD too low";
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffers[LoD]);
	CurrentLoD = LoD;
}

void Grid::BindLoDNoDetail()
{
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffers[MaxLoD+1]);
	CurrentLoD = MaxLoD + 1;
}

int Grid::HowManyElements()
{
	if (CurrentLoD == MaxLoD + 1)
		return 6;
	int pow2 = (int)(pow(2, CurrentLoD) + 0.5);
	int res = (Density-1) / pow2;
	return res * res * 6;
}

Grid::Grid(int density, int numberOfLods)
{
	int resolution = density - 1;
	Density = density;
	MaxLoD = numberOfLods - 1;
	vector<vec2> vertices(density * density);
	for (int x = 0; x < density; x++)
	{
		for (int y = 0; y < density; y++)
		{
			vertices[y * density + x] = vec2((x / (float)resolution), (y / (float)resolution));
		}
	}
	glGenBuffers(1, &vertexBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
	glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(glm::vec2), &vertices[0], GL_STATIC_DRAW);

	indexBuffers = new GLuint[numberOfLods + 1];
	int pow2 = 1;
	for (int a = 0; a < numberOfLods; a++)
	{
		int curRes = resolution / pow2;
		vector<int> indices(curRes * curRes * 6);
		for (int i = 0; i < curRes * curRes; i++)
		{

			indices[6 * i] = ((pow2 * i) % 1200) + ((pow2 * i) / 1200) * 1201 * pow2;
			indices[6 * i + 1] = indices[6 * i] + pow2;

			indices[6 * i + 2] = indices[6 * i] + 1201 * pow2;
			indices[6 * i + 3] = indices[6 * i] + 1201 * pow2;
			indices[6 * i + 4] = indices[6 * i] + pow2;
			indices[6 * i + 5] = indices[6 * i] + (1201 + 1) * pow2;
		}
		pow2 *= 2;
		glGenBuffers(1, &indexBuffers[a]);
		glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffers[a]);
		glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(int), &indices[0], GL_STATIC_DRAW);
	}

	vector<int> indices(6);
	indices[0] = 0;
	indices[1] = resolution;
	indices[2] = resolution * density;
	indices[3] = resolution * density;
	indices[4] = resolution;
	indices[5] = resolution * density + resolution;

	glGenBuffers(1, &indexBuffers[numberOfLods]);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, indexBuffers[numberOfLods]);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(int), &indices[0], GL_STATIC_DRAW);

}

void Grid::CleanUp()
{
	glDeleteBuffers(1, &vertexBuffer);
	for (int i = 0; i <= MaxLoD + 1; i++)
	{
		glDeleteBuffers(1, &indexBuffers[i]);
	}
}

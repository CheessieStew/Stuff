
#include "terrain.hpp"

using namespace glm;


short* loadHGT(const char* name)
{
	std::ifstream file(name, std::ios::in | std::ios::binary);
	if (!file)
	{
		fprintf(stderr, "File %s could not be opened.\n", name);
		throw "open error";
	}
	unsigned char buffer[2];
	int i = 0;
	short *height = new short[1201 * 1201];
	while (!(file.eof()))
	{

		if (file.read((char*)buffer, sizeof(buffer)))
		{
			height[1201 * (1200 - i / 1201) + (i % 1201)] = (buffer[0] << 8) | buffer[1];
			i++;

		}
		else if (file.bad())
		{
			fprintf(stderr, "Read error after %d chars.\n", i);
			if (file.bad())
				fprintf(stderr, "bad\n", i);
			if (file.fail())
				fprintf(stderr, "fail\n", i);
			throw "read error";
		}
	}
	file.close();
	return height;
}


TerrainChunk::TerrainChunk(const char* path)
{
	printf("a\n");
	heights = loadHGT(path);
	if (heights == NULL)
		throw "loadHGT did not succeed";
	int w, ww, s, t;
	for (int i = 0; i < 1201; i++)
	{
		for (int j = 0; j < 1201; j++)
		{
			w = i * 1201 + j;
			if (heights[w] < -500 || heights[w] > 9000)
			{
				t = 0;
				s = 0;
				for (int ii = i - 1; ii <= i + 1; ii++)
					for (int jj = j - 1; jj <= j + 1; jj++)
					{
						if (jj == j && ii == i)
							continue;
						ww = (ii)* 1201 + jj;
						if (ww >= 0 && ww < 1201 * 1201 &&
							heights[ww] >= -500 && heights[ww] <= 9000)
						{
							s += heights[ww];
							t++;
						}
					}
				if (t > 0)
					heights[w] = s / t;
				else printf("couldn't fill in a gap");
			}
		}
	}
	char ns;
	char we;
	const char* filename = strrchr(path, '/');
	if (filename == nullptr)
		filename = path;
	else
		filename++;

	sscanf(filename, "%c%d%c%d", &ns, &yStart, &we, &xStart);
	if (ns == 'S')
		yStart *= -1;
	else if (ns != 'N')
		throw "hgt name incorrect";
	if (we == 'W')
		xStart *= -1;
	else if (we != 'E')
		throw "hgt name incorrect";
	printf("Got %c%d%c%d from %s\n", ns, yStart, we, xStart, filename);

	glGenBuffers(1, &heightBuffer);

	glBindBuffer(GL_ARRAY_BUFFER, heightBuffer);

	glBufferData(GL_ARRAY_BUFFER, 1201 * 1201 * sizeof(short), &heights[0], GL_STATIC_DRAW);
}

void TerrainChunk::BindHeight()
{
	// 2nd attribute buffer : UVs
	glBindBuffer(GL_ARRAY_BUFFER, heightBuffer);
	glVertexAttribPointer(1, 1, GL_SHORT, GL_FALSE, 0, 0);
}
bool TerrainChunk::operator<(const TerrainChunk & chunk) const
{
	return (xStart < chunk.xStart) || (xStart == chunk.xStart && yStart < chunk.yStart);
}

void TerrainChunk::CleanUp()
{
	glDeleteBuffers(1, &heightBuffer);
}

#include "utilities.hpp"
#include <math.h>



float PointLineDistance(float a, float b, float c, float x, float y)
{
	return fabs(a * x + b * y + c) / sqrt(a * a + b * b);
}

float PointLineDistance(float a, float b, float c, Vector2 v)
{
	return fabs(a * v.x + b * v.y + c) / sqrt(a * a + b * b);
}

float PointLineDistance(Line line, float x, float y)
{
	return fabs(line.a * x + line.b * y + line.c) / sqrt(line.a * line.a + line.b * line.b);
}

float PointLineDistance(Line line, Vector2 v)
{
	return fabs(line.a * v.x + line.b * v.y + line.c) / sqrt(line.a * line.a + line.b * line.b);
}

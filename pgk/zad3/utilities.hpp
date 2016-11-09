#pragma once
struct Line
{
	float a;
	float b;
	float c;
};

struct Vector2
{
	float x;
	float y;
};

float PointLineDistance(float a, float b, float c, float x, float y);

float PointLineDistance(float a, float b, float c, Vector2 v);

float PointLineDistance(Line line, float x, float y);

float PointLineDistance(Line line, Vector2 v);
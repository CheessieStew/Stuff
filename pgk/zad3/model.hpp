#pragma once
#include "utilities.hpp"
using namespace std;


class Ball
{
public:
	Vector2 position;
	float radius;
	Vector2 velocity;
	float rotation;
	float angularVelocity;
	Ball(float br, float xp, float yp);
	~Ball();

private:
	Ball();
};

class Board
{
public:
	Vector2 brickSize;
	int bricksHorizontally;
	int bricksVertically;
	Vector2 bricksCenter;

	Vector2 paddleSize;
	Vector2 paddleVelocity;
	Vector2 paddlePosition;

	bool* bricks;
	Ball& ball;
	Line* lines;
	int gridX;
	int gridY;
	int wallsClosing;
	Board(int gX, int gY, int wc, float br, float bw, float bh, int hor, int ver, float centerX, float centerY, float pt, float pw);
	~Board();
	void Update(double time);
private:
	Board();
	bool CheckBrick(int x, int y);
	bool BallCollision(Vector2* shape, int shapeLen, Vector2 pos, bool checkOnly, bool round);
};


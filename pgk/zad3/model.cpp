#include <stdlib.h>
#include <algorithm>
#define _USE_MATH_DEFINES
#include<math.h>
#include "utilities.hpp"
#include "model.hpp"
using namespace std;




// line equations for the walls:
// gridY/(2*wc) * x + y - gridY/2 = 0
// -gridY/(2*wc) * x + y - gridY/2 = 0
// gridY/(2*wc) * (x-gridX) + y - gridY/2 = 0
// -gridY/(2*wc) * (x-gridX) + y - gridY/2 = 0
// 0 * x + y - gridY = 0
// 0 * x + y + 0 = 0 : for bottom

Board::Board(int gX, int gY, int wc, float br, float bw, float bh, int hor, int ver, float centerX, float centerY, float pw, float ph) :
	ball(*(new Ball(br, gX / 2, gY / 2))),
	wallsClosing(wc),
	gridX(gX),
	gridY(gY),
	bricksHorizontally(hor),
	bricksVertically(ver)
{
	bricksCenter.x = centerX;
	bricksCenter.y = centerY;
	brickSize.x = bw;
	brickSize.y = bh;
	paddleSize.x = pw;
	paddleSize.y = ph;
	lines = (Line*)malloc(sizeof(Line) * 6);
	bricks = (bool*)malloc(sizeof(bool)*ver*hor);
	for (int i = 0; i < ver; i++)
		for (int j = 0; j < hor; j++)
			bricks[i*hor + j] = true;
	lines[0].a = (float)gridY / (2 * (float)wallsClosing);
	lines[0].b = 1;
	lines[0].c = -(float)gridY / 2;

	lines[1].a = -(float)gridY / (2 * wallsClosing);
	lines[1].b = 1;
	lines[1].c = -(float)gridY / 2;

	lines[2].a = (float)gridY / (2 * wallsClosing);
	lines[2].b = 1;
	lines[2].c = -(float)gridY / 2 - (float)gridY * (float)gridX / (2 * (float)wallsClosing);

	lines[3].a = -(float)gridY / (2 * (float)wallsClosing);
	lines[3].b = 1;
	lines[3].c = -(float)gridY / 2 + (float)gridY * (float)gridX / (2 * (float)wallsClosing);

	lines[4].a = 0;
	lines[4].b = 1;
	lines[4].c = -(float)gridY;

	lines[5].a = 0;
	lines[5].b = 1;
	lines[5].c = 0;

}

Ball::Ball(float br, float xp, float yp) :
	radius(br),
	angularVelocity(0),
	rotation(0)
{
	position.x = xp;
	position.y = yp;
	velocity.x = 0;
	velocity.y = 0;
}

bool Board::CheckBrick(int x, int y)
{
	return (x >= 0 && x < bricksHorizontally
			&& y >= 0 && y < bricksVertically
			&& bricks[y*bricksHorizontally + x]);
}

/*
void Board::Update(double time)
{
	//where the ball would be if no colission occurs
	float bnxp = ball.xpos + ball.xVelocity * time;
	float bnyp = ball.ypos + ball.yVelocity * time;
	bool clear = true;

	//walls
	float distances[6];
	for (int i = 0; i < 6; i++)
	{
		distances[i] = PointLineDistance(lines[i], bnxp, bnyp);
		if (distances[i] <= ball.radius)
		{
			clear = false;
			Line mirror;
			float touchX = lines[i].b * (lines[i].b * ball.xpos - lines[i].b * ball.ypos) - lines[i].a * lines[i].c;
			touchX /= sqrt(lines[i].a * lines[i].a + lines[i].b * lines[i].b);
			float touchY = lines[i].a * (- lines[i].b * ball.xpos + lines[i].a * ball.ypos) - lines[i].b * lines[i].c;
			touchY /= sqrt(lines[i].a * lines[i].a + lines[i].b * lines[i].b);

			if (lines[i].b == 0)
			{
				ball.xVelocity = -ball.xVelocity;
				ball.angularVelocity = (((float)rand()) / RAND_MAX) * 4 - 2;
				continue;
			}
			if (lines[i].a == 0)
			{
				ball.yVelocity = -ball.yVelocity;
				ball.angularVelocity = (((float)rand()) / RAND_MAX) * 4 - 2;
				continue;
			}
			// mirror: y = a*x
			float a = -lines[i].a / lines[i].b;
			float nvelX = (1 - a*a) / (1 + a*a) * ball.xVelocity;
			nvelX += 2 * a / (1 + a*a) * ball.yVelocity;
			float nvelY = 2 * a / (1 + a * a) * ball.xVelocity;
			nvelY += -(1 - a * a) / (1 + a * a) * ball.yVelocity;
			ball.xVelocity = nvelX;
			ball.yVelocity = nvelY;
			ball.angularVelocity = (((float)rand()) / RAND_MAX) * 4 - 2;
		}
	}
	
	bnxp = ball.xpos + ball.xVelocity * time;
	bnyp = ball.ypos + ball.yVelocity * time;

	int xBrick = (bnxp - (bricksCenterX - bricksHorizontally / 2.0 * brickWidth )) + 1000*brickWidth;
	xBrick /= (int)brickWidth;
	int yBrick = (bnyp - (bricksCenterY - bricksVertically   / 2.0 * brickHeight)) + 1000*brickHeight;
	yBrick /= (int)brickHeight;
	xBrick -= 1000;
	yBrick -= 1000;
	int destroyX = xBrick;
	int destroyY = yBrick;

	Line lineTop;
	lineTop.a = 0;
	lineTop.b = 1;
	lineTop.c = -(bricksCenterY - (bricksVertically / 2.0 - yBrick) * brickHeight) - brickHeight + 0.1;
	Line lineBottom;
	lineBottom.a = 0;
	lineBottom.b = 1;
	lineBottom.c = -(bricksCenterY - (bricksVertically / 2.0 - yBrick) * brickHeight) - 0.1;
	Line LineRight;
	LineRight.a = 1;
	LineRight.b = 0;
	LineRight.c = -(bricksCenterX - (bricksHorizontally / 2.0 - xBrick) * brickWidth) - brickWidth + 0.1;
	Line LineLeft;
	LineLeft.a = 1;
	LineLeft.b = 0;
	LineLeft.c = -(bricksCenterX - (bricksHorizontally / 2.0 - xBrick) * brickWidth) - 0.1;
	bool top = false, bottom = false, left = false, right = false;

		if (PointLineDistance(lineTop, bnxp, bnyp) <= ball.radius 
			&& yBrick + 1 < bricksVertically 
			&& yBrick + 1 >= 0)
		{
			destroyY = yBrick + 1;
			top = true;
		}

		if (PointLineDistance(lineBottom, bnxp, bnyp) <= ball.radius 
			&& yBrick - 1 < bricksVertically 
			&& yBrick - 1 >= 0)
		{
			destroyY = yBrick - 1;
			bottom = true;
		}

		if (PointLineDistance(LineLeft, bnxp, bnyp) <= ball.radius 
			&& xBrick -1 < bricksHorizontally
			&& xBrick - 1 >= 0)
			
		{
			destroyX = xBrick - 1;
			left = true;
		}

		if (PointLineDistance(LineRight, bnxp, bnyp) <= ball.radius 
			&& xBrick + 1 < bricksHorizontally
			&& xBrick + 1 >= 0)
		{
			destroyX = xBrick + 1;
			right = true;
		}

	bool boom = false;
	if (CheckBrick(destroyX,destroyY))
	{
		if (!boom && (bottom && right || top && left))
		{
			printf("(mby corner1, org %d %d)", destroyX, destroyY);
			if (top)
			{
				if (CheckBrick(destroyX + 1, destroyY))
				{
					destroyX += 1;
					left = false;
				}
				else if (CheckBrick(destroyX, destroyY - 1))
				{
					destroyY -= 1;
					top = false;
				}
			}
			else if (bottom)
			{
				if (CheckBrick(destroyX - 1, destroyY))
				{
					destroyX -= 1;
					right = false;
				}
				else if (CheckBrick(destroyX, destroyY + 1))
				{
					destroyY += 1;
					bottom = false;
				}
			}
			if (bottom && right || top && left)
			{
				boom = true;
				printf("corner1: ");
				float temp = ball.xVelocity;
				ball.xVelocity = ball.yVelocity;
				ball.yVelocity = temp;
			}

		}
		if (!boom && (bottom && left || top && right))
		{
			printf("(mby corner2, org %d %d)", destroyX, destroyY);
			if (top)
			{
				if (CheckBrick(destroyX - 1, destroyY))
				{
					destroyX -= 1;
					right = false;
				}
				else if (CheckBrick(destroyX, destroyY - 1))
				{
					destroyY -= 1;
					top = false;
				}
			}
			else if (bottom)
			{
				if (CheckBrick(destroyX + 1, destroyY))
				{
					destroyX += 1;
					left = false;
				}
				else if (CheckBrick(destroyX, destroyY + 1))
				{
					destroyY += 1;
					bottom = false;
				}
			}
			if (bottom && left || top && right)
			{
				boom = true;
				printf("corner2: ");
				float temp = ball.xVelocity;
				ball.xVelocity = -ball.yVelocity;
				ball.yVelocity = -temp;
			}
		}
		if (!boom && (top || bottom))
		{
			boom = true;
			printf("side1: ");
			ball.yVelocity *= -1;
		}
		if (!boom && (left || right))
		{
			boom = true;
			printf("side2: ");
			ball.xVelocity *= -1;
		}
		if (!boom)
		{
			// apparently we entered a brick, but no collision was detected - should not look too weird
			// if we hurry up and fix it here
			printf("lol! ");
			lineTop.c = -(bricksCenterY - (bricksVertically / 2.0 - destroyY) * brickHeight) - brickHeight + 0.1;
			lineBottom.c = -(bricksCenterY - (bricksVertically / 2.0 - destroyY) * brickHeight) - 0.1;
			LineRight.c = -(bricksCenterX - (bricksHorizontally / 2.0 - destroyX) * brickWidth) - brickWidth + 0.1;
			LineLeft.c = -(bricksCenterX - (bricksHorizontally / 2.0 - destroyX) * brickWidth) - 0.1;
			if (PointLineDistance(lineTop, bnxp, bnyp) <= ball.radius
				&& yBrick + 1 < bricksVertically
				&& yBrick + 1 >= 0)
			{
				destroyY = yBrick + 1;
				top = true;
			}

			if (PointLineDistance(lineBottom, bnxp, bnyp) <= ball.radius
				&& yBrick - 1 < bricksVertically
				&& yBrick - 1 >= 0)
			{
				destroyY = yBrick - 1;
				bottom = true;
			}

			if (PointLineDistance(LineLeft, bnxp, bnyp) <= ball.radius
				&& xBrick - 1 < bricksHorizontally
				&& xBrick - 1 >= 0)

			{
				destroyX = xBrick - 1;
				left = true;
			}

			if (PointLineDistance(LineRight, bnxp, bnyp) <= ball.radius
				&& xBrick + 1 < bricksHorizontally
				&& xBrick + 1 >= 0)
			{
				destroyX = xBrick + 1;
				right = true;
			}
			if (!boom && (bottom && right || top && left))
			{
				printf("(mby corner1, org %d %d)", destroyX, destroyY);
				if (top)
				{
					if (CheckBrick(destroyX + 1, destroyY))
					{
						destroyX += 1;
						left = false;
					}
					else if (CheckBrick(destroyX, destroyY - 1))
					{
						destroyY -= 1;
						top = false;
					}
				}
				else if (bottom)
				{
					if (CheckBrick(destroyX - 1, destroyY))
					{
						destroyX -= 1;
						right = false;
					}
					else if (CheckBrick(destroyX, destroyY + 1))
					{
						destroyY += 1;
						bottom = false;
					}
				}
				if (bottom && right || top && left)
				{
					boom = true;
					printf("corner1: ");
					float temp = ball.xVelocity;
					ball.xVelocity = ball.yVelocity;
					ball.yVelocity = temp;
				}

			}
			if (!boom && (bottom && left || top && right))
			{
				printf("(mby corner2, org %d %d)", destroyX, destroyY);
				if (top)
				{
					if (CheckBrick(destroyX - 1, destroyY))
					{
						destroyX -= 1;
						right = false;
					}
					else if (CheckBrick(destroyX, destroyY - 1))
					{
						destroyY -= 1;
						top = false;
					}
				}
				else if (bottom)
				{
					if (CheckBrick(destroyX + 1, destroyY))
					{
						destroyX += 1;
						left = false;
					}
					else if (CheckBrick(destroyX, destroyY + 1))
					{
						destroyY += 1;
						bottom = false;
					}
				}
				if (bottom && left || top && right)
				{
					boom = true;
					printf("corner2: ");
					float temp = ball.xVelocity;
					ball.xVelocity = -ball.yVelocity;
					ball.yVelocity = -temp;
				}
			}
			if (!boom && (top || bottom))
			{
				boom = true;
				printf("side1: ");
				ball.yVelocity *= -1;
			}
			if (!boom && (left || right))
			{
				boom = true;
				printf("side2: ");
				ball.xVelocity *= -1;
			}

		}
		clear = false;
		bricks[destroyY * bricksHorizontally + destroyX] = false;
		printf("boom of (%d,%d) from (%d, %d) (top %d, bot %d, left %d, right %d)\n", destroyX, destroyY, xBrick, yBrick, top, bottom, left, right);

	}
	
	bnxp = ball.xpos + ball.xVelocity * time;
	bnyp = ball.ypos + ball.yVelocity * time;
	if (ball.ypos - ball.radius <= paddleTop)
	{
		ball.yVelocity *= -1;
	}

	if (!clear)
	{
		Update(time);
	}
	else
	{
		ball.xpos += ball.xVelocity * time;
		ball.ypos += ball.yVelocity * time;
		ball.rotation += ball.angularVelocity * time;
		if (ball.rotation > 2 * M_PI)
			ball.rotation -= 2 * M_PI;
		if (ball.rotation < 2 * M_PI)
			ball.rotation += 2 * M_PI;
		paddleX += paddleVelocity * time;
	}
}
*/


bool Board::BallCollision(Vector2* shape, int shapeLen, Vector2 pos, bool checkOnly = false, bool round = false)
{
	int hits = 0;
	Line mirror1;
	mirror1.a = 0;
	mirror1.b = 0;
	mirror1.c = 0;
	float bot1, top1, left1, right1, bot2, top2, left2, right2;
	Line mirror2;
	mirror2.a = 0;
	mirror2.b = 0;
	mirror2.c = 0;
	for (int i = 0; i < shapeLen; i++)
	{
		Line line;
		float dx = shape[i].x - shape[(i + 1) % shapeLen].x;
		float dy = shape[i].y - shape[(i + 1) % shapeLen].y;
		if (dx == 0)
		{
			line.a = 1;
			line.b = 0;
			line.c = -shape[i].x;
		}
		else
		{
			line.a = -dy;
			line.b = dx;
			line.c = -line.a * shape[i].x - line.b * shape[i].y;
		}
		float touchX = line.b * (line.b * pos.x - line.a * pos.y) - line.a * line.c;
		touchX /= line.a * line.a + line.b * line.b;
		float touchY = line.a * (-line.b * pos.x + line.a * pos.y) - line.b * line.c;
		touchY /= line.a * line.a + line.b * line.b;

		float distance = PointLineDistance(line, pos);
		//printf("distance %f, touch %f %f ||", distance,touchX, touchY);
		float bot = min(shape[i].y, shape[(i + 1) % shapeLen].y);
		float top = max(shape[i].y, shape[(i + 1) % shapeLen].y);
		float left = min(shape[i].x, shape[(i + 1) % shapeLen].x);
		float right = max(shape[i].x, shape[(i + 1) % shapeLen].x);
		if (distance <= ball.radius
			&& touchX >= left && touchX <= right
			&& touchY >= bot && touchY <= top)
		{
			hits++;
			if (mirror1.a != 0 || mirror1.b != 0 || mirror1.c != 0)
			{
				mirror2 = line;
				top2 = top;
				bot2 = bot;
				left2 = left;
				right2 = right;
			}
			else
			{
				mirror1 = line;
				top1 = top;
				bot1 = bot;
				left1 = left;
				right1 = right;
			}
		}
	}
	if (hits == 0)
	{
		return false;
	}
	if (hits > 2) throw "What the heck, hits > 2";
	
	if (checkOnly)
		return true;
	

	Vector2 mvec;
	mvec.x = mirror1.b;
	mvec.y = -mirror1.a;
	if (hits == 2 && !(mirror1.a == mirror2.a && mirror1.b == mirror2.b))
	{
		printf("corner\n");
		Vector2 crossPoint;
		if (mirror2.a != 0)
		{
			crossPoint.y = mirror2.c * mirror1.a / mirror2.a - mirror1.c;
			crossPoint.y /= mirror1.b - mirror2.b * mirror1.a / mirror2.a;
			crossPoint.x = (-mirror2.b * crossPoint.y - mirror2.c) / mirror2.a;
		}
		else
		{			
			crossPoint.y = -mirror2.c / mirror2.b;
			crossPoint.x = -mirror1.b *crossPoint.y / mirror1.a;
		}
		Line mirror;
		mirror.a = mirror1.a * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
		mirror.a -= mirror2.a * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
		mirror.b = mirror1.b * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
		mirror.b -= mirror2.b * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
		mirror.c = mirror1.c * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
		mirror.c -= mirror2.c * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
		Vector2 check1;
		Vector2 check2;

		if (mirror.b != 0)
		{
			check1.x = crossPoint.x + 0.1;
			check1.y = -mirror.a * check1.x - mirror.c;
			check1.y /= mirror.b;
			check2.x = crossPoint.x - 0.1;
			check2.y = -mirror.a * check2.x - mirror.c;
			check2.y /= mirror.b;
		}
		else
		{
			check1.x = -mirror.c / mirror.a;
			check1.y = crossPoint.y + 0.1;
			check1.x = -mirror.c / mirror.a;
			check2.y = crossPoint.y - 0.1;
		}
		bool check = check1.x > left1 && check1.x < right1 && check1.y > bot1 && check1.y < top1
			&& check2.x > left2 && check2.x < right2 && check2.y > bot2 && check2.y < top2;
		 check = check || check1.x > left2 && check1.x < right2 && check1.y > bot2 && check1.y < top2
			&& check2.x > left1 && check2.x < right1 && check2.y > bot1 && check2.y < top1;
		if (check)
		{
			mirror.a = mirror1.a * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
			mirror.a += mirror2.a * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
			mirror.b = mirror1.b * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
			mirror.b += mirror2.b * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
			mirror.c = mirror1.c * sqrt(mirror2.a*mirror2.a + mirror2.b * mirror2.b);
			mirror.c += mirror2.c * sqrt(mirror1.a*mirror1.a + mirror1.b * mirror1.b);
		}
		mvec.x = mirror.b;
		mvec.y = -mirror.a;
	}
	//c does not matter, mirror should go through (0,0)
	float velocityDotMirror = mvec.x * ball.velocity.x + mvec.y * ball.velocity.y;
	float mirrorDotMirror = mvec.x * mvec.x + mvec.y * mvec.y;
	Vector2 nVel;
	nVel.x = 2 * velocityDotMirror / mirrorDotMirror * mvec.x - ball.velocity.x;
	nVel.y = 2 * velocityDotMirror / mirrorDotMirror * mvec.y - ball.velocity.y;
	ball.velocity = nVel;
	ball.angularVelocity = rand() * 4.0 / RAND_MAX - 2;
	return true;
}

void Board::Update(double time)
{
	//where the ball would be if no colission occurs
	Vector2 bnp = ball.position;
	bnp.x += ball.velocity.x * time;
	bnp.y += ball.velocity.y * time;
	bool clear = true;
	//atm we have only squares and triangles
	Vector2* shape = new Vector2[4];
	
	if (bnp.x < 0.5*gridX && bnp.y < 0.6*gridY)
	{
		bnp = ball.position;
		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;

		shape[0].x = 0; shape[0].y = 0;
		shape[1].x = wallsClosing; shape[1].y = 0;
		shape[2].x = 0; shape[2].y = gridY / 2.0;
		BallCollision(shape, 3, bnp);

		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;
	}
	if (bnp.x < 0.5*gridX && bnp.y > 0.4*gridY)
	{
		bnp = ball.position;
		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;

		shape[0].x = 0; shape[0].y = gridY;
		shape[1].x = wallsClosing; shape[1].y = gridY;
		shape[2].x = 0; shape[2].y = gridY / 2.0;
		BallCollision(shape, 3, bnp);
	}
	if (bnp.x > 0.5*gridX && bnp.y < 0.6*gridY)
	{
		bnp = ball.position;
		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;

		shape[0].x = gridX; shape[0].y = 0;
		shape[1].x = gridX - wallsClosing; shape[1].y = 0;
		shape[2].x = gridX; shape[2].y = gridY / 2.0;
		BallCollision(shape, 3, bnp);
	}
	if (bnp.x > 0.5*gridX && bnp.y > 0.4*gridY)
	{
		bnp = ball.position;
		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;

		shape[0].x = gridX; shape[0].y = gridY;
		shape[1].x = gridX - wallsClosing; shape[1].y = gridY;
		shape[2].x = gridX; shape[2].y = gridY / 2.0;
		BallCollision(shape, 3, bnp);
	}
	if (bnp.y > 0.8*gridY)
	{
		bnp = ball.position;
		bnp.x += ball.velocity.x * time;
		bnp.y += ball.velocity.y * time;

		shape[0].x = 0; shape[0].y = gridY;
		shape[1].x = gridX; shape[1].y = gridY;
		shape[2].x = gridX; shape[2].y = gridY + 10;
		shape[3].x = 0; shape[3].y = gridY + 10;
		BallCollision(shape, 4, bnp);

	}
	//if (bnp.y < 0.2*gridY)
	//{
	//	bnp = ball.position;
	//	bnp.x += ball.velocity.x * time;
	//	bnp.y += ball.velocity.y * time;
	//
	//	shape[0].x = 0; shape[0].y = 0;
	//	shape[1].x = gridX; shape[1].y = 0;
	//	shape[2].x = gridX; shape[2].y = - 10;
	//	shape[3].x = 0; shape[3].y = - 10;
	//	BallCollision(shape, 4, bnp);
	//
	//}
	bnp = ball.position;
	bnp.x += ball.velocity.x * time;
	bnp.y += ball.velocity.y * time;

	int xBrick = (bnp.x - (bricksCenter.x - bricksHorizontally / 2.0 * brickSize.x)) + 1000 * brickSize.x;
	xBrick /= (int)brickSize.x;
	xBrick -= 1000;
	int yBrick = (bnp.y - (bricksCenter.y - bricksVertically / 2.0 * brickSize.y)) + 1000 * brickSize.y;
	yBrick /= (int)brickSize.y;
	yBrick -= 1000;
	bool collisionOccured = false;
	for (int corner = 0; corner <= 1; corner++)
	{
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (!corner && i * j != 0 || corner && i * j == 0)
					continue; //no corner collision can occur until we've covered the plain ones
				if (!CheckBrick(xBrick + i, yBrick + j))
					continue;
				shape[0].x = bricksCenter.x - (bricksHorizontally / 2.0 - xBrick - i) * brickSize.x;
				shape[0].y = bricksCenter.y - (bricksVertically / 2.0 - yBrick - j) * brickSize.y;
				shape[1].x = bricksCenter.x - (bricksHorizontally / 2.0 - xBrick - i) * brickSize.x + brickSize.x;
				shape[1].y = bricksCenter.y - (bricksVertically / 2.0 - yBrick - j) * brickSize.y;
				shape[2].x = bricksCenter.x - (bricksHorizontally / 2.0 - xBrick - i) * brickSize.x + brickSize.x;
				shape[2].y = bricksCenter.y - (bricksVertically / 2.0 - yBrick - j) * brickSize.y + brickSize.y;
				shape[3].x = bricksCenter.x - (bricksHorizontally / 2.0 - xBrick - i) * brickSize.x;
				shape[3].y = bricksCenter.y - (bricksVertically / 2.0 - yBrick - j) * brickSize.y + brickSize.y;
				//for each alive brick next to the one we're considering,
				//expand the collider to contain it so no weird corner effects happen
				printf("expanding?\n");
				if (CheckBrick(xBrick - i, yBrick - j - 1))
				{
					printf("expanding %d %d bot\n", xBrick - i, yBrick - j);
					shape[0].y -= brickSize.y;
					shape[1].y -= brickSize.y;
				}
				if (CheckBrick(xBrick - i, yBrick - j + 1))
				{
					printf("expanding %d %d top\n", xBrick - i, yBrick - j);
					shape[2].y += brickSize.y;
					shape[3].y += brickSize.y;
				}
				if (CheckBrick(xBrick - i - 1, yBrick - j))
				{
					printf("expanding %d %d left\n", xBrick - i, yBrick - j);
					shape[0].x -= brickSize.x;
					shape[3].x -= brickSize.x;
				}
				if (CheckBrick(xBrick - i + 1, yBrick - j))
				{
					printf("expanding %d %d right\n", xBrick - i, yBrick - j);
					shape[1].x += brickSize.x;
					shape[2].x += brickSize.x;
				}
				if (BallCollision(shape, 4, bnp, corner && collisionOccured))
					//if a plain collision occured, we consider the corner one as an extra
					//brick to destroy only, we DON'T want to bounce
				{
					collisionOccured = true;
					bricks[(yBrick + j)*bricksHorizontally + xBrick + i] = false;
					printf("Collision with %d %d\n", xBrick + i, yBrick + j);
				}
			}
		}
	}

	bnp = ball.position;
	bnp.x += ball.velocity.x * time;
	bnp.y += ball.velocity.y * time;

	shape[0].y = paddlePosition.y - paddleSize.y / 2;
	shape[1].x = paddlePosition.x + paddleSize.x / 2;
	shape[1].y = paddlePosition.y - paddleSize.y / 2;
	shape[2].x = paddlePosition.x + paddleSize.x / 2;
	shape[2].y = paddlePosition.y + paddleSize.y / 2;
	shape[0].x = paddlePosition.x - paddleSize.x / 2;
	shape[3].x = paddlePosition.x - paddleSize.x / 2;
	shape[3].y = paddlePosition.y + paddleSize.y / 2;
	BallCollision(shape, 4, bnp);

	bnp = ball.position;
	bnp.x += ball.velocity.x * time;
	bnp.y += ball.velocity.y * time;

	ball.position = bnp;
	ball.rotation += ball.angularVelocity * time;
	paddlePosition.x += paddleVelocity.x * time;
}






Board::Board() :
	ball(*(new Ball(1, 60, 45))),
	wallsClosing(30),
	gridX(120),
	gridY(90)
{
}
Board::~Board()
{
}

Ball::Ball()
{
}

Ball::~Ball()
{
}
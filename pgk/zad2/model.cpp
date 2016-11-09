#include <stdlib.h>
#include <string>
#include <sstream>
#include <stdio.h>
#include <algorithm>
#include "model.hpp"
using namespace std;


Card::Card()
	{
	}

bool Card::operator ==(const Card &c)
{
	return (this->color == c.color && this->pattern == c.pattern);
}

void Board::reset()
{
	int i = 0;
	int ik = 0;
	int iw = 0;
	while (i < sizeX*sizeY)
	{
		cards[i].removed = false;
		cards[i].color = ik;
		cards[i].pattern = iw;
		i++;
		if (i % 2 == 0)
		{
			ik++;
			if (ik == colors)
			{
				ik = 0;
				iw++;
				if (iw == patterns)
					iw = 0;
			}
		}
	}
	random_shuffle(&cards[0], &cards[sizeX * sizeY - 1]);
}

Board::Board(int m, int n, int k, int w)
	{
		char*  up = "Number of cards on board (width*height) is not even";
		if (m*n % 2 != 0)
			throw up;
		up = "Number of patterns needs to be in range of 1 to 6";
		if (w > 3 || w < 1)
			throw up;
		up = "Number of colors needs to be in range of 1 to 6";
		if (k > 6 || k < 1)
			throw up;

		sizeX = m;
		sizeY = n;
		colors = k;
		patterns = w;

		cards = new Card[m*n];
		reset();
	}

Card & Board::operator[](int i)
{
	return cards[i];
}

Board::~Board()
	{
		delete[] cards;
	}

Board::Board()
	{
	}





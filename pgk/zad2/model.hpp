#include <string>
using namespace std;
class Card
{
private:

public:
	int pattern;
	int color;
	bool removed;
	Card();
	bool operator ==(const Card &c);
};

class Board
{
private:
	int sizeX;
	int sizeY;
	int colors;
	int patterns;
	Card* cards;

public:
	Board(int m, int n, int k, int w);
	Card& operator [](int i);
	void reset();
	~Board();

private:
	Board();
};



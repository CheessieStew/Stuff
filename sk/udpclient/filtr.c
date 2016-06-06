#include <stdio.h>
int main()
{
	int a=getc(stdin);
	while (a!=EOF)
	{
		putc(a%95 + 32,stdout);
		a=getc(stdin);
	}
}

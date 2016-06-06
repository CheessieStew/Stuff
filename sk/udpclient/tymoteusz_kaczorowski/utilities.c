#include "utilities.h"

int isnumber(const char* str)
{

  if (*str==0) return 0;
  while (*str != 0)
  {
    if (!(isdigit(*str))) return 0;
    str++;
  }
  return 1;
}

int min(int a, int b)
{
  if (a<b) return a;
  return b;
}

int compareTimers(const struct timeval* a, const struct timeval* b)
{
  return (a->tv_sec - b -> tv_sec) * 1000000 + (a->tv_usec - b->tv_usec);
}

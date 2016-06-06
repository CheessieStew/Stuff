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


int compareTimers(const struct timeval* a, const struct timeval* b)
{
  return (a->tv_sec - b -> tv_sec) * 1000 + (a->tv_usec - b->tv_usec)/1000;
}

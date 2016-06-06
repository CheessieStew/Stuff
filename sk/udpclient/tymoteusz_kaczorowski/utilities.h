#ifndef UTILITIES_H
#define UTILITIES_H
#include <netinet/ip.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netdb.h>
#include <ctype.h>

int isnumber(const char* str);
int min (int a, int b);
int compareTimers(const struct timeval* a, const struct timeval* b);

#endif

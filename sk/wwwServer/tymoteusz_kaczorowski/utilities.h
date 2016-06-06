#ifndef UTILITIES_H
#define UTILITIES_H
#define CONNECTION_CLOSE 1
#define CONNECTION_KEEP_ALIVE 2
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
#include <sys/stat.h>
#include <sys/time.h>




int isnumber(const char* str);
int compareTimers(const struct timeval* a, const struct timeval* b);
#endif

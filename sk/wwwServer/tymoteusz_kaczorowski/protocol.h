#include "utilities.h"
#define PARSE_EPIC_FAIL -2
#define PARSE_TRASH -1
#define PARSE_FINISHED 0
#define PARSE_PARTIAL 1

typedef struct GetHeader
{
  char* what;
  char* host;
  int connection;
  int parsestate;
} GetHeader;

typedef struct AnswerHeader
{
  int answerCode;
  char* contentType;
  int contentLength;
  char* location;
} AnswerHeader;

int parseGet(char* line, GetHeader* ret);
AnswerHeader* makeAnswerHeader(GetHeader* gethdr, char* folder);
char* codeName(int code);

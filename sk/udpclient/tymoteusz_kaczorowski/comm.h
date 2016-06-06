#include "utilities.h"


int getFileFromServer(int sockfd, struct sockaddr_in* server,
                      int length, int segmentSize, int windowSize, int timeout,
                      FILE * out);

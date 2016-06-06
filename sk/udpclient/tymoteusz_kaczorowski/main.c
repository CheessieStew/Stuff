#include "utilities.h"
#include "comm.h"

int main(int argc, char* argv[])
{
  char* filename;
  int length;
  int windowSize = 4;
  int timeOut = 2000;
  int segmentSize = 1000;
  if (argc>=4 && isnumber(argv[1]) && isnumber(argv[3]))
  {
    filename = argv[2];
    length = atoi(argv[3]);
    if (argc==7 && isnumber(argv[4]) && isnumber(argv[5]) && isnumber(argv[6]))
    {
      windowSize = atoi(argv[4]);
      timeOut = atoi(argv[5]);
      segmentSize = atoi(argv[6]);
    }
    else if (argc!=4)
    {
      printf("Usage:\nudpclient [port] [output file] [length]\n");
      printf("Optional: [window size] [timeout] [segment size]\n");
      return EXIT_FAILURE;
    }
  }
  else
  {
    printf("Usage:\nudpclient [port] [output file] [length]\n");
    printf("Optional: [window size] [timeout] [segment size]\n");
    return EXIT_FAILURE;
  }

	int sockfd = socket(AF_INET, SOCK_DGRAM, 0);
	if (sockfd < 0) {
		fprintf(stderr, "socket error: %s\n", strerror(errno));
		return EXIT_FAILURE;
	}
  struct sockaddr_in clientAddress;
  memset(&clientAddress, 0, sizeof(struct sockaddr_in));
  clientAddress.sin_family = AF_INET;
  clientAddress.sin_port = htons(1337);
  clientAddress.sin_addr.s_addr = htonl(INADDR_ANY);
  if (bind (sockfd, (struct sockaddr*)&clientAddress, sizeof(clientAddress)) < 0)
  {
    fprintf(stderr, "bind error: %s\n", strerror(errno));
    return EXIT_FAILURE;
  }

  struct addrinfo* result;
  struct addrinfo hints = {
    .ai_family = AF_INET,
  };
  int r = getaddrinfo("aisd.ii.uni.wroc.pl",argv[1], &hints, &result);
  if (r!=0)
  {
    fprintf(stderr,"getaddrinfo error: %s\n", strerror(errno));
    return EXIT_FAILURE;
  }
  if (result==NULL)
  {
    fprintf(stderr,"aisd.ii.uni.wroc.pl not found in DNS :(\n");
    return EXIT_FAILURE;
  }
  struct sockaddr_in* addr = (struct sockaddr_in*)(result->ai_addr);


  //socklen_t senderLen = sizeof(sender);
  //recvfrom(sockfd, recvd, IP_MAXPACKET+1, 0, (struct sockaddr*)&sender, &senderLen);
  //printf("=====\n%s\n====\n",recvd);
  FILE* out = fopen(filename,"w");

  if (getFileFromServer(sockfd, addr, length, segmentSize, windowSize, timeOut, out) < 0)
  {
    fprintf(stderr,"getFileFromServer failed\n");
    return EXIT_FAILURE;
  }
  return EXIT_SUCCESS;

  fclose(out);
	close (sockfd);
	return EXIT_SUCCESS;
}

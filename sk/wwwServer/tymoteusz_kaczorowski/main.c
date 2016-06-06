#include "utilities.h"
#include "comm.h"

#define BUFFERSIZE 8192

int main(int argc, char* argv[])
{

  if (!(argc == 3 && isnumber(argv[1])))
  {
    printf("Usage:\n");
    return EXIT_FAILURE;
  }
  int port = atoi(argv[1]);
  struct stat sb;
  if (!(stat(argv[2],&sb) == 0 && S_ISDIR(sb.st_mode)))
  {
    fprintf(stderr,"Folder does not exist\n");
    return EXIT_FAILURE;
  }



  int sockfd = socket(AF_INET, SOCK_STREAM, 0);
  if (sockfd<0)
  {
    fprintf(stderr,"socket error: %s\n",strerror(errno));
    return EXIT_FAILURE;
  }

  struct sockaddr_in serverAddr;
  memset(&serverAddr, 0, sizeof(struct sockaddr_in));
  serverAddr.sin_family = AF_INET;
  serverAddr.sin_port = htons(port);
  serverAddr.sin_addr.s_addr = htonl(INADDR_ANY);


  if (bind(sockfd,(struct sockaddr*)&serverAddr,sizeof(serverAddr)) < 0)
  {
    fprintf(stderr,"bind error: %s\n",strerror(errno));
    return EXIT_FAILURE;
  }

  if (listen(sockfd, 64) < 0)
  {
    fprintf(stderr,"listen error: %s\n",strerror(errno));
    return EXIT_FAILURE;
  }

  printf("starting Kwasne Powidelko Server v0.6.6.6\n");
  while (1)
  {
    int connSockfd = accept(sockfd, NULL, NULL);
    int cpid = fork();
    if (cpid==0)
    {
      printf("new connection, waiter ID: %d\n",getpid());
      int serveRes = serveClient(connSockfd, argv[2]);
      shutdown(connSockfd,SHUT_RDWR);
      close(connSockfd);
      if (serveRes<0)
        printf("waiter %d terminating after a failure\n",getpid());
      else
        printf("waiter %d terminating with success\n",getpid());
      return EXIT_SUCCESS;
    }
    if (cpid<0)
    {
      fprintf(stderr,"fork error: %s\n",strerror(errno));
    }
  }
}

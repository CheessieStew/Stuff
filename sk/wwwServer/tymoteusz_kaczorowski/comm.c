#include "comm.h"
#include "utilities.h"
#include <poll.h>
#include "protocol.h"
#define GOTSOMETRASH -1
#define GOTNOTHING 0
#define GOTPROBLEM -2

int sockReadLine(int sockfd, char* target, int timeout);
int sendAnswer(int sockfd, AnswerHeader* answer);

int serveClient(int sockfd, char* folder)
{
  int timeout = 1500;
  char buffer[4086];
  int recvd;
  struct timeval pre;
  struct timeval post;
  GetHeader gethdr;
  gethdr.host = malloc(1024);
  gethdr.what = malloc(1024);
  int howManyTrashLines = 0;
  gethdr.parsestate = PARSE_FINISHED;
  AnswerHeader* answer;
  while (timeout > 0 && howManyTrashLines < 20)
  {
    gettimeofday(&pre, NULL);
    recvd = sockReadLine(sockfd, buffer, timeout);
    if (recvd == GOTPROBLEM)
    {
      fprintf(stderr, "Waiter %d: Serving client failed, sockReadLine\n",
              getpid());
      return -1;
    }
    if (recvd == GOTNOTHING)
    {
      gettimeofday(&post, NULL);
      timeout -= compareTimers(&post, &pre);
    }
    else
    {
      timeout = 1500;
      if (recvd == GOTSOMETRASH) // bardzo duzo syfu
      {
        answer = malloc(sizeof(AnswerHeader));
        answer->answerCode = 501;
        answer->contentLength = 0;
        printf("Waiter %d: got some trash\n",getpid());
        sendAnswer(sockfd,answer);
        free(answer);
      }
      else
      {

        int parseres = parseGet(buffer, &gethdr);
        switch(parseres)
        {
          case PARSE_PARTIAL:
          howManyTrashLines = 0;
          break;

          case PARSE_FINISHED:
          if (gethdr.parsestate == PARSE_PARTIAL)
          {
            answer = makeAnswerHeader(&gethdr, folder);
            if (gethdr.connection == CONNECTION_CLOSE)
              timeout = 0;
            printf("Waiter %d: get %s from %s\n",getpid(), gethdr.what,
                   gethdr.host);
          }
          else
          {
            answer = malloc(sizeof(AnswerHeader));
            answer->answerCode = 501;
            answer->contentLength = 0;
            answer->contentType = "text/html";
            printf("Waiter %d: got some trash\n", getpid());
          }
          sendAnswer(sockfd,answer);
          free(answer);
          break;

          case PARSE_TRASH:
          howManyTrashLines++;
          break;

          case PARSE_EPIC_FAIL:
          fprintf(stderr, "Waiter %d: Serving client failed, parsing\n",
                  getpid());
          return -1;

          default:
          break;
        }
        gethdr.parsestate = parseres;
      }
    }
  }
  printf("Waiter %d ending, trash: %d\n",getpid(),howManyTrashLines);
  return 0;
}

int sendAnswer(int sockfd, AnswerHeader *hdr)
{
  printf("Waiter %d: sending answer\n",getpid());
  char* html501 = "<html><body>501 ;-;</body></html>";
  char* html404 = "<html><body>404 :(</body></html>";
  char* html403 = "<html><body>403 :I</body></html>";
  char* format =
"HTTP/1.1 %d %s\r\n\
Server: Kwasne Powidelko server v0.6.6.6\r\n\
Content-Length: %d\r\n\
Content-Type: %s\r\n\
\r\n";
  FILE* file;
  char buffer[4086];

  switch (hdr->answerCode)
  {
    case 501:
    hdr->contentLength = strlen(html501);
    break;
    case 404:
    hdr->contentLength = strlen(html404);
    break;
    case 403:
    hdr->contentLength = strlen(html403);
    break;
  }

  sprintf(buffer, format,hdr->answerCode, codeName(hdr->answerCode),
          hdr->contentLength, hdr->contentType);
  if (send(sockfd, buffer, strlen(buffer), 0) < 0)
  {
    fprintf(stderr,"Waiter %d: send error %s\n",getpid(),strerror(errno));
    return -1;
  }

  switch (hdr->answerCode)
  {
    case 501:
    if (send(sockfd, html501, strlen(html501), 0) < 0)
    {
      fprintf(stderr,"Waiter %d: send error %s\n",getpid(),strerror(errno));
      return -1;
    }
    break;

    case 404:
    if (send(sockfd, html404, strlen(html404), 0) < 0)
    {
      fprintf(stderr,"Waiter %d: send error %s\n",getpid(),strerror(errno));
      return -1;
    }
    break;

    case 403:
    if (send(sockfd, html403, strlen(html403), 0) < 0)
    {
      fprintf(stderr,"Waiter %d: send error %s\n",getpid(),strerror(errno));
      return -1;
    }
    break;

    case 301:
    case 200:
    file = fopen(hdr->location, "r");
    int rd = 1;
    while (rd>0)
    {
      rd = fread(buffer, 1, 4086, file);
      if (send(sockfd, buffer, rd, 0) < 0)
      {
        fprintf(stderr,"Waiter %d: send error %s\n",getpid(),strerror(errno));
        return -1;
      }
    }
    fclose(file);
    break;
  }
  return 0;
}

int sockReadLine(int sockfd, char* target, int timeout)
{
  static char localBuffer[4086];
  static int bufStart;
  static int bufLen;
  static int readLastTime;
  static int shouldTryToCopy;

  if (!readLastTime)
  {
    struct pollfd mypfd;
    printf("Waiter %d, polling (timeout = %d)\n",getpid(),timeout);
    poll(&mypfd, 1, timeout);
  }
  if (bufStart+bufLen < 4086)
  {
    int read = recv(sockfd, localBuffer+bufStart+bufLen,
                    4086-bufStart-bufLen, MSG_DONTWAIT);
    if (read < 0 && errno != EAGAIN && errno != EWOULDBLOCK)
    {
      fprintf(stderr,"Waiter %d: recv error: %s\n",getpid(),strerror(errno));
      return GOTPROBLEM;
    }
    if (read >0)
      bufLen += read;
    shouldTryToCopy = 1;
  }
  if (bufStart + bufLen > 4086 && bufLen < 4086)
  {
    int read = recv(sockfd, localBuffer + ((bufStart+bufLen)%4086),
                    bufStart - ((bufStart+bufLen)%4086), MSG_DONTWAIT);
    if (read < 0 && errno != EAGAIN && errno != EWOULDBLOCK)
    {
      fprintf(stderr,"recv error: %s\n",strerror(errno));
      return GOTPROBLEM;
    }
    if (read >0)
      bufLen += read;
    shouldTryToCopy = 1;
  }



  if ((!readLastTime && !shouldTryToCopy) || bufLen == 0)
  {
    readLastTime = 0;
    shouldTryToCopy = 0;
    return GOTNOTHING;
  }

  if (bufLen == 0)
  {
    readLastTime = 0;
    shouldTryToCopy = 0;
    return GOTNOTHING;
  }


  int i = 0;
  for (; i<bufLen && localBuffer[(bufStart+i)%4086]!='\r'; i++);
  if (localBuffer[(bufStart+i)%4086]!='\r' && bufLen < 4086)
  {
    //moze jeszcze nie skonczyli wysylac linijki, po prostu
    readLastTime = 0;
    shouldTryToCopy = 0;
    return GOTNOTHING;
  }

  if (i==bufLen)
  {
    if (bufLen == 4086)
    {
      //dostalismy tyle, ze bufor przepelniony, ale bez znaku nowej linii
      //uznajemy to za smieci
      bufStart = 0;
      bufLen = 0;
      readLastTime = 0;
      shouldTryToCopy = 0;
      printf("Waiter %d: sockReadLine buffer overflow\n",getpid());
      return GOTSOMETRASH;
    }
    else
    {
      //moze jeszcze nie skonczyli wysylac linijki, po prostu
      readLastTime = 0;
      shouldTryToCopy = 0;
      return GOTNOTHING;
    }
  }
  i = (bufStart+i)%4086;
  if (localBuffer[i+1]!='\n')
  {
      // \r bez \n - smieci
      bufLen -= i - bufStart + 1;
      bufStart += i - bufStart + 1;
      bufStart %= 4086;
      readLastTime = 0;
      printf("Waiter %d: \\r bez \\n\n",getpid());
      return GOTSOMETRASH;
  }
  else
  {
    int wrote = 0;
    if (i>=bufStart)
    {
      wrote += i-bufStart;
      strncpy(target, localBuffer+bufStart, i-bufStart);
    }
    else
    {
      wrote += 4085-bufStart + i;
      strncpy(target, localBuffer+bufStart, 4085-bufStart);
      strncpy(target+4085-bufStart, localBuffer, i);
    }
    bufLen -= wrote+2;
    bufStart += wrote+2;
    bufStart %= 4086;
    readLastTime = 1;
    target[wrote] = 0;
    return wrote+1;
  }
}

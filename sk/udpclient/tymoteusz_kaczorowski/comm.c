#include "comm.h"
#include <sys/time.h>

typedef struct DataSegment
{
  int number;
  struct timeval tv;
  uint8_t* segment;
  uint8_t received;
} DataSegment;

DataSegment* newWindow(int windowSize, int segmentSize);
int sendGet(int sockfd, struct sockaddr_in* server, int start, int length);
int handlePacket(int sockfd, struct sockaddr_in* server, DataSegment* window,
                 int windowStart, int windowSize, int segmentSize, int numberOfSegments);

DataSegment* newWindow(int windowSize, int segmentSize)
{
  DataSegment *res = malloc(sizeof(DataSegment) * windowSize);
  for (int i=0; i<windowSize; i++)
  {
    res[i].number = i;
    res[i].segment = malloc(sizeof(uint8_t)*segmentSize+1);
    res[i].segment[0] = 0;
    res[i].tv.tv_sec = 0;
    res[i].tv.tv_usec = 0;
    res[i].received = 0;
  }
  return res;
}

int sendGet(int sockfd, struct sockaddr_in* server, int start, int length)
{
  char message[100];
  sprintf(message,"GET %d %d\n",start, length);
  ssize_t messageLength = strlen(message);
  if (sendto(sockfd,message,messageLength,0,(struct sockaddr*)server,
              sizeof(struct sockaddr_in)) != messageLength)
  {
    fprintf(stderr,"sendto error: %s\n", strerror(errno));
    return -1;
  }
  return 0;
}

int getFileFromServer(int sockfd, struct sockaddr_in* server,
                      int length, int segmentSize, int windowSize, int timeout,
                      FILE * out)
{

  int numberOfSegments = (length+segmentSize/2)/segmentSize;
  int actualWindowSize = min(numberOfSegments,windowSize);
  fd_set readset;
  struct timeval untilEarliestTimeOut;
  struct timeval lastUpdate;
  int selectResult;
  int windowStart = 0;
  int numberOfReceived = 0;

  DataSegment* window = newWindow(actualWindowSize, segmentSize);

  while (numberOfReceived != numberOfSegments)
  {
    //determine time until next timeout
    //send GET for each not-yet-received segment and reset their timers
    untilEarliestTimeOut.tv_sec = timeout/1000;
    untilEarliestTimeOut.tv_usec = (timeout % 1000) * 1000;
    for (int i=0; i<actualWindowSize; i++)
    {
      if (window[i].received == 0 && window[i].number < numberOfSegments &&
          window[i].tv.tv_sec*1000000 + window[i].tv.tv_usec <= 0)
      {
        if (sendGet(sockfd, server, window[i].number*segmentSize, segmentSize)<0)
          return -1;
        window[i].tv.tv_sec = timeout/1000;
        window[i].tv.tv_usec = (timeout % 1000) * 1000;
      }
      else if (window[i].received==0 && window[i].number < numberOfSegments &&
               compareTimers(&window[i].tv, &untilEarliestTimeOut) <= 0)
      {
        untilEarliestTimeOut.tv_sec = window[i].tv.tv_sec;
        untilEarliestTimeOut.tv_usec = window[i].tv.tv_usec;
      }
    }
    lastUpdate.tv_sec = untilEarliestTimeOut.tv_sec;
    lastUpdate.tv_usec = untilEarliestTimeOut.tv_usec;

    while (numberOfReceived != numberOfSegments &&
           (untilEarliestTimeOut.tv_sec > 0 || untilEarliestTimeOut.tv_usec >0))
    {
      FD_ZERO(&readset);
      FD_SET(sockfd,&readset);
      selectResult = select(sockfd+1,&readset,NULL,NULL,&untilEarliestTimeOut);
      if (selectResult<-1)
      {
        fprintf(stderr, "select error: %s\n", strerror(errno));
        return -1;
      }
      else
      {
        for (int i=0; i<actualWindowSize; i++)
        if (window[i].received==0 && window[i].tv.tv_sec * 1000000 + window[i].tv.tv_usec >0)
        {
          window[i].tv.tv_sec -= lastUpdate.tv_sec - untilEarliestTimeOut.tv_sec;
          window[i].tv.tv_usec -= lastUpdate.tv_usec - untilEarliestTimeOut.tv_usec;
          if (window[i].tv.tv_usec < 0)
          {
            window[i].tv.tv_sec -= 1;
            window[i].tv.tv_usec += 1000000;
          }
        }
        lastUpdate.tv_sec = untilEarliestTimeOut.tv_sec;
        lastUpdate.tv_usec = untilEarliestTimeOut.tv_usec;

        if (selectResult>0)
        {
          int handleres = handlePacket(sockfd, server, window, windowStart,
                                       actualWindowSize, segmentSize, numberOfSegments);
          if (handleres<0) return -1;
          numberOfReceived+=handleres;
          if (handleres>0) printf("Got: %d%%\n",100*numberOfReceived/numberOfSegments);
          while (window[windowStart].received)
          {
            int dataLen = segmentSize;
            if (window[windowStart].number == numberOfSegments-1) dataLen = length % segmentSize;
            if (dataLen == 0) dataLen = segmentSize;
            for (int i=0; i<dataLen; i++) fputc(window[windowStart].segment[i],out);
            window[windowStart].number = window[(actualWindowSize + windowStart-1)%actualWindowSize].number +1;
            window[windowStart].tv.tv_sec = 0;
            window[windowStart].tv.tv_usec = 0;
            window[windowStart].segment[0] = 0;
            window[windowStart].received = 0;
            windowStart = (windowStart+1)%actualWindowSize;
          }
        }
      }
    }
  }

  return 0;
}

int handlePacket(int sockfd, struct sockaddr_in* server, DataSegment* window,
                 int windowStart, int windowSize, int segmentSize, int numberOfSegments)
{
  u_int8_t buffer[IP_MAXPACKET+1];
  struct sockaddr_in sender;
  socklen_t senderLen = sizeof(sender);
  ssize_t datagramLen = recvfrom(sockfd, buffer, IP_MAXPACKET, 0,
                                (struct sockaddr*)&sender , &senderLen);
  if (datagramLen < 0)
  {
    fprintf(stderr,"recvfrom error: %s\n",strerror(errno));
    return -1;
  }
  if (sender.sin_addr.s_addr == server->sin_addr.s_addr &&
      sender.sin_port == server->sin_port)
  {
    int dataStart = -1, dataLen = -1;
    sscanf((char*)buffer, "DATA %d %d\n", &dataStart, &dataLen);
    int segmentAt = (windowStart + dataStart/segmentSize - window[windowStart].number) % windowSize;
    if (dataStart % segmentSize == 0 && dataLen == segmentSize && dataStart/segmentSize < numberOfSegments &&
        window[segmentAt].received==0 && window[segmentAt].number==dataStart/segmentSize)
    {
      window[segmentAt].received = 1;
      uint8_t* bufFinger = buffer;

      while (bufFinger[0]!='\n') bufFinger++;
      bufFinger++;
      for (int i=0; i<dataLen; i++) window[segmentAt].segment[i] = bufFinger[i];
      window[segmentAt].tv.tv_sec = 0;
      window[segmentAt].tv.tv_usec = 0;
      return 1;
    } else return 0;
  }
  else return 0;
}

#include "protocol.h"


void fillAnswerHeader(char* filename, char* host, AnswerHeader* ret);
char* checkFileType(char* filename);

char* codeName(int code)
{
  switch(code)
  {
    case 200:
      return "OK";
    case 301:
      return "Moved Permanently";
    case 403:
      return "Forbidden";
    case 404:
      return "Not Found";
    case 501:
      return "Not implemented";
    default:
      return "Not implemented";
  }
}

int parseGet(char* line, GetHeader* ret)
{
  if (ret == NULL)
    return PARSE_EPIC_FAIL;
  if (ret->host == NULL || ret->what == NULL)
    return PARSE_EPIC_FAIL;

  if (line[0]==0)
    return PARSE_FINISHED;

  if (ret->parsestate == PARSE_TRASH)
    return PARSE_TRASH;

  if (strncmp(line,"GET",3)==0)
  {
    if (ret->parsestate != PARSE_FINISHED)
      return PARSE_TRASH;
    int i=0;
    while (line[4+i] != ' ' && line[4+i] != 0)
      i++;
    if (line[4+i]==0)
      return PARSE_TRASH;
    strncpy(ret->what,line+4,i);
    ret->what[i]=0;
    for (int j=4+i; line[j]!=0; j++)
      line[j]=tolower(line[j]);
    if (strncmp(line+4+i+1,"http",4)!=0)
      return PARSE_TRASH;
    else
      return PARSE_PARTIAL;
  }
  else if (strncmp(line,"Connection: ",12)==0)
  {
    for (int i=12; line[i]!=0; i++)
      line[i]=tolower(line[i]);
    if (strncmp(line+12,"keep-alive",10)==0)
      ret->connection=CONNECTION_KEEP_ALIVE;
    else if (strncmp(line+12,"close",5)==0)
      ret->connection=CONNECTION_CLOSE;
    else
      return PARSE_TRASH;
    return PARSE_PARTIAL;
  }
  else if (strncmp(line,"Host: ",6)==0)
  {
    strcpy(ret->host,line+6);
    return PARSE_PARTIAL;
  }
  else if (ret->parsestate != PARSE_PARTIAL)
    return PARSE_TRASH;
  else
    return (ret->parsestate);

}

AnswerHeader* makeAnswerHeader(GetHeader* gethdr, char* folder)
{
  AnswerHeader *ret = malloc(sizeof(AnswerHeader));
  ret->answerCode = 0;

  if (gethdr->parsestate == PARSE_TRASH)
  {
    ret->answerCode = 501;
    return ret;
  }

  char filename[1024];
  char absname[1024]; //abs po sklejeniu
  char absfolderdomain[1024]; //absolutna domeny
  strcpy(filename,folder);

  int i = strlen(folder);
  if (filename[i-1]!='/')
  {
    filename[i]='/';
    i++;
  }
  filename[i] = 0;

  int j = strlen(gethdr->host)-1;
  while (gethdr->host[j]!=':' && j>0)
    j--;
  if (j==0)
    j = strlen(gethdr->host);
  strncpy(filename+i,gethdr->host,j);
  filename[i+j] = 0;
  realpath(filename,absfolderdomain);

  struct stat sb;
  if (!(stat(filename,&sb)==0 && S_ISDIR(sb.st_mode)))
  {
    printf("Waiter %d: 403 cause host not a folder\n", getpid());
    ret->answerCode = 403;
    ret->contentType = "text/html";
    ret->contentLength = 0;
    return ret;
  }

  if (gethdr->what[0]!='/')
  {
    filename[i+j] = '/';
    j++;
  }
  strcpy(filename+i+j,gethdr->what);

  realpath(filename,absname);
  if (strncmp(absname,absfolderdomain,strlen(absfolderdomain))!=0)
  {
    ret->answerCode = 403;
    printf("Waiter %d: 403 cause prefixes differ\n", getpid());
    ret->contentType = "text/html";
    ret->contentLength = 0;
    return ret;
  }

  fillAnswerHeader(filename,gethdr->host,ret);
  return ret;
}

void fillAnswerHeader(char* filename, char* host, AnswerHeader* ret)
{
  struct stat sb;
  ret->location = filename;
  if (!(stat(filename,&sb) == 0))
  {
    //nawet, jesli to bylo przekierowanie (301)
    ret->answerCode = 404;
    ret->contentLength = 0;
    ret->contentType = "text/html";
    ret->location = "";
    printf("404...\n");
    return;
  }

  if (S_ISDIR(sb.st_mode))
  {
    ret->answerCode = 301;
    if (filename[strlen(filename)-1]!='/')
    {
      filename[strlen(filename)+1] = 0;
      filename[strlen(filename)] = '/';
    }
    strcpy(filename+strlen(filename),"index.html");
    return fillAnswerHeader(filename, host, ret);
  }
  if (ret->answerCode != 301)
    ret->answerCode = 200;
  ret->contentLength = sb.st_size;
  ret->contentType = checkFileType(filename);
}

char* checkFileType(char* filename)
{
  int i = strlen(filename)-1;
  while (filename[i]!='.' && i>0)
    i--;
  if (i==0)
    return "application/octet-stream";
  if (strncmp(filename+i,".txt",4)==0)
    return "text/txt";
  if (strncmp(filename+i,".html",5)==0)
    return "text/html";
  if (strncmp(filename+i,".css",4)==0)
    return "text/css";
  if (strncmp(filename+i,".jpg",4)==0)
    return "image/jpg";
  if (strncmp(filename+i,".jpeg",5)==0)
    return "image/jpeg";
  if (strncmp(filename+i,".png",4)==0)
    return "image/png";
  if (strncmp(filename+i,".pdf",4)==0)
    return "application/pdf";
  return "application/octet-stream";

}

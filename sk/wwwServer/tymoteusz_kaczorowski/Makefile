CC=gcc
CFLAGS= -std=gnu99 -g -Wall -W -I.
DEPS = utilities.h protocol.h comm.h
OBJ = main.o utilities.o protocol.o comm.o

%.o: %.c $(DEPS)
	$(CC) -c -o $@ $< $(CFLAGS)

wwwserver: $(OBJ)
	gcc -o $@ $^ $(CFLAGS)
	cp -f wwwserver ../wwwserver

clean:
	rm -f $(OBJ)

distclean:
	rm -f wwwserver $(OBJ)

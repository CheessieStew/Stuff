CC=gcc
CFLAGS= -std=gnu99 -g -Wall -W -I.
DEPS = utilities.h comm.h
OBJ = main.o utilities.o comm.o

%.o: %.c $(DEPS)
	$(CC) -c -o $@ $< $(CFLAGS)

udpclient: $(OBJ)
	gcc -o $@ $^ $(CFLAGS)

clean:
	rm -f $(OBJ)

distclean:
	rm -f udpclient $(OBJ)

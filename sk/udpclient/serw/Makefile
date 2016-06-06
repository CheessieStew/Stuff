CC=gcc
CFLAGS= -std=gnu99 -g -Wall -W -I.
DEPS =
OBJ = ser.o

%.o: %.c $(DEPS)
	$(CC) -c -o $@ $< $(CFLAGS)

udpclient: $(OBJ)
	gcc -o $@ $^ $(CFLAGS)

clean:
	rm -f $(OBJ)

distclean:
	rm -f udpclient $(OBJ)

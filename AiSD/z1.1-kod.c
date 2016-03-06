#include <stdio.h>

struct Heap {int *tab; int maxIndex; int Capacity;};

void budujkopiec(struct Heap *heap);
void przesunnizej (struct Heap *heap, int i);
void przesunwyzej (struct Heap *heap, int i);


int leftSon (int i)
{
    return (2*(i- (i%2) +1) + (i%2));
}

int rightSon (int i)
{
    return leftSon(i) + 2;
}

int parent (int i)
{
    if (i==0 || i==1) return i;
    if (i%4 == 0 || i%4 == 1) return (parent(i-2));
    return (i / 2) - 1 + i%2;
}

int neighbour (int i, int maxIndex)
{
    int mirror = i + 1 - 2 * (i%2);
    if (leftSon(mirror) <= maxIndex) return leftSon(mirror);
    if (mirror <= maxIndex) return mirror;
    return parent(mirror);
}

void budujkopiec(struct Heap *heap)
{
    for (int i= heap->maxIndex; i>=0; i--)
        {
            if ((i%2) == 0) przesunnizej (heap, i);
            else przesunwyzej (heap,i);
        }
}

void przesunnizej (struct Heap *heap, int i)
{
    int k=i;
    int j;
    do
    {
        j=k;
        if (j%2==0 && leftSon(j) <= heap->maxIndex && heap->tab[leftSon(j)] > heap->tab[k]) k = leftSon(j);
        if (j%2==0 && rightSon(j) <= heap->maxIndex && heap->tab[rightSon(j)] > heap->tab[k]) k = rightSon(j);
        if (j%2==1 && j>1 && heap->tab[parent(j)] > heap->tab[k]) k = parent(j);
        if (j%2==0 && leftSon(j)> heap->maxIndex && heap->tab[neighbour(j,heap->maxIndex)] > heap->tab[k]) k = neighbour(j,heap->maxIndex);
        int placeholder = heap->tab[k];
        heap->tab[k] = heap->tab[j];
        heap->tab[j] = placeholder;
    }
    while (j!=k);
}

void przesunwyzej (struct Heap *heap, int i)
{
    int k=i;
    int j;
    do
    {
        j=k;
        if (j%2==1 && leftSon(j) <= heap->maxIndex && heap->tab[leftSon(j)] < heap->tab[k]) k = leftSon(j);
        if (j%2==1 && rightSon(j) <= heap->maxIndex && heap->tab[rightSon(j)] < heap->tab[k]) k = rightSon(j);
        if (j%2==0 && j>1 && heap->tab[parent(j)] < heap->tab[k]) k = parent(j);
        if (j%2==1 && leftSon(j)> heap->maxIndex && heap->tab[neighbour(j,heap->maxIndex)] < heap->tab[k]) k = neighbour(j,heap->maxIndex);
        int placeholder = heap->tab[k];
        heap->tab[k] = heap->tab[j];
        heap->tab[j] = placeholder;
    }
    while (j!=k);
}

void insert(struct Heap *heap, int what)
{
    if (heap->maxIndex +2 > heap->Capacity)
    {
        return;
    }
    heap->maxIndex++;
    heap->tab[heap->maxIndex]=what;
    if (what>parent(heap->maxIndex)) przesunwyzej(heap,heap->maxIndex);
    else przesunnizej(heap,heap->maxIndex);
}

int deletemin(struct Heap *heap)
{
    if (heap->maxIndex == 0)
    {
        heap->maxIndex--;
        return heap->tab[0];
    }
    int res = heap->tab[1];
    heap->tab[1]=heap->tab[heap->maxIndex];
    heap->maxIndex--;
    przesunwyzej(heap,1);
    return res;
}
int deletemax (struct Heap *heap)
{
    int res = heap->tab[0];
    heap->tab[0]=heap->tab[heap->maxIndex];
    heap->maxIndex--;
    if (heap->maxIndex>=0) przesunnizej(heap,0);
    return res;
}

int main()
{
    struct Heap heap;
    heap.maxIndex = 9;
    heap.Capacity = 64;
    heap.tab = malloc(sizeof(int)*64);
    for (int i=0; i<=63; i++) heap.tab[i] = i;
    budujkopiec(&heap);

    while (heap.maxIndex>=0)
    {
         int a = deletemax(&heap);
         printf("%d\n",a);
    }
}


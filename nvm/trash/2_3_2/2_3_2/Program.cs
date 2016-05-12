using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_3_2
{
    //nie implementuje IEnumerable, bo który iterator miałby być domyślnym?
    class BinaryTreeNode<T>
    {
        public T Value;
        BinaryTreeNode<T> left;
        BinaryTreeNode<T> right;

        //tworzy węzeł z zadaną wartością
        public BinaryTreeNode(T val)
        {
            Value = val;
        }

        //inicjalizuje wartościami z tablicy w stylu prostego BST, korzystając do porównań z zadanego ewaluatora wartości typu T
        public BinaryTreeNode(T[] arr, Func<T,int> evaluator) 
        {
            Value = arr[0];
            for (int i=1; i<arr.Length; i++)
            {
                this.AddVal(arr[i], evaluator);
            }
        }

        //dodaje wartość w stylu prostego BST
        public void AddVal(T val, Func<T,int> evaluator)
        {

                if (evaluator(val) < evaluator(Value))
                {
                    if (left == null) left = new BinaryTreeNode<T>(val);
                    else left.AddVal(val, evaluator);
                }
                else
                {
                    if (right == null) right = new BinaryTreeNode<T>(val);
                    else right.AddVal(val, evaluator);
                }
        }

        //nie ma usuwania, bo nie jest potrzebna do testowania enumeratorów

        public DFSEnumerator GetDFSEnumerator()
        {
            return new DFSEnumerator(this);
        }
        public BFSEnumerator GetBFSEnumerator()
        {
            return new BFSEnumerator(this);
        }

        public IEnumerator<BinaryTreeNode<T>> GetDFSEnumeratorYield()
        {
            Stack<BinaryTreeNode<T>> dfsStack = new Stack<BinaryTreeNode<T>>();
            dfsStack.Push(this);
            BinaryTreeNode<T> current;
            while (dfsStack.Any())
            {
                current = dfsStack.Pop();
                if (current.right != null)
                {
                    dfsStack.Push(current.right);
                }
                if (current.left != null)
                {
                    dfsStack.Push(current.left);
                };
                yield return current;
            }
        }
        public IEnumerator<BinaryTreeNode<T>> GetBFSEnumeratorYield()
        {
            Queue<BinaryTreeNode<T>> bfsQueue = new Queue<BinaryTreeNode<T>>();
            bfsQueue.Enqueue(this);
            BinaryTreeNode<T> current;
            while (bfsQueue.Any())
            {
                current = bfsQueue.Dequeue();
                if (current.left != null)
                {
                    bfsQueue.Enqueue(current.left);
                };
                if (current.right != null)
                {
                    bfsQueue.Enqueue(current.right);
                }

                yield return current;
            }
        }

        //pre-order, bo in-order mniej wygodny do zrobienia, a nie o to tu chodzi
        public class DFSEnumerator : IEnumerator<BinaryTreeNode<T>>
        {
            private Stack<BinaryTreeNode<T>> dfsStack;
            private BinaryTreeNode<T> root;

            public BinaryTreeNode<T> Current
            {
                get;
                private set;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public DFSEnumerator(BinaryTreeNode<T> root)
            {
                dfsStack = new Stack<BinaryTreeNode<T>>();
                dfsStack.Push(root);
                this.root = root;
            }

            public void Dispose() {}

            public bool MoveNext()
            {
                bool res = false;
                if (dfsStack.Any())
                {
                    Current = dfsStack.Pop();
                    res = true;
                }

                if (Current.right != null)
                {
                    dfsStack.Push(Current.right);
                }
                if (Current.left != null)
                {
                    dfsStack.Push(Current.left);
                };
                return res;
            }

            public void Reset()
            {
                dfsStack.Clear();
                dfsStack.Push(root);
            }
        }
        public class BFSEnumerator : IEnumerator<BinaryTreeNode<T>>
        {
            private Queue<BinaryTreeNode<T>> bfsQueue;
            private BinaryTreeNode<T> root;

            public BinaryTreeNode<T> Current
            {
                get;
                private set;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public BFSEnumerator(BinaryTreeNode<T> root)
            {
                bfsQueue = new Queue<BinaryTreeNode<T>>();
                bfsQueue.Enqueue(root);
                this.root = root;
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                bool res = false;
                if (bfsQueue.Any())
                {
                    Current = bfsQueue.Dequeue();
                    res = true;
                }
                if (Current.left != null)
                {
                    bfsQueue.Enqueue(Current.left);
                };
                if (Current.right != null)
                {
                    bfsQueue.Enqueue(Current.right);
                }
                return res;
            }

            public void Reset()
            {
                bfsQueue.Clear();
                Current = root;
                if (Current.left != null)
                {
                    bfsQueue.Enqueue(Current.left);
                };
                if (Current.right != null)
                {
                    bfsQueue.Enqueue(Current.right);
                }
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            int[] testu = new int[] { 33, 1, 5, 1337, 42, 666, 7 };
            BinaryTreeNode<int> testTree = new BinaryTreeNode<int>(testu, (x => x));

            BinaryTreeNode<int>.DFSEnumerator enum1 = testTree.GetDFSEnumerator();
            Console.WriteLine("DFS, pre-order: ");
            while (enum1.MoveNext() == true)
            {
                Console.Write($" {enum1.Current.Value}");
            }
            Console.WriteLine();

            BinaryTreeNode<int>.BFSEnumerator enum2 = testTree.GetBFSEnumerator();
            Console.WriteLine("BFS:");
            while (enum2.MoveNext() == true)
            {
                Console.Write($" {enum2.Current.Value}");
            }
            Console.WriteLine();

            IEnumerator<BinaryTreeNode<int>> enum3 = testTree.GetDFSEnumeratorYield();
            Console.WriteLine("DFS, pre-order, YIELD: ");
            while (enum3.MoveNext() == true)
            {
                Console.Write($" {enum3.Current.Value}");
            }
            Console.WriteLine();

            IEnumerator<BinaryTreeNode<int>> enum4 = testTree.GetBFSEnumeratorYield();
            Console.WriteLine("BFS, YIELD: ");
            while (enum4.MoveNext() == true)
            {
                Console.Write($" {enum4.Current.Value}");
            }
            Console.WriteLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_1_2
{

    class Grid
    {
        private int[][] arr;
        public int Rows { get; }
        public int Cols { get; }

        public int Size
        {
            get
            {
                return Rows * Cols;
            }
        }

        public Grid(int r, int c)
        {
            Rows = r;
            Cols = c;
            arr = new int[r][];
            for (int i = 0; i < r; i++)
            {
                arr[i] = new int[c];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int[] row in arr)
            {
                foreach (int elem in row)
                    sb.AppendFormat("{0}; ", elem);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public int[] this[int r]
        {
            get { return arr[r]; }
            set
            {
                if (value.Length != Cols) throw new Exception("Jasne, chciałbyś.");
                arr[r] = value;
            }
        }
        public int this[int r, int c]
        {
            get { return arr[r][c]; }
            set { arr[r][c] = value; }
        }

    }


    class Program
    {
        static void Main(string[] args)
        {
            Grid gr = new Grid(4, 5)
            {
                [0] = new[] {1, 2, 3, 4, 5},
                [1] = new[] {4, 2, 3, 1, 5},
                [2] = new[] {0, 0 ,0 ,0, 0},
                [3] = new[] { 1, 1, 1, 1, 1 }
            };
            var x = gr[2][0];
            x = 7;

            Console.WriteLine(gr);
            Console.ReadKey();
        }
    }
}

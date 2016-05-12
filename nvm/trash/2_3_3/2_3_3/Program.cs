using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_3_3
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] testu = new int[] { 1, 3, 3, 7, 1337, 42, 666, 6, 66 };
            List <int> testuList = new List<int>(testu);
            Console.WriteLine("Oryginalna lista:");
            Console.WriteLine(string.Join(",",testuList.ToArray()));

            List <double> testuList2 = testuList.ConvertAll(x => x / 2.0);
            Console.WriteLine("Po ConvertAll(x => x / 2.0):");
            Console.WriteLine(string.Join(",", testuList2.ToArray()));

            List<int> testuList3 = testuList.FindAll(x => x % 3 == 0);
            Console.WriteLine("Po FindAll(x => x % 3 == 0):");
            Console.WriteLine(string.Join(",", testuList3.ToArray()));

            Console.WriteLine("Wypisana metodą ForEach(x => Console.Write($\"{x}, \")):");
            testuList.ForEach(x => Console.Write($"{x}, "));
            Console.WriteLine();

            Console.WriteLine("Odfiltrowane RemoveAll(x => x % 3 == 0)");
            List<int> testuList4 = new List<int>(testu);
            testuList4.RemoveAll(x => x % 3 == 0);
            Console.WriteLine(string.Join(",", testuList4.ToArray()));

            Console.WriteLine("Posortowane mod 4, Sort((x,y) => x % 4 - y % 4)");
            testuList.Sort((x,y) => x % 4 - y % 4);
            Console.WriteLine(string.Join(",", testuList.ToArray()));
            Console.WriteLine("Dla jasności, takie ta posortowana lista ma wartości modulo 4:");
            testuList.ForEach(x => Console.Write($"{x % 4}, "));
            Console.WriteLine();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_4_8
{
    class Program
    {

        static Func<A,B> fix<A,B>(Func<Func<A, B>, A, B> f)
        {
            Func<A, B> fixf = null;
            fixf = (x => f(fixf, x));
            return fixf;
        }
        static void Main(string[] args)
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            foreach (var item in
            list.Select (i => (fix<int, int>(((f, x) => (x<=2) ? 1 : f(x-1)+ f(x-2))))(i)))
                Console.WriteLine(item);
        }
    }
}



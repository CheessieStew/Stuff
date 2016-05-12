using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_5_1
{
    class Program
    {

        static int LiczbaTestow = 10;
        static int LiczbaProb = 10000;
        static int TestRange = 1000;
        static int Foo (int x, int y)
        {
            int s = 0;
            if (x > y)
            {
                x = -x;
                y = -y;
            }
            while (x < y)
            {
                
                s += x % 17 + y % 31;
                x++;
                y--;
            }
            return s;
        }

        static dynamic FooD (dynamic x, dynamic y)
        {
            dynamic s = 0;
            if (x > y)
            {
                x = -x;
                y = -y;
            }
            while (x < y)
            {
                s += x % 17 + y % 31;
                x++;
                y--;
            }
            return s;
        }

        static void Test(int seed)
        {
            Console.WriteLine("Testuję:");
            Console.WriteLine($"{"Normal",20} {"Dynamic",20}");
            
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;

                Random ran = new Random(seed);
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Foo(p, p+ran.Next(TestRange));
                }

                DateTime end = DateTime.Now;
                TimeSpan testNormal = end - start;

                start = DateTime.Now;

                ran = new Random(seed);
                for (int p = 0; p < LiczbaProb; p++)
                {
                    FooD(p, p+ran.Next(TestRange));
                }

                end = DateTime.Now;
                TimeSpan testDynamic = end - start;

                Console.WriteLine($"{testNormal,20} {testDynamic,20}");
            }
        }

        static void Main(string[] args)
        {
            Test(5);

        }
    }
}

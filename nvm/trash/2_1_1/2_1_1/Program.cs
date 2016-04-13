using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_1_1
{
    class Program
    {


        private static bool Fine(int x)
        {
            int p = x;
            int sum = 0;
            int digit;
            while (p > 0)
            {
                digit = p % 10;
                if (digit == 0 || x % digit != 0) return false;
                sum += digit;
                p /= 10;
            }
            if (x % sum != 0) return false;
            return true;
        }

        static void Main(string[] args)
        {
            IEnumerable<int> goods = Enumerable.Range(1, 100000).Where(Fine);

            foreach (int x in goods)
            {
                Console.Write($"{x}; ");
            }

            Console.ReadKey();

        }
    }
}

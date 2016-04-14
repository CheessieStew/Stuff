using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_1_1
{
    /// <summary>
    /// Program class.
    /// </summary>
    class Program
    {

        /// <summary>
        /// A static method that checks, if x is divisible by each of it's digits and by the sum of all those digits.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>bool</returns>
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

        /// <summary>
        /// Main class, prints all the numbers from 1 to 100000 that satisfy the condition.
        /// </summary>
        /// <param name="args"></param>
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

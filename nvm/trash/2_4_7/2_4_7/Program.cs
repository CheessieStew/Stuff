using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_4_7
{
    class Program
    {
        static void Main(string[] args)
        {
            var smth = new { Field1 = "The value", Field2 = 5 };
            var list = new[] { smth }.ToList();
            list.Add(new { Field1 = "The other value", Field2 = 6 });
            list.ForEach(x => Console.WriteLine(x));
            //próba dodania czegoś innego typu anonimowego:
            //list.Add(new { Field1 = "The wrong value", Field2 = true });
            //kompilator jej nie lubi


        }
    }
}

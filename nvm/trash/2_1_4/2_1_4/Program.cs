using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace _2_1_4
{
    public class OznakowaneAttribute : Attribute { }

    class Something
    {
        [Oznakowane]
        public int Good()
        { return 42; }

        [Oznakowane]
        public bool TypeTrap()
        { return true; }

        public int NieoznakowaneTrap()
        { return 1337; }
        
        [Oznakowane]
        public int ParamTrap(int a)
        { return 1337 + a; }

        [Oznakowane]
        private int PrivTrap()
        { return 1337; }

        [Oznakowane]
        public static int StaticTrap()
        { return 1337; }
    }

    class Program
    {
        static void invokeOznakowane(Object o)
        {
            Console.WriteLine("Publiczne, niestatyczne metody () -> int:");
            List<MethodInfo> methods = o.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Where(m =>
                m.ReturnType == typeof(int) && m.GetParameters().Length == 0).ToList();
            methods.ForEach(m => Console.WriteLine($"metoda {m.Name}"));

            methods = methods.Where(m => m.CustomAttributes.Any(a => a.AttributeType == typeof(OznakowaneAttribute))).ToList();
            methods.ForEach(m => Console.WriteLine($"Wywołana metoda zwróciła {m.Invoke(o, null)}"));
        }

        static void Main(string[] args)
        {
            Something s = new Something();
            invokeOznakowane(s);
            Console.ReadKey();
        }
    }
}

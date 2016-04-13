using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace _2_1_3
{
    class Something
    {
        private int f1;
        public int F2;
        private int p1
        {
            get
            {
                return f1 * F2;
            }

            set
            {
                f1 = value / F2;
            }
        }
        public int P2 { get; set; }

        public Something()
        {
            f1 = 1;
            F2 = 2;
            p1 = 3;
            P2 = 4;
        }

        private void publicMethod()
        {
            Console.WriteLine($"Something.privateMethod");
        }

        private void privateMethod()
        {
            Console.WriteLine($"Something.privateMethod");
        }
    }

    class Program
    {
        private static void reflect(Object o)
        {
            Type t = o.GetType();
            Console.WriteLine($"Obiekt typu: {t}");
            Console.WriteLine($"Zawiera metody:");
            List<MethodInfo> methodList = t.GetMethods().ToList();
            methodList.ForEach(method => Console.WriteLine(method.ToString()));
            methodList.ForEach(x =>
                {
                    if (x.IsPrivate && x.GetParameters().Length == 0)
                    {
                        Console.WriteLine($"Wywołuję prywatną metodę:");
                        x.Invoke(o,null);
                    }
                });

        }

        static void Main(string[] args)
        {
            Something smth = new Something();
            reflect(smth);
            Console.ReadKey();
        }
    }
}

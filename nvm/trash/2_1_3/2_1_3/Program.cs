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

        public void PublicMethod()
        {
            return;
        }

        private void privateMethod(String str)
        {
            Console.WriteLine($"Something.privateMethod({str})");
        }

    }

    class Program
    {
        private static void reflect(Object o)
        {
            Type t = o.GetType();
            String str = "Hello. Yes, this is reflection.";
            Console.WriteLine($"Obiekt typu: {t}");
            Console.WriteLine();
            List<MethodInfo> methodList = t.GetMethods(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            List<PropertyInfo> propertyList = t.GetProperties(
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            Console.WriteLine($"Prywatne metody:");
            methodList.ForEach(method => Console.WriteLine($"{method.ToString()}"));
            Console.WriteLine();
            Console.WriteLine($"Prywatne właściwości:");
            propertyList.ForEach(prop => Console.WriteLine($"{prop.ToString()}"));
            Console.WriteLine();
   
            methodList.ForEach(x =>
                {
                if (x.IsPrivate && x.GetParameters().Length == 1 && x.GetParameters().First().ParameterType == str.GetType())
                    {
                        Console.WriteLine($"Wywołuję prywatną metodę:");
                        x.Invoke(o, new object[] { str });
                    }
                });
            Console.WriteLine();

            propertyList.ForEach(x =>
            {
                    Console.WriteLine($"Wartość prywatnej wł. {x.Name} = {x.GetValue(o)}");
            });


            Console.WriteLine("Testy!");
            int liczbaTestow = 10;
            int liczbaProb = 10000;
            TimeSpan[] resultsNoRef;
            if (t == typeof(Something)) resultsNoRef = testPublicNoReflection((Something)o, liczbaTestow, liczbaProb);
            else
            {
                Console.WriteLine("obiekt typu innego, niż Something");
                return;
            }

            TimeSpan[] resultsRef = testPublicReflection("PublicMethod", o, liczbaTestow, liczbaProb);
            Console.WriteLine($"{"bez refleksji",20} | {"z refleksją",20}");
            for (int i = 0; i < liczbaTestow; i++)
                Console.WriteLine($"{resultsNoRef[i],20} | {resultsRef[i],20}");

        }

        private static TimeSpan[] testPublicNoReflection(Something s, int liczbaTestow, int liczbaProb)
        {
            TimeSpan[] res = new TimeSpan[liczbaTestow];
            DateTime start;
            DateTime end;
            for (int test = 0; test < liczbaTestow; test++)
            {
                start = DateTime.Now;
                for (int proba = 0; proba < liczbaProb; proba++)
                {
                    s.PublicMethod();
                }
                end = DateTime.Now;
                res[test] = start - end;
            }
            return res;
        }

        private static TimeSpan[] testPublicReflection(String methodName, Object o, int liczbaTestow, int liczbaProb)            
        {
            TimeSpan[] res = new TimeSpan[liczbaTestow];
            DateTime start;
            DateTime end;
            for (int test = 0; test < liczbaTestow; test++)
            {
                start = DateTime.Now;
                for (int proba = 0; proba < liczbaProb; proba++)
                {
                    o.GetType().GetMethod(methodName).Invoke(o, null);
                }
                end = DateTime.Now;
                res[test] = start - end;
            }
            return res;
        }

        static void Main(string[] args)
        {
            Something smth = new Something();
            reflect(smth);
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_4_2_5
{
    static class StreamReaderExt
    {
        public static IEnumerable<string> GetLines(this StreamReader reader)
        {

            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }



    }

    class Program
    {


        static void Main(string[] args)
        {
            StreamWriter to = new StreamWriter("liczby.temp");
            for (int i = 0; i < 100; i++)
            {
                to.WriteLine($"{32 * (i % 2) + 16 * (i % 3) + 8 * (i % 5) + 4 * (i % 7) + 2 * (i % 11) + 1 * (i % 13)}");
            }
            to.Close();
            StreamReader liczby = new StreamReader("liczby.temp");
            IEnumerable<int> query1 = from liczbaStr in liczby.GetLines()
                                      select Int32.Parse(liczbaStr) into liczba
                                      where liczba < 100
                                      orderby liczba
                                      select liczba;

            IEnumerable<int> query2 = liczby.GetLines().Select(Int32.Parse).Where(x => x < 100).OrderBy(x => x);
            query1.ToString().CompareTo(query2.ToString());
            Console.WriteLine($"2.4.2, przeformułowane wyrażenie = stare? {query1.ToString().CompareTo(query2.ToString()) == 0}");
            Console.WriteLine();
            liczby.Close();
            File.Delete("liczby.temp");

            to = new StreamWriter("nazwiska.temp");
            String[] testu = new String[]
                {"Kowalski","Malinowski","Krasicki","Abacki"};
            foreach (String s in testu)
                to.WriteLine(s);
            to.Close();
            StreamReader nazwiska = new StreamReader("nazwiska.temp");
            IEnumerable<char> query = from nazwisko in nazwiska.GetLines()
                                      group nazwisko by nazwisko[0] into grouped
                                      orderby grouped.Key
                                      select grouped.Key;
            Console.WriteLine("2.4.3, uzyskany zbiór:");
            foreach (char a in query)
                Console.Write($"{a} ");
            Console.WriteLine();
            Console.WriteLine();

            nazwiska.Close();
            File.Delete("nazwiska.temp");

            Console.WriteLine("Nazwa projektu kłamie, 2.4.4 i 2.4.5 tu nie ma");
            Console.WriteLine("Zrobiłbym 2.4.6, ale spać mi się chce");
        }
    }
}

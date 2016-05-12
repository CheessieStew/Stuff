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

            Console.WriteLine("Nazwa projektu kłamie, 2.4.4 tu nie ma");

            to = new StreamWriter("osobaPesel.temp");
            testu = new String[]
                {"Andrzej Kowalski 100000", "Andrzej Malinowski 9978123", "Andrzej Krasicki 3343343", "Andrzej Abacki 1212143"};
            foreach (String s in testu)
                to.WriteLine(s);
            to.Close();
            to = new StreamWriter("peselNumerKonta.temp");
            testu = new String[]
            {"100000 3", "4444444 2", "3343343 4", "1212143 1"};
            //jeden pesel niewystępujący w pierwszym pliku
            //jeden pesel z pierwszego nie występuje tu
            foreach (String s in testu)
                to.WriteLine(s);
            to.Close();

            StreamReader osoby = new StreamReader("osobaPesel.temp");
            StreamReader konta = new StreamReader("peselNumerKonta.temp");
            var q = from imNazPes in osoby.GetLines()
                    select imNazPes.Split(' ') into imNazPesSplt
                    join pesNrSplt in konta.GetLines().Select(s => s.Split(' ')) on imNazPesSplt[2] equals pesNrSplt[0]
                    select new { Imie = imNazPesSplt[0], Nazwisko = imNazPesSplt[1], Pesel = imNazPesSplt[2], Konto = pesNrSplt[1] };
            foreach (var thing in q)
                Console.WriteLine(thing);

            osoby.Close();
            konta.Close();
            File.Delete("osobaPesel.temp");
            File.Delete("peselNumerKonta.temp");

            Console.WriteLine();
            Console.WriteLine("Zrobiłbym 2.4.6, ale spać mi się chce");
        }
    }
}

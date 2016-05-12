using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace _2_4_2
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
            StreamWriter to = new StreamWriter("liczby.txt");
            for (int i = 0; i < 100; i++)
            {
                to.WriteLine($"{32* (i % 2) + 16 * (i % 3) + 8 * (i % 5) + 4 * (i % 7) + 2 * (i % 11) + 1 * (i % 13)}");
            }
            to.Close();
            StreamReader liczby = new StreamReader("liczby.txt");
            IEnumerable<int> query1 = from liczbaStr in liczby.GetLines()
                                    select Int32.Parse(liczbaStr) into liczba
                                    where liczba < 100
                                    orderby liczba
                                    select liczba;

            IEnumerable<int> query2 = liczby.GetLines().Select(Int32.Parse).Where(x=> x<100).OrderBy(x=> x);

            query1.ToString().CompareTo(query2.ToString());
            Console.WriteLine(query1.ToString().CompareTo(query2.ToString())==0);
        }
    }
}

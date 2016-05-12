using System;

using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _2_3_1
{
    class Program
    {
        static int LiczbaProb = 10000;
        static int LiczbaTestow = 10;
        static TimeSpan AverageAddingForList = TimeSpan.Zero;
        static TimeSpan AverageAddingForArrList = TimeSpan.Zero;
        static TimeSpan AverageAddingHash = TimeSpan.Zero;
        static TimeSpan AverageAddingDict = TimeSpan.Zero;


        static void TestListAddingRange(int[] testu)
        {
            Console.WriteLine("Testuję dodawanie (addRange):");
            Console.WriteLine($"{"List",20} {"ArrayList",20}");
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    List<int> listu = new List<int>();

                    listu.AddRange(testu);
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    ArrayList arrListu = new ArrayList();
                    arrListu.AddRange(testu);
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }
        static void TestListAddingFor(int[] testu)
        {
            Console.WriteLine("Testuję dodawanie (for):");
            Console.WriteLine($"{"List",20} {"ArrayList",20}");
            AverageAddingForList = TimeSpan.Zero;
            AverageAddingForArrList = TimeSpan.Zero;
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    List<int> listu = new List<int>();
                    foreach (int smth in testu)
                    {
                        listu.Add(smth);
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start;
                AverageAddingForList += testListu;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    ArrayList arrListu = new ArrayList();
                    foreach (int smth in testu)
                    {
                        arrListu.Add(smth);
                    }
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start;
                AverageAddingForArrList += testArrListu;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
            AverageAddingForList = new TimeSpan(AverageAddingForList.Ticks / testu.Length);
            AverageAddingForArrList = new TimeSpan(AverageAddingForArrList.Ticks / testu.Length);
        }

        static void TestListAccessOrderFor(int[] testu)
        {
            Console.WriteLine("Testuję dostęp po kolei (for):");
            Console.WriteLine($"{"List",20} {"ArrayList",20}");
            List<int> listu = new List<int>();
            foreach (int smth in testu)
            {
                listu.Add(smth);
            }
            ArrayList arrListu = new ArrayList();
            foreach (int smth in testu)
            {
                arrListu.Add(smth);
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i=0; i<listu.Count; i++)
                    {
                        nevermind = listu[i];
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < arrListu.Count; i++)
                    {
                        nevermind = (int)arrListu[i];
                    }
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }
        static void TestListAccessOrderForEach(int[] testu)
        {
            Console.WriteLine("Testuję dostęp po kolei (foreach):");
            Console.WriteLine($"{"List",20} {"ArrayList",20}");
            List<int> listu = new List<int>();
            foreach (int smth in testu)
            {
                listu.Add(smth);
            }
            ArrayList arrListu = new ArrayList();

            foreach (int smth in testu)
            {
                arrListu.Add(smth);
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    foreach (Object smth in listu)
                        nevermind = (int)smth;
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    foreach (Object smth in arrListu)
                        nevermind = (int)smth;
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }
        static void TestListAccessRandomOrder(int[] testu, int[] order)
        {
            Console.WriteLine("Testuję dostęp w losowej kolejności:");
            foreach (int smth in order) Console.Write($"{smth} ");
            Console.WriteLine();
            Console.WriteLine($"{"List",20} {"ArrayList",20}");
            List<int> listu = new List<int>();
            foreach (int smth in testu)
            {
                listu.Add(smth);
            }
            ArrayList arrListu = new ArrayList();
            foreach (int smth in testu)
            {
                arrListu.Add(smth);
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < listu.Count; i++)
                    {
                        nevermind = listu[order[i]];
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < arrListu.Count; i++)
                    {
                        nevermind = (int)arrListu[order[i]];
                    }
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }

        static void TestListRemoveOrder(int[] testu)
        {
            Console.WriteLine("Testuję usuwanie po kolei:");
            Console.WriteLine();
            if (AverageAddingForList == TimeSpan.Zero)
            {
                Console.WriteLine("Test dodawania (for) potrzebny, by ustalić średni czas i pomniejszyć o niego wszystkie testy usuwania, przeprowadzam...");
                TextWriter stdout = Console.Out;
                Console.SetOut(TextWriter.Null);
                TestListAddingFor(testu);
                Console.SetOut(stdout);
            }
            Console.WriteLine($"{"List",20} {"ArrayList",20}");

            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    List<int> listu = new List<int>();
                    foreach (int smth in testu)
                    {
                        listu.Add(smth);
                    }
                    int i=0;
                    while (listu.Any())
                    {
                        listu.Remove(testu[i]);
                        i++;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start - AverageAddingForList;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    ArrayList arrListu = new ArrayList();
                    foreach (int smth in testu)
                    {
                        arrListu.Add(smth);
                    }
                    int i = 0;
                    while (arrListu.Count!=0)
                    {
                        arrListu.Remove(testu[i]);
                        i++;
                    }
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start - AverageAddingForArrList;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }
        static void TestListRemoveRandomOrder(int[] testu, int[] order)
        {
            Console.WriteLine("Testuję usuwanie w losowej kolejności:");
            foreach (int smth in order) Console.Write($"{smth} ");
            Console.WriteLine();
            if (AverageAddingForList == TimeSpan.Zero)
            {
                Console.WriteLine("Test dodawania (for) potrzebny, by ustalić średni czas i pomniejszyć o niego wszystkie testy usuwania, przeprowadzam...");
                TextWriter stdout = Console.Out;
                Console.SetOut(TextWriter.Null);
                TestListAddingFor(testu);
                Console.SetOut(stdout);
            }
            Console.WriteLine($"{"List",20} {"ArrayList",20}");

            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    List<int> listu = new List<int>();
                    foreach (int smth in testu)
                    {
                        listu.Add(smth);
                    }
                    int i = 0;
                    while (listu.Any())
                    {
                        listu.Remove(testu[order[i]]);
                        i++;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testListu = end - start - AverageAddingForList;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    ArrayList arrListu = new ArrayList();
                    foreach (int smth in testu)
                    {
                        arrListu.Add(smth);
                    }
                    int i = 0;
                    while (arrListu.Count != 0)
                    {
                        arrListu.Remove(testu[order[i]]);
                        i++;
                    }
                }
                end = DateTime.Now;
                TimeSpan testArrListu = end - start - AverageAddingForArrList;
                Console.WriteLine($"{testListu,20} {testArrListu,20}");
            }
        }

        static void TestHashAdding(int[] testu)
        {
            Console.WriteLine("Testuję dodawanie:");
            Console.WriteLine($"{"Hashtable",20} {"Dictionary",20}");
            AverageAddingHash = TimeSpan.Zero;
            AverageAddingDict = TimeSpan.Zero;
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Hashtable hash = new Hashtable();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        hash.Add(smth, ind);
                        ind++;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testHash = end - start;
                AverageAddingHash += testHash;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        dict.Add(smth, ind);
                        ind++;
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                AverageAddingDict += testDict;
                Console.WriteLine($"{testHash,20} {testDict,20}");
            }
            AverageAddingDict = new TimeSpan(AverageAddingDict.Ticks / testu.Length);
            AverageAddingHash = new TimeSpan(AverageAddingHash.Ticks / testu.Length);
        }

        static void TestHashAccessOrderFor(int[] testu)
        {
            Console.WriteLine("Testuję dostęp po kolei (for):");
            Console.WriteLine($"{"Hashtable",20} {"dictionary",20}");
            Hashtable hash = new Hashtable();
            int ind = 0;
            foreach (int smth in testu)
            {
                hash.Add(smth, ind);
                ind++;
            }
            Dictionary<int, int> dict = new Dictionary<int, int>();
            ind = 0;
            foreach (int smth in testu)
            {
                dict.Add(smth, ind);
                ind++;
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < hash.Count; i++)
                    {
                        nevermind = (int)hash[i];
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testHash = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < dict.Count; i++)
                    {
                        nevermind = (int)dict[i];
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                Console.WriteLine($"{testHash,20} {testDict,20}");
            }
        }
        static void TestHashAccessOrderForEach(int[] testu)
        {
            Console.WriteLine("Testuję dostęp po kolei (foreach):");
            Console.WriteLine($"{"Hashtable",20} {"dictionary",20}");
            Hashtable hash = new Hashtable();
            int ind = 0;
            foreach (int smth in testu)
            {
                hash.Add(smth, ind);
                ind++;
            }
            Dictionary<int, int> dict = new Dictionary<int, int>();
            ind = 0;
            foreach (int smth in testu)
            {
                dict.Add(smth, ind);
                ind++;
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    foreach (DictionaryEntry entry in hash)
                    {
                        nevermind = (int)entry.Value;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testHash = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    foreach (KeyValuePair<int,int> entry in dict)
                    {
                        nevermind = entry.Value;
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                Console.WriteLine($"{testHash,20} {testDict,20}");
            }
        }
        static void TestHashAccessRandomOrder(int[] testu, int[] order)
        {
            Console.WriteLine("Testuję dostęp w losowej kolejności:");
            foreach (int smth in order) Console.Write($"{smth} ");
            Console.WriteLine();
            Console.WriteLine($"{"Hashtable",20} {"dictionary",20}");
            Hashtable hash = new Hashtable();
            int ind = 0;
            foreach (int smth in testu)
            {
                hash.Add(smth, ind);
                ind++;
            }
            Dictionary<int, int> dict = new Dictionary<int, int>();
            ind = 0;
            foreach (int smth in testu)
            {
                dict.Add(smth, ind);
                ind++;
            }
            for (int t = 0; t < LiczbaTestow; t++)
            {

                DateTime start = DateTime.Now;
                int nevermind;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < hash.Count; i++)
                    {
                        nevermind = (int)hash[order[i]];
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan TestHash = end - start;

                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    for (int i = 0; i < dict.Count; i++)
                    {
                        nevermind = dict[order[i]];
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                Console.WriteLine($"{TestHash,20} {testDict,20}");
            }
        }

        static void TestHashRemoveOrder(int[] testu)
        {
            Console.WriteLine("Testuję usuwanie po kolei:");
            if (AverageAddingHash == TimeSpan.Zero)
            {
                Console.WriteLine("Test dodawania (for) potrzebny, by ustalić średni czas i pomniejszyć o niego wszystkie testy usuwania, przeprowadzam...");
                TextWriter stdout = Console.Out;
                Console.SetOut(TextWriter.Null);
                TestHashAdding(testu);
                Console.SetOut(stdout);
            }
            Console.WriteLine($"{"Hashtable",20} {"dictionary",20}");
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Hashtable hash = new Hashtable();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        hash.Add(smth, ind);
                        ind++;
                    }
                    ind = 0;
                    while (hash.Count != 0)
                    {
                        hash.Remove(testu[ind]);
                        ind++;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testHash = end - start;
                AverageAddingHash += testHash;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        dict.Add(smth, ind);
                        ind++;
                    }
                    ind = 0;
                    while (dict.Count != 0)
                    {
                        dict.Remove(testu[ind]);
                        ind++;
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                AverageAddingDict += testDict;
                Console.WriteLine($"{testHash,20} {testDict,20}");
            }
        }
        static void TestHashRemoveRandomOrder(int[] testu, int[] order)
        {
            Console.WriteLine("Testuję usuwanie w losowej kolejności:");
            foreach (int smth in order) Console.Write($"{smth} ");
            Console.WriteLine();
            if (AverageAddingHash == TimeSpan.Zero)
            {
                Console.WriteLine("Test dodawania (for) potrzebny, by ustalić średni czas i pomniejszyć o niego wszystkie testy usuwania, przeprowadzam...");
                TextWriter stdout = Console.Out;
                Console.SetOut(TextWriter.Null);
                TestHashAdding(testu);
                Console.SetOut(stdout);
            }
            Console.WriteLine($"{"Hashtable",20} {"dictionary",20}");
            for (int t = 0; t < LiczbaTestow; t++)
            {
                DateTime start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Hashtable hash = new Hashtable();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        hash.Add(smth, ind);
                        ind++;
                    }
                    ind = 0;
                    while (hash.Count != 0)
                    {
                        hash.Remove(testu[order[ind]]);
                        ind++;
                    }
                }
                DateTime end = DateTime.Now;
                TimeSpan testHash = end - start;
                AverageAddingHash += testHash;
                start = DateTime.Now;
                for (int p = 0; p < LiczbaProb; p++)
                {
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    int ind = 0;
                    foreach (int smth in testu)
                    {
                        dict.Add(smth, ind);
                        ind++;
                    }
                    ind = 0;
                    while (dict.Count != 0)
                    {
                        dict.Remove(testu[order[ind]]);
                        ind++;
                    }
                }
                end = DateTime.Now;
                TimeSpan testDict = end - start;
                AverageAddingDict += testDict;
                Console.WriteLine($"{testHash,20} {testDict,20}");
            }
        }

        static void Main(string[] args)
        {


            int[] testu = { 1, 3, 6, 8, 1023, 255, 1000232, 1123412, 1232, 2323, 3939, 10202, 938, 9292, 42, 1337, 666 };
            int[] indexes = new int[testu.Length];
            for (int i=0; i < indexes.Length; i++)
            {
                indexes[i] = i;
            }
            Random rnd = new Random();
            indexes = indexes.OrderBy(x => rnd.Next()).ToArray();

            //Odkomentować wywołania, które chcemy zobaczyć

            //LISTY:

            //Testowanie dodawania po jednym elemencie, zbiorczo
            //TestListAddingFor(testu);
            //TestListAddingRange(testu);

            //Testowanie dostępu po kolei forem, po kolei foreachem, lub w losowej kolejności
            //TestListAccessOrderFor(testu);
            //TestListAccessOrderForEach(testu);
            //TestListAccessRandomOrder(testu, indexes);

            //Testowanie usuwania w kolejności takiej, jak dodano, lub w losowej kolejności
            //TestListRemoveOrder(testu);
            //TestListRemoveRandomOrder(testu, indexes);

            //HASHTABLICA/SŁOWNIK
            
            //Testowanie dodawania. Wartości z tablicy testu są kluczami, ich indeksy wartościami.
            //TestHashAdding(testu);

            //Testowanie dostępu po kolei forem, po kolei foreachem, lub w losowej kolejności
            //TestHashAccessOrderFor(testu);
            //TestHashAccessOrderForEach(testu);
            //TestHashAccessRandomOrder(testu, indexes);

            //TestHashRemoveOrder(testu);
            //TestHashRemoveRandomOrder(testu,indexes);
            Console.ReadKey();

        }
    }
}


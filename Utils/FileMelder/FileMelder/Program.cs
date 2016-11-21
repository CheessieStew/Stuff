using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FileMelder
{
    class FileMeld 
    {


        static void Main(string[] args)
        {
            String[] files = Directory.
                GetFiles(Directory.GetCurrentDirectory()).
                Where(s => s.EndsWith($".cs", StringComparison.OrdinalIgnoreCase)).ToArray();
            Console.Error.WriteLine($"Found {files.Length} .cs files to meld");
            StreamReader[] streams = new StreamReader[files.Length];
            List<String> usingClauses = new List<String>();
            String[] readLines = new String[files.Length];
            for(int i = 0; i < streams.Length; i++)
            {
                streams[i] = new StreamReader(files[i]);
                String[] split = null;
                int j = 0;
                do
                {
                    readLines[i] = streams[i].ReadLine();
                    if (readLines[i] == null)
                        split = null;
                    else
                        split = readLines[i].Split(' ');
                    if (split[0] == "using" && !usingClauses.Contains(split[1]))
                    {
                        usingClauses.Add(split[1]);
                    }
                    j++;
                } while (split != null && split[0] == "using");
            }
            foreach (String usingClause in usingClauses)
                Console.WriteLine($"using {usingClause}");
            for (int i = 0; i < streams.Length; i++)
            {
                while (readLines[i] != null) 
                {
                    Console.WriteLine(readLines[i]);
                    readLines[i] = streams[i].ReadLine();
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zad2
{
    public class ReportDataProvider
    {
        public string GetData()
        {
            return Console.ReadLine();
        }
    }

    public class DocumentFormatter
    {
        public string FormatDocument(string s)
        {
            return s.ToUpper();
        }
    }

    public class ReportPrinter2
    {
        public void PrintReport(string doc)
        {
            Console.WriteLine(doc);
        }
    }


    public class ReportPrinter1
    {
        public string unformattedDocument;
        string documentToBePrinted;

        public string GetData()
        {
            return Console.ReadLine();
        }

        public void FormatDocument()
        {
            documentToBePrinted = unformattedDocument.ToUpper();
        }

        public void PrintReport()
        {
            if (documentToBePrinted != null)
                Console.WriteLine(documentToBePrinted);
            else Console.WriteLine("No formatted document awaiting");
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            ReportDataProvider provider = new ReportDataProvider();
            DocumentFormatter formatter = new DocumentFormatter();
            ReportPrinter2 printer2 = new ReportPrinter2();
            

            ReportPrinter1 printer1 = new ReportPrinter1();

            while (true)
            {
                printer2.PrintReport(formatter.FormatDocument(provider.GetData()));

                printer1.unformattedDocument = printer1.GetData();
                printer1.FormatDocument();
                printer1.PrintReport();

            }
            Console.ReadKey();
        }
    }
}

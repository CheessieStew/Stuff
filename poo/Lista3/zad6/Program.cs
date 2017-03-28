using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zad6
{
    interface IDataProvider
    {
        string GetData();
    }

    class ConsoleDataProvider : IDataProvider
    {
        public string GetData() => Console.ReadLine();
    }

    interface IDocumentFormatter
    {
        string FormatDocument(string document);
    }

    interface IReportPrinter
    {
        void PrintReport(string report);
    }

    class ConsoleReportPrinter : IReportPrinter
    {
        public void PrintReport(string report)
        {
            Console.WriteLine(report);
        }
    }

    class DelegateFormatter : IDocumentFormatter
    {
        public delegate string FormattingFunction(string s);
        public FormattingFunction Formatter;

        public string FormatDocument(string document)
        {
            return Formatter(document);
        }
    }

    class ReportComposer : IDataProvider, IDocumentFormatter, IReportPrinter
    {
        public IDataProvider provider;
        public IDocumentFormatter formatter;
        public IReportPrinter printer;

        public string GetData() => provider.GetData();
        public string FormatDocument(string document) => formatter.FormatDocument(document);
        public void PrintReport(string report) => printer.PrintReport(report);

        public void ComposeReport() => PrintReport(FormatDocument(GetData()));
    }

    class Program
    {
        static void Main(string[] args)
        {
            ReportComposer rp = new ReportComposer()
            {
                provider = new ConsoleDataProvider(),
                formatter = new DelegateFormatter()
                {
                    Formatter = (s => s.ToUpper())
                },
                printer = new ConsoleReportPrinter()
            };
            while (true)
                rp.ComposeReport();
        }
    }
}

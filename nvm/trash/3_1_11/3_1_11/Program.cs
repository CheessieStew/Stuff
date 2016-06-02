using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_1_11
{
    class Program
    { 

        static void Main(string[] args)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\n\npublic class Powidelko { public static void DoSomething(){\n");
            Console.WriteLine("Dej mnie ciało statycznej funkcji bezargumentowej typu void, \"---\" kończy.");
            Console.WriteLine("Zaimportowane jest tylko System");
            String a = Console.ReadLine();
            while (a != "---")
            {
                sb.Append(a);
                a = Console.ReadLine();
            }
            sb.Append("}}");
            CompilerResults res = codeProvider.CompileAssemblyFromSource(
                new CompilerParameters()
                {
                    GenerateInMemory = true
                },
                sb.ToString());
            if (res.Errors.HasErrors)
            {
                Console.WriteLine("Bledy!");
                foreach (var error in res.Errors)
                    Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Wykonuje kod:");
                try
                {
                   
                    var type = res.CompiledAssembly.GetType("Powidelko");
                    type.GetMethod("DoSomething").Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

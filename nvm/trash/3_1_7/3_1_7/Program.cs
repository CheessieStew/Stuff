using System;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Threading;
using System.Messaging;
using System.Diagnostics;
using System.Linq;

namespace _3_1_7
{
    class Sender
    {

        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("_3_1_7.Receiver.txt"));
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters comParams = new CompilerParameters()
            {
                GenerateExecutable = true,
                OutputAssembly = "receiver.exe"
            };
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .ToArray();

            comParams.ReferencedAssemblies.AddRange(references);

            CompilerResults res = codeProvider.CompileAssemblyFromSource(
                comParams,
                reader.ReadToEnd());

            if (res.Errors.HasErrors)
            {
                foreach (var error in res.Errors)
                {
                    Console.WriteLine(error);
                }

            }
            else
            {
                Process.Start("receiver.exe");

                int i = 0;
                MessageQueue.Create(@".\PRIVATE$\myqueue2");
                MessageQueue queue = new MessageQueue(@"FormatName:DIRECT=OS:.\PRIVATE$\myqueue2");
                queue.SetPermissions("Everyone", MessageQueueAccessRights.ReceiveMessage);
                queue.SetPermissions("Everyone", MessageQueueAccessRights.WriteMessage);

                while (true)
                {

                    Message msg = new Message();
                    msg.Formatter = new XmlMessageFormatter(new Type[1] { typeof(string) });
                    msg.Body = $"Wiadomość jakaś taka sobie {i}";
                    msg.Recoverable = true;
                     queue.Send(msg);
                    i++;
                    if (i % 20 == 0)
                    {
                        Console.WriteLine("Wyslano 20 wiadomosci");
                        Thread.Sleep(10000);
                    }
                }
            }

        }

    }
}


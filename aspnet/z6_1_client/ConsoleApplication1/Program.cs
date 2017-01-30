using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ConsoleApplication1.ServiceReference1;
using System.Xml;
using System.Net.Sockets;

class Program
{
    static readonly string host = "localhost";
    static readonly string addr = @"/Service.asmx";
    static readonly string theMethod = "SayLouder";
    static void Main(string[] args)
    {
        TcpClient c = new TcpClient(host, 80);
        Stream tcpStream = c.GetStream();
        StreamWriter writer = new StreamWriter(tcpStream, Encoding.Default);
        while (true)
        {

            string data = MakeSoap(theMethod, new Tuple<String, String>[]
            {
               new Tuple<string, string>("str",Console.ReadLine())
            });
            string headers = $"\r\nPOST {addr} HTTP/1.1\r\n"
            + $"Host: {host}\r\n"
            + "Content-Type: text/xml; charset=utf-8\r\n"
            + $"SOAPAction: \"http://tempuri.org/{theMethod}\"\r\n"
            + $"Content-Length = {data.Length}\r\n\r\n";

            Console.Write(headers);
            Console.Write(data);

            writer.Write(headers);
            writer.Write(data);
            writer.Flush();

            Console.WriteLine(new StreamReader(tcpStream).ReadToEnd());

            //Console.WriteLine($"Response: {FindAnswer(tcpStream, theMethod)}");


        }
    }

    static string FindAnswer(Stream stream, string methodname)
    {
        string searched = $"{methodname}Result";
        XmlTextReader reader = new XmlTextReader(stream);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element
                && reader.Name == searched)
            {
                if (reader.Read() && reader.NodeType==XmlNodeType.Text)
                    return reader.Value;
            }

        }
        return "No answer";
    }

    static string MakeSoap(string MethodName,
        IEnumerable<Tuple<String, String>> values)
    {
        StringBuilder res = new StringBuilder();
        res.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        res.AppendLine();
        res.Append("<soap:Envelope ");
        res.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
        res.Append("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ");
        res.Append("xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
        res.AppendLine();
        res.Append("  <soap:Body>");
        res.AppendLine();
        res.Append($"    <{MethodName} xmlns=\"http://tempuri.org/\">");
        res.AppendLine();

        foreach (var pair in values)
        {
            res.Append($"      <{pair.Item1}>{pair.Item2}</{pair.Item1}>");
            res.AppendLine();
        }
        res.Append($"    </{MethodName}>");
        res.AppendLine();

        res.Append("  </soap:Body>");
        res.AppendLine();
        res.Append("</soap:Envelope>");
        res.AppendLine();
        return res.ToString();
    }
}

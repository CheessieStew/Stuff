using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;
namespace _3_1_6
{
    [Serializable()]
    class SomeObject : ISerializable
    {
        public int wierszykLen
        {
            get
            {
                return wierszyk.Length;
            }
            private set
            {
            }
        }

        public String wierszyk;
        public ConsoleColor wierszykColor;

        public SomeObject()
        {
            wierszyk = "Nie mam na kebaba";
            wierszykColor = ConsoleColor.Blue;
        }

        public SomeObject(SerializationInfo info, StreamingContext context)
        {
            wierszyk = (String)info.GetValue("wierszyk", typeof(String));
            wierszykColor = (ConsoleColor)info.GetValue("wierszykColor", typeof(ConsoleColor));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("wierszyk", wierszyk);
            info.AddValue("wierszykColor", wierszykColor);
        }

        public void Wierszuj()
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = wierszykColor;
            Console.WriteLine(wierszyk);
            Console.ForegroundColor = old;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Thread server = new Thread(Server);
            server.Start();
            Client();
        }

        static void Client()
        {
            int i = 0;
            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
            foreach (ConsoleColor color in colors)
            {
                i++;
                SomeObject obj = new SomeObject();
                obj.wierszyk = $"Nie mam na kebaba{i}";
                obj.wierszykColor = color;
                TcpClient me = new TcpClient("127.0.0.1", 1337);
                BinaryFormatter form = new BinaryFormatter();
                form.Serialize(me.GetStream(), obj);
                me.Close();
                Thread.Sleep(500);
            }
        }

        static void Server()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback,1337);
            listener.Start();
            while(true)
            {
                Socket client = listener.AcceptSocket();
                byte[] buffer = new byte[4086];
                BinaryFormatter form = new BinaryFormatter();
                SomeObject obj = (SomeObject)form.Deserialize(new NetworkStream(client));
                Console.WriteLine("Serwer dostał obiekt");
                obj.Wierszuj();
                client.Close();
            }
        }
    }
}

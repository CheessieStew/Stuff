using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace zad2
{
    class CaesarStream : Stream
    {
        Stream wrapped;
        int shift;

        public CaesarStream(Stream stream, int shift)
        {
            wrapped = stream;
            this.shift = shift;
        }

        public override bool CanRead
        {
            get
            {
                return wrapped.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return wrapped.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return wrapped.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return wrapped.Length;
            }
        }

        public override long Position
        {
            get
            {
                return wrapped.Position;
            }

            set
            {
                wrapped.Position = value;
            }
        }

        public override void Flush()
        {
            wrapped.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var res = wrapped.Read(buffer, offset, count);
            for (int i = 0; i < buffer.Length; i++)
            {
                if(!char.IsControl((char)buffer[i]))
                    buffer[i] = (byte)(buffer[i] + shift);
            }
            return res;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return wrapped.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            wrapped.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            for (int i = 0; i < data.Length; i++)
            {
                if (!char.IsControl((char)buffer[i]))
                    data[i] = (byte)(data[i] + shift);
            }
                
            wrapped.Write(data, offset, count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var caeOut = new StreamWriter(new CaesarStream(Console.OpenStandardOutput(),2));
            caeOut.AutoFlush = true;
            Console.SetOut(caeOut);
            var caeIn = new StreamReader(new CaesarStream(Console.OpenStandardInput(), 2));
            Console.SetIn(caeIn);
            Console.WriteLine("Kotecek lol");
            Console.WriteLine(Console.ReadLine());
            
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zad3
{

    namespace Shapeful
    {
        interface IShape
        {
            float Area { get; }
        }

        public class Rectangle : IShape
        {
            public float Width { get; set; }
            public float Height { get; set; }

            public static implicit operator Square(Rectangle rect)
            {
                if (rect.Width == rect.Height)
                    return new Square(rect.Width);
                else return null;
            }
            public float Area => Width * Height;
        }

        public class Square : IShape
        {
            public float Side;

            public Square(float side)
            {
                Side = side;
            }

            public float Area => Side * Side;
        }

        public static class Example
        {
            public static void Run()
            {
                int w = 4;
                IShape square = new Square(w);
            }
        }
    }
    

    namespace ExampleCode
    {
        public class Rectangle
        {
            public virtual int Width { get; set; }
            public virtual int Height { get; set; }
        }
        public class Square : Rectangle
        {
            public override int Width
            {
                get { return base.Width; }
                set { base.Width = base.Height = value; }
            }
            public override int Height
            {
                get { return base.Height; }
                set { base.Width = base.Height = value; }
            }
        }

        public class AreaCalculator
        {
            public int CalculateArea(Rectangle rect)
            {
                return rect.Width * rect.Height;
            }
        }

        public static class Example
        {
            public static void Run()
            {
                int w = 4, h = 5;
                Rectangle rect = new Square { Width = w, Height = h };
                AreaCalculator calc = new AreaCalculator();
                Console.WriteLine("prostokąt o wymiarach {0} na {1} ma pole {2}",
                                    w, h, calc.CalculateArea(rect));
            }


        }
    }


    public static class Program
    {

        static void Main(string[] args)
        {
            ExampleCode.Example.Run();
            Shapeful.Example.Run();

            Console.ReadKey();
        }
    }
}
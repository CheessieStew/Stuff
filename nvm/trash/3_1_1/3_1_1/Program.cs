using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
namespace _3_1_1
{
    class Complex : IFormattable
    {
        public double real;
        public double imaginary;

        public Complex(double a, double b)
        {
            real = a; imaginary = b;
        }

        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary);
        }
        public static Complex operator +(Complex c1, Double c2)
        {
            return (c1 + new Complex(c2, 0));
        }
        public static Complex operator +(Double c1, Complex c2)
        {
            return (new Complex(c1, 0) + c2);
        }

        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.real - c2.real, c1.imaginary - c2.imaginary);
        }
        public static Complex operator -(Complex c1, Double c2)
        {
            return (c1 + new Complex(c2, 0));
        }
        public static Complex operator -(Double c1, Complex c2)
        {
            return (new Complex(c1, 0) + c2);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.real * c2.real - c1.imaginary * c2.imaginary, c1.real * c2.imaginary + c1.imaginary * c1.real);
        }
        public static Complex operator *(Complex c1, Double c2)
        {
            return (c1 * new Complex(c2, 0));
        }
        public static Complex operator *(Double c1, Complex c2)
        {
            return (new Complex(c1, 0) * c2);
        }

        public static Complex operator /(Complex c1, Complex c2)
        {
            double len = c2.real * c2.real + c2.imaginary * c2.imaginary;
            return new Complex((c1.real * c2.real + c1.imaginary * c2.imaginary)/len,
                               (c1.imaginary * c2.real - c1.real * c2.imaginary)/len);
        }
        public static Complex operator /(Complex c1, Double c2)
        {
            return (c1 / new Complex(c2, 0));
        }
        public static Complex operator /(Double c1, Complex c2)
        {
            return (new Complex(c1, 0) / c2);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;
            if (format == null) format = "d";
            switch(format.ToLowerInvariant())
            {
                case "d":
                    return real.ToString(formatProvider) + " + " + imaginary.ToString(formatProvider) + "i";
                case "w":
                    return "[" + real.ToString(formatProvider) + "," + imaginary.ToString(formatProvider) + "]";
                default:
                    throw new FormatException();
            }
        }

        public override string ToString()
        {
            return this.ToString("d", CultureInfo.CurrentCulture);
        }
        
        public string ToString(string format)
        {
            return this.ToString(format, CultureInfo.CurrentCulture);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Complex a = new Complex(4, 3);
            Complex b = new Complex(1, 2);
            Console.WriteLine($"{a} + {b} = {a + b}");
            Console.WriteLine($"{a:d} + {b:d} = {a + b:d}");
            Console.WriteLine($"{a:w} + {b:w} = {a + b:w}");

        }
    }
}

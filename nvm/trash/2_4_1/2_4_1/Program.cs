using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_4_1
{
    static class StringExt
    {
        public static bool ispalindrome(this string str)
        {
            int lo = 0;
            int hi = str.Length - 1;
            while (lo <= hi)
            {
                if (!char.IsLetterOrDigit(str[lo]))
                {
                    lo++;
                    continue;
                }
                if (!char.IsLetterOrDigit(str[hi]))
                {
                    hi--;
                    continue;
                }
                if (char.ToLower(str[lo]) != char.ToLower(str[hi]))
                {
                    return false;
                }
                lo++;
                hi--;
            }
            return true;
        }

        public static string ispalindrome2(this string str)
        {
            if (str.ispalindrome())
            {
                return "yup";
            }
            return "nope";
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            string[] testu =
            {
                "Kobyła ma mały bok.",
                "Ikar łapał raki.",
                "Nie mam na kebaba.",
                "Kajak"
            };
            testu.ToList().ForEach(str => Console.WriteLine($"\"{str}\" - {str.ispalindrome2()}"));
        }
    }
}

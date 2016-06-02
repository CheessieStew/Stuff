using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3._1._2
{
    class MySet : ArrayList
    {
        public MySet() : base()
        {
        }

        public MySet(ICollection c) : base()
        {
            AddRange(c);
        }

        public override void AddRange(ICollection c)
        {
            foreach (object o in c)
            {
                if (!Contains(o))
                    Add(o);
            }
        }

        public override int Add(object value)
        {
            if (!Contains(value))
                return base.Add(value);
            else return -1;
        }

        static void Main(string[] args)
        {
            int[] testu = new int[] { 1, 1, 5, 5, 6, 6, 4, 5, 3, 3, 7, 5, 6 };
            MySet set1 = new MySet(testu);
            MySet set2 = new MySet();
            set2.AddRange(testu);
            foreach (object o in set1)
                Console.WriteLine(o);
            Console.WriteLine();
            foreach (object o in set2)
                Console.WriteLine(o);
        }
    }
}

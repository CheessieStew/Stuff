using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;

namespace poo_3
{
    class ClassicSingleton
    {
        private static object _key = new object();
        private static ClassicSingleton _instance;
        public static ClassicSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_key)
                    {
                        if (_instance == null)
                            _instance = new ClassicSingleton();
                    }
                }
                return _instance;
            }
        }
    }

    class SingletonFor5Seconds
    {
        private static object _key = new object();
        private static DateTime _validThru;
        private static SingletonFor5Seconds _instance;
        public static SingletonFor5Seconds Instance
        {
            get
            {
                if (_instance == null || _validThru <= DateTime.Now)
                {
                    lock (_key)
                    {
                        if (_instance == null || _validThru <= DateTime.Now)
                        {
                            _instance = new SingletonFor5Seconds();
                            _validThru = DateTime.Now.AddSeconds(5);
                        }
                    }
                }
                return _instance;
            }
        }
    }

    class ThreadSpecificSingleton
    {
        private static Dictionary<Thread, ThreadSpecificSingleton> _instances
            = new Dictionary<Thread, ThreadSpecificSingleton>();
        public static ThreadSpecificSingleton Instance
        {
            get
            {
                if (!_instances.ContainsKey(Thread.CurrentThread)
                    || _instances[Thread.CurrentThread] == null)
                {
                    _instances.Add(Thread.CurrentThread, new ThreadSpecificSingleton());
                }
                return _instances[Thread.CurrentThread];
            }
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void ClassicSingletonTest()
        {
            var a = ClassicSingleton.Instance;
            var b = ClassicSingleton.Instance;
            Assert.AreSame(a, b);
        }

        [Test]
        public void Singleton5SecondsTest()
        {
            var a = SingletonFor5Seconds.Instance;
            var b = SingletonFor5Seconds.Instance;
            Assert.AreSame(a, b, "1nd assertion failed");
            Thread.Sleep(5001);
            var c = SingletonFor5Seconds.Instance;
            Assert.AreNotSame(a, c, "2nd assertion failed");
        }


        void refAdder(object target) => (target as List<object>).Add(ThreadSpecificSingleton.Instance);

        [Test]
        public void ThreadSingletonTest()
        {
            var refs = new List<object>();
            
            Thread t1 = new Thread(new ParameterizedThreadStart(refAdder));
            Thread t2 = new Thread(new ParameterizedThreadStart(refAdder));
            t1.Start(refs);
            t2.Start(refs);
            t1.Join();
            t2.Join();
            Assert.AreNotSame(refs[0], refs[1]);

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var t = new Tests();
            t.ThreadSingletonTest();
        }
    }
}

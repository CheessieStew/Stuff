using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace poo_4_3
{
    public class ReferenceComparer<T> : EqualityComparer<T> where T : class
    {
        public override bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }


    public class Airplane
    {
        public bool IsAlive
        {
            get
            {
                return home.Contains(this);
            }
        }

        public int Id
        {
            get
            {
                return home.PlaneID(this);
            }
        }
        private Airport home;

        private Airplane(Airport airport)
        {
            home = airport;
        }

        public class Airport
        {
            public int Count
            {
                get
                {
                    return registry.Count;
                }
            }
            private int NextId;
            public int Capacity { get; private set; }
            Dictionary<Airplane, int> registry;

            public int PlaneID(Airplane plane)
            {
                return registry[plane];
            }

            public bool Contains(Airplane plane)
            {
                return registry.ContainsKey(plane);
            }

            public Airplane Dispense()
            {
                if (Count < Capacity)
                {
                    var res = new Airplane(this);
                    registry.Add(res, NextId);
                    NextId++;
                    return res;
                }
                return null;
            }

            public void Recycle(Airplane plane)
            {
                registry.Remove(plane);
            }

            public Airport(int capacity)
            {
                Capacity = capacity;
                registry = new Dictionary<Airplane, int>(Capacity, new ReferenceComparer<Airplane>());
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void DispenseTest()
        {
            var port = new Airplane.Airport(5);
            var plane = port.Dispense();
            Assert.IsNotNull(plane);
        }

        [Test]
        public void OverflowTest()
        {
            var port = new Airplane.Airport(5);
            var planes = new List<Airplane>();
            while (port.Count < port.Capacity)
            {
                planes.Add(port.Dispense());
                Assert.IsNotNull(planes.Last());
            }
            var p = port.Dispense();
            Assert.IsNull(p);
        }

        [Test]
        public void ReturnTest()
        {
            var port = new Airplane.Airport(5);
            var planes = new List<Airplane>();
            while (port.Count < port.Capacity)
            {
                planes.Add(port.Dispense());
                Assert.IsNotNull(planes.Last());
            }
            var p = port.Dispense();
            Assert.IsNull(p);
            port.Recycle(planes.Last());
            Assert.IsFalse(planes.Last().IsAlive);
            p = port.Dispense();
            Assert.IsNotNull(p);
            var p2 = port.Dispense();
            Assert.IsNull(p2);
            port.Recycle(p);
            p2 = port.Dispense();
            Assert.IsNotNull(p2);
        }
    }
}

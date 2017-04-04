using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace poo_4_2
{
    public static class NumExtension
    {
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }


    public interface IShapeFactoryWorker
    {
        bool CanCreate(string name);
        IShape CreateShape(string name, params object[] parameters);
    }

    public interface IShape
    {
        float Area { get; }
    }

    public class NullShape : IShape
    {
        public float Area
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class Square : IShape
    {
        public float Side;
        public float Area
        {
            get
            {
                return Side * Side;
            }
        }

        public Square(float side)
        {
            Side = side;
        }
    }

    public class SquareCreator : IShapeFactoryWorker
    {
        public bool CanCreate(string name)
        {
            return name.ToLowerInvariant() == "square";
        }

        public IShape CreateShape(string name, params object[] parameters)
        {

            if (CanCreate(name) && parameters.Length >= 1 && parameters[0].IsNumber())
            {
                return new Square(Convert.ToSingle(parameters[0]));
            }
            return new NullShape();
        }
    }

    public class Circle : IShape
    {
        public float Radius;
        public float Area
        {
            get
            {
                return (float)(Radius * Radius * Math.PI);
            }
        }

        public Circle(float r)
        {
            Radius = r;
        }
    }

    public class CircleCreator : IShapeFactoryWorker
    {
        public bool CanCreate(string name)
        {
            return name.ToLowerInvariant() == "circle";
        }

        public IShape CreateShape(string name, params object[] parameters)
        {
            if (CanCreate(name) && parameters.Length >= 1 && parameters[0].IsNumber())
            {
                return new Circle(Convert.ToSingle(parameters[0]));
            }
            return new NullShape();
        }
    }

    public class Rectangle : IShape
    {
        public float Side1, Side2;
        public float Area
        {
            get
            {
                return Side1 * Side2;
            }
        }

        public Rectangle(float side1, float side2)
        {
            Side1 = side1;
            Side2 = side2;
        }
    }

    public class RectangleCreator : IShapeFactoryWorker
    {
        public bool CanCreate(string name)
        {
            return name.ToLowerInvariant() == "rectangle";
        }

        public IShape CreateShape(string name, params object[] parameters)
        {
            if (CanCreate(name) && parameters.Length >= 2 
                && parameters[0].IsNumber() && parameters[1].IsNumber())
            {
                return new Rectangle(Convert.ToSingle(parameters[0]), Convert.ToSingle(parameters[1]));
            }
            return new NullShape();
        }
    }

    public class ShapeFactory
    {
        private List<IShapeFactoryWorker> _workers = new List<IShapeFactoryWorker>();
        public void RegisterWorker(IShapeFactoryWorker worker)
        {
            if (worker != null)
                _workers.Insert(0, worker);
            else
                throw new ArgumentNullException("worker");
        }

        public ShapeFactory()
        {
            RegisterWorker(new SquareCreator());
            RegisterWorker(new CircleCreator());
        }

        public IShape CreateShape(string name, params object[] parameters)
        {
            foreach(var worker in _workers)
            {
                if (worker.CanCreate(name))
                    return worker.CreateShape(name, parameters);
            }
            return new NullShape();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int somenum = 1;
            object smth = somenum;
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void SquareTest()
        {
            var f = new ShapeFactory();
            for (int i = 0; i < 100; i++)
            {
                var sq = f.CreateShape("square", i);
                Assert.IsTrue(sq is Square);
                Assert.AreEqual(sq.Area, i*i);
            }
        }

        [Test]
        public void CircleTest()
        {
            var f = new ShapeFactory();
            for (int i = 0; i < 100; i++)
            {
                var sq = f.CreateShape("circle", i);
                Assert.IsTrue(sq is Circle);
                Assert.AreEqual(sq.Area, (float)(i * i * Math.PI));
            }
        }

        [Test]
        public void RectangleTest()
        {
            var f = new ShapeFactory();
            f.RegisterWorker(new RectangleCreator());
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    var sq = f.CreateShape("rectangle", i, j);
                    Assert.IsTrue(sq is Rectangle);
                    Assert.AreEqual(sq.Area, i * j);
                }
            }
        }
    }
}

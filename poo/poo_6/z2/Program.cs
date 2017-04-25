using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace z2
{
    class Context : Dictionary<string, bool>
    {

    }

    abstract class Expression
    {
        public abstract bool GetValue(Context ctx);
    }

    abstract class UnaryExpression : Expression
    {
        protected readonly Expression e;
        public UnaryExpression(Expression ex)
        {
            e = ex;
        }
    }

    class Negation : UnaryExpression
    {
        public override bool GetValue(Context ctx)
        {
            return !e.GetValue(ctx);
        }

        public Negation(Expression ex) : base(ex)
        { }
    }

    abstract class BinaryExpression : Expression
    {
        protected readonly Expression e1;
        protected readonly Expression e2;
        public BinaryExpression(Expression ex1, Expression ex2)
        {
            e1 = ex1;
            e2 = ex2;
        }
    }

    class Conjunction : BinaryExpression
    {
        public Conjunction(Expression ex1, Expression ex2) : base(ex1, ex2)
        { }

        public override bool GetValue(Context ctx)
        {
            return e1.GetValue(ctx) && e2.GetValue(ctx);
        }
    }

    class Alternative : BinaryExpression
    {
        public Alternative(Expression ex1, Expression ex2) : base(ex1, ex2)
        { }

        public override bool GetValue(Context ctx)
        {
            return e1.GetValue(ctx) || e2.GetValue(ctx);
        }
    }

    abstract class Atom : Expression
    {

    }

    class Constant : Atom
    {
        readonly bool Value;
        public Constant(bool v)
        {
            Value = v;
        }
        public override bool GetValue(Context ctx)
        {
            return Value;
        }
    }

    class Variable : Atom
    {
        readonly string Name;

        public Variable(string n)
        {
            Name = n;
        }

        public override bool GetValue(Context ctx)
        {
            if (ctx.ContainsKey(Name))
                return ctx[Name];
            throw new KeyNotFoundException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Context ctx = new Context();
            ctx["x"] = false;
            ctx["y"] = true;
            Expression exp =
                new Negation(
                    new Alternative(
                        new Conjunction(
                            new Variable("x"),
                            new Constant(true)
                            ),
                        new Conjunction(
                            new Variable("z"),
                            new Constant(false)
                            )
                        )
                    );
            bool val = exp.GetValue(ctx);            Console.WriteLine(val);            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zad5
{
    class Person
    {
        public string name { get; }

        public Person(string n)
        {
            name = n;
        }
    };

    abstract class PersonRegistryWithNotifier
    {
        protected IPeopleProvider people;
        public abstract void NotifyPeople();
    }

    interface IPeopleProvider
    {
        IEnumerable<Person> GetPeople();
    }

    class ConsolePeopleProvider : IPeopleProvider
    {
        public IEnumerable<Person> GetPeople()
        {
            var s = Console.ReadLine();
            while(! string.IsNullOrEmpty(s))
            {
                yield return new Person(s);
                s = Console.ReadLine();
            }
        }
    }

    class LoudConsolePeopleProvider : IPeopleProvider
    {
        public IEnumerable<Person> GetPeople()
        {
            var s = Console.ReadLine();
            while (!string.IsNullOrEmpty(s))
            {
                yield return new Person(s.ToUpper());
                s = Console.ReadLine();
            }
        }
    }
    
    abstract class PersonRegistryWithPeople
    {
        protected INotifier notifier;
        protected abstract IEnumerable<Person> people { get; }
        public void NotifyPeople()
        {
            notifier.NotifyPeople(people);
        }
    }

    interface INotifier
    {
        void NotifyPeople(IEnumerable<Person> ppl);
    }

    class ConsoleNotifier : INotifier
    {
        public void NotifyPeople(IEnumerable<Person> ppl)
        {
            foreach (var person in ppl)
                Console.WriteLine($"Hey there, {person.name}!");
        }
    }

    class FunnyConsoleNotifier : INotifier
    {
        static readonly string[] jokes =
        {
            "Your pants are on fire!",
            "Your shoe is untied!",
            "Your fridge is running!"
        };

        static Random rnd = new Random();

        public void NotifyPeople(IEnumerable<Person> ppl)
        {
            
            foreach (var person in ppl)
                Console.WriteLine($"Hey there, {person.name}! {jokes[rnd.Next(0,jokes.Length-1)]}");
        }
    }

    class SimplePersonRegistryWithPeople : PersonRegistryWithPeople
    {
        public List<Person> People = new List<Person>();
        protected override IEnumerable<Person> people
        {
            get { return People; }
        }

        public SimplePersonRegistryWithPeople(INotifier notifier)
        {
            this.notifier = notifier;
        }
    }

    class SimplePersonRegistryWithNotifier : PersonRegistryWithNotifier
    {
        public SimplePersonRegistryWithNotifier(IPeopleProvider provider)
        {
            this.people = provider;
        }

        public override void NotifyPeople()
        {
            foreach (var person in people.GetPeople())
                Console.WriteLine($"Hey there, {person.name}!");
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var r1 = new SimplePersonRegistryWithPeople(new FunnyConsoleNotifier());
            r1.People.AddRange(new[] { "Tomek", "Adrian", "Aneta" }.Select(x => new Person(x)));
            r1.NotifyPeople();
            var r2 = new SimplePersonRegistryWithPeople(new ConsoleNotifier());
            r2.People.AddRange(new[] { "Adam", "Ania", "Julia" }.Select(x => new Person(x)));
            r2.NotifyPeople();

            var r3 = new SimplePersonRegistryWithNotifier(new ConsolePeopleProvider());
            r3.NotifyPeople();

            var r4 = new SimplePersonRegistryWithNotifier(new LoudConsolePeopleProvider());
            r4.NotifyPeople();
        }
    }
}

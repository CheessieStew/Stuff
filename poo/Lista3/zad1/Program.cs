using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class Enumerable
{
    //polymorphism
    public static T RandomElement<T>(this IEnumerable<T> ienumerable, Random rand)
    {
        return ienumerable.ElementAt(rand.Next(0, ienumerable.Count()));
    }
}


//Creator
interface IContentGenerator<T>
{
    T TryGenerate(Random random);
    bool FillParameters(Random random);
    void ResetParameters();
}

interface IStorable
{
    // a class implementing this interface is serializable to json
}

class NPC : IStorable
{

}

class Faction : IStorable
{
    IContentGenerator<NPC> NPCGenerator;

    public NPC GenerateNPC(Random rand)
    {
        if (NPCGenerator != null)
        {
            NPCGenerator.FillParameters(rand);
            return NPCGenerator.TryGenerate(rand);
        }
        return null;
    }
}

class Event
{
    void Execute()
    {

    }
}


static class VariableStorage
{
    //this class holds all the variables relevant to the current gamestate

    public class VariableLocation
    {
        string Space, Key;
        public VariableLocation(string space, string key)
        {
            Space = space;
            Key = key;
        }
    }

    public static void Add (VariableLocation location, IStorable storable)
    {

    }

}

//Controller of a subsystem
class World
{
    IContentGenerator<Faction>[] NationGenerators;
    IContentGenerator<Faction>[] OrganizationGenerators;
    IContentGenerator<Event>[] EventGenerators;
    List<Faction> Nations;
    List<Faction> Organizations;

    Random rand = new Random();

    public class DailyReport
    {
        public void Add(Event ev)
        {

        }
    }

    public void CreateNewWorld()
    {
        for (int i = 0; i < 10; i++)
        {

            var gen = NationGenerators.RandomElement(rand);
            gen.FillParameters(rand);
            VariableStorage.Add(new VariableStorage.VariableLocation("Nations", $"Nation{i}"), gen.TryGenerate(rand));
        }
        for (int i = 0; i < 20; i++)
        {

            var gen = OrganizationGenerators.RandomElement(rand);
            gen.FillParameters(rand);
            VariableStorage.Add(new VariableStorage.VariableLocation("Organizations", $"Nation{i}"), gen.TryGenerate(rand));
        }
    }

    //Information expert (holds generators, so it manages their behaviour), creator (creaters DailyReport)
    public DailyReport AdvanceDay()
    {
        DailyReport res = new DailyReport();
        foreach (IContentGenerator<Event> gen in EventGenerators)
        {
            gen.ResetParameters();
            while (gen.FillParameters(rand))
            {
                Event ev = gen.TryGenerate(rand);
                if (ev != null) res.Add(ev);
            }
        }
        return new DailyReport();
    }

    public NPC CreateNPC()
    {
        return Nations.RandomElement(rand).GenerateNPC(rand);
    }

}
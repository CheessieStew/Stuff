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
    bool FillParameters(Random random, VariableStorage storage);
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

    public NPC GenerateNPC(Random rand, VariableStorage storage)
    {
        if (NPCGenerator != null)
        {
            NPCGenerator.FillParameters(rand, storage);
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


class VariableStorage
{
    //this class holds all the variables relevant to the current gamestate

    public struct VariableLocation
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

    public static void Get (VariableLocation location, IStorable storable)
    {

    }

    // loads the Storage's state
    public static VariableStorage FromJSon()
    {
        return new VariableStorage();
    }
}

//Controller of a subsystem
class World
{
    IContentGenerator<Faction>[] nationGenerators;
    IContentGenerator<Faction>[] organizationGenerators;
    IContentGenerator<Event>[] eventGenerators;
    List<Faction> nations;
    List<Faction> organizations;
    VariableStorage storage;

    Random rand = new Random();

    public class DailyReport
    {
        public void Add(Event ev)
        {

        }
    }

    public void Populate()
    {
        for (int i = 0; i < 10; i++)
        {

            var gen = nationGenerators.RandomElement(rand);
            gen.FillParameters(rand, storage);
            VariableStorage.Add(new VariableStorage.VariableLocation("Nations", $"Nation{i}"), gen.TryGenerate(rand));
        }
        for (int i = 0; i < 20; i++)
        {
            var gen = organizationGenerators.RandomElement(rand);
            gen.FillParameters(rand, storage);
            VariableStorage.Add(new VariableStorage.VariableLocation("Organizations", $"Nation{i}"), gen.TryGenerate(rand));
        }
        for (int i = 0; i < 40; i++)
        {

        }
    }

    //Information expert (holds generators, so it manages their behaviour), creator (creaters DailyReport)
    public DailyReport AdvanceDay()
    {
        DailyReport res = new DailyReport();
        foreach (IContentGenerator<Event> gen in eventGenerators)
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
        return nations.RandomElement(rand).GenerateNPC(rand, storage);
    }

}
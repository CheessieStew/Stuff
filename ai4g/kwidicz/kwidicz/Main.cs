using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Grab Snaffles and try to throw them through the opponent's goal!
 * Move towards a Snaffle and use your team id to determine where you need to throw it.
 **/

struct Entity
{
    public int id;
    public Vector2 pos;
    public Vector2 vel;
    public int state;
}

struct Order //of Phoenix jajajaja
{
    public enum ActionType { NOOP, MOVE, THROW, SPELL};
    public static string ThrowComment = "Booom!";
    public static string FlyComment = "Wooo!";
    public static string ChaseComment = "C'mere!";
    public static string DodgeComment = "Mad skillz!";
    public static string FailedDodgeComment = "Not mad enough...";
    public static string DefComment = "Stop right there!";
    public Vector2 target;
    public int param;
    public ActionType type;
    public int spellTarget;
    public Spell spell;
    public String comment;
    public bool chasing;
    public int chasedTarget;
    public override string ToString()
    {
        if (type == ActionType.SPELL)
            return $"{spell.name} {spellTarget} {comment}";
        return $"{type} {(int)Math.Round(target.x)} {(int)Math.Round(target.y)} {param} {comment}";
    }
}

class Spell
{
    public int cost;
    public string name;
    static Spell _obliviate;
    static Spell _petrificus;
    static Spell _accio;
    static Spell _flipendo;

    private Spell(int c, string n)
    {
        cost = c;
        name = n;
    }

    public static Spell Obliviate
    {
        get
        {
            if (_obliviate != null)
                return _obliviate;
            _obliviate = new Spell(5, "OBLIVIATE");
            return _obliviate;
        }
    }
    public static Spell Petrificus
    {
        get
        {
            if (_petrificus != null)
                return _petrificus;
            _petrificus = new Spell(10, "PETRIFICUS");
            return _petrificus;
        }
    }
    public static Spell Accio
    {
        get
        {
            if (_accio != null)
                return _accio;
            _accio = new Spell(20, "ACCIO");
            return _accio;
        }
    }
    public static Spell Flipendo
    {
        get
        {
            if (_flipendo != null)
                return _flipendo;
            _flipendo = new Spell(20, "FLIPENDO");
            return _flipendo;
        }
    }

}


partial class Kwidicz
{
    int myTeamId;
    static int width = 16001;
    static int height = 7501;
    Entity[] myWizards = new Entity[2];
    Order[] orders = new Order[2];
    int mana = 0;
    int myScore = 0;
    int enemyScore = 0;
    Entity[] enemyWizards = new Entity[2];
    List<Entity> snaffles = new List<Entity>();
    List<Tuple<int, int>> bullets = new List<Tuple<int, int>>();
    Entity[] bludgers = new Entity[2];
    int[] bludgerMemories = new int[2];
    Vector2 enemyGoal;
    Vector2 myGoal;

    public Kwidicz(int id)
    {
        myTeamId = id;
    }

    public void GetGameInfo()
    {
        if (myTeamId == 0)
            myGoal = new Vector2(0, 3750);
        else
            myGoal = new Vector2(16000, 3750);
        enemyGoal = new Vector2(16000 - myGoal.x, myGoal.y);
        int entities = int.Parse(Console.ReadLine()); // number of entities still in game
        List<Entity> newSnaffles = new List<Entity>();
        for (int i = 0; i < entities; i++)
        {
            Entity e = new Entity();
            string[] inputs = Console.ReadLine().Split(' ');
            e.id = int.Parse(inputs[0]); // entity identifier
            int entityId = e.id;
            string entityType = inputs[1]; // "WIZARD", "OPPONENT_WIZARD" or "SNAFFLE" (or "BLUDGER" after first league)
            e.pos.x = int.Parse(inputs[2]); // position
            e.pos.y = int.Parse(inputs[3]); // position
            e.vel.x = int.Parse(inputs[4]); // velocity
            e.vel.y = int.Parse(inputs[5]); // velocity
            e.state = int.Parse(inputs[6]); // 1 if the wizard is holding a Snaffle, 0 otherwise
            switch (entityType)
            {
                case "WIZARD":
                    entityId -= 2 * myTeamId;
                    myWizards[entityId] = e;
                    break;
                case "OPPONENT_WIZARD":
                    entityId -= 2 * (1 - myTeamId);
                    enemyWizards[entityId] = e;
                    break;
                case "SNAFFLE":
                    newSnaffles.Add(e);
                    break;
                case "BLUDGER":
                    bludgers[entityId % 2] = e;
                    break;

            }
            foreach(Entity s in snaffles)
            {
                if (newSnaffles.Any(ss => ss.id == s.id))
                    continue;
                else
                {
                    if (s.pos.x > 14000 && myTeamId == 0 || s.pos.x < 2000 && myTeamId == 1)
                        myScore++;
                    else if (s.pos.x > 14000 && myTeamId == 1 || s.pos.x < 2000 && myTeamId == 0)
                        enemyScore++;
                }
            }
        }
        snaffles = newSnaffles;
        Console.Error.WriteLine($"me: {myScore}, enemy: {enemyScore}");
    }

    public void Tick()
    {
        if (mana < 100)
            mana++;
        if (bludgerMemories[0] > 0) bludgerMemories[0]--;
        if (bludgerMemories[1] > 0) bludgerMemories[1]--;
        bullets = bullets.Select(b => new Tuple<int, int>(b.Item1 - 1, b.Item2)).Where(b => b.Item1 > 0).ToList();

    }
    static void Main(string[] args)
    {
       // for (int i = 0; i < 100; i++)
       // {
       //     for (int j = 0; j < 100; j++)
       //     {
       //         Console.Error.Write(new Vector2(i, j).InCone(new Vector2(0, 50), new Vector2(80, 50), 2, 65) == true ? "0": ".");
       //     }
       //     Console.WriteLine();
       // }

        int myTeamId = int.Parse(Console.ReadLine()); // if 0 you need to score on the right of the map, if 1 you need to score on the left
        Kwidicz player = new Kwidicz(myTeamId);

        // game loop
        while (true)
        {
            player.GetGameInfo();
            player.Think();
            for(int i = 0; i < 2; i++)
            {
                Console.WriteLine($"{player.orders[i]} (mana {player.mana})");
            }
            player.Tick();

        }
    }
}
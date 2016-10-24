using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;



class Vertex
{
    public int x, y;
    public Vertex prev;
    public Vertex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Vertex u, Vertex v)
    {
        Object a = u;
        Object b = v;
        if (a == null || b == null)
            return a==b;
        return (u.x == v.x && u.y == v.y);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        else return (this == obj as Vertex);
    }

    public override int GetHashCode()
    {
        return (x ^ y);
    }

    public static bool operator !=(Vertex u, Vertex v)
    {
        return !(u.x == v.x && u.y == v.y);
    }
    public override String ToString()
    {
        return "("+x+"," + y + ")";
    }
    public Vertex[] Neighbours()
    {
        return new Vertex[4]
            {
                new Vertex(x - 1, y),
                new Vertex(x, y - 1),
                new Vertex(x + 1, y),
                new Vertex(x, y + 1)
            };
    }
}

class Game
{
    int kr, kc, rCount, cCount, alarm;
    Vertex cRoom, start;
    String[] rows;
    int[,] fScore, gScore;
    static void Main(string[] args)
    {
        Game game = new Game();
        game.dfs();
        game.goWin();

    }
    
    void dfs()
    {
        Stack<Vertex> stack = new Stack<Vertex>();
        stack.Push(new Vertex(kr, kc));
        HashSet<Vertex> visited = new HashSet<Vertex>();
        while (stack.Any())
        {
            Vertex cur = stack.Pop();
            visited.Add(cur);

            foreach (Vertex x in cur.Neighbours())
            {
                if (visited.Contains(x))
                    continue;
                if (rows[x.x][x.y] == '?')
                    moveTo(cur,false);
                if (rows[x.x][x.y] == '#') 
                    continue;
                if (rows[x.x][x.y] == 'C')
                {
                    cRoom = x;
                    continue;
                }
                stack.Push(x);
            }
        }
    }
    
    void goWin()
    {
        moveTo(cRoom, true);
        moveTo(start, true);
    }

    void moveTo(Vertex goal, bool finish)
    {
        Console.Error.WriteLine("moving to " + goal);
        Vertex start = AStar(goal, new Vertex(kr, kc), finish);
        Console.Error.WriteLine("Astar finished");
        if (start != new Vertex(kr, kc))
            throw new Exception("damn");
        while (start!=goal)
        {
            if (start.prev == null)
                throw new Exception("mto: HOW THE HELL?! " + start + ".prev = null");
            if (start.y == start.prev.y)
            {
                if (start.x - start.prev.x == 1)
                    Console.WriteLine("UP");
                else if (start.x - start.prev.x == -1)
                    Console.WriteLine("DOWN");
                else throw new Exception("damn #2");
            }
            else if (start.x == start.prev.x)
            {
                if (start.y - start.prev.y == 1)
                    Console.WriteLine("LEFT");
                else if (start.y - start.prev.y == -1)
                    Console.WriteLine("RIGHT");
                else throw new Exception("damn #2");
            }
            else throw new Exception("damn #3");
            GetGameInfo();
            start = start.prev;
        }
    }

    Vertex AStar(Vertex start, Vertex goal, bool finish)
    {
        HashSet<Vertex> openset = new HashSet<Vertex>();
        HashSet<Vertex> closedset = new HashSet<Vertex>();
        openset.Add(start);
        gScore[start.x, start.y] = 0;
        fScore[goal.x, goal.y] = int.MaxValue;
        while (openset.Any())
        {
            Vertex x = goal;
            foreach(Vertex v in openset)
            {
                x = fScore[v.x, v.y] <= fScore[x.x, x.y] ? v : x;
            }
            Console.Error.WriteLine("AStar in " + x);
            if (x == goal)
            {
                if (x.prev == null)
                    throw new Exception("Astar: HOW THE HELL?! " + x + ".prev = null");
                return x;

            }
            openset.Remove(x);
            closedset.Add(x);
            foreach(Vertex y in x.Neighbours())
            {
                if (closedset.Contains(y) || y.x < 0 || y.y < 0 || y.x >= rCount || y.y >= cCount)
                    continue;
                if (rows[y.x][y.y] == '#')
                    continue;
                if (!finish && rows[y.x][y.y] == 'C')
                    continue;
                bool better = false;
                int ngsc = gScore[x.x, x.y] + 1;
                if (!openset.Contains(y))
                {
                    if (!openset.Add(y))
                        Console.Error.WriteLine("lol");
                    Console.Error.WriteLine("Astar adding " + y + " to openset");
                    better = true;
                }
                else if (ngsc < gScore[y.x, y.y])
                {
                    better = true;   
                    Console.Error.WriteLine("openset contains " + y);
                }
                else
                    Console.Error.WriteLine("openset contains " + y);
                if (better)
                {
                    gScore[y.x, y.y] = ngsc;
                    fScore[y.x, y.y] = ngsc + Math.Abs(y.x - goal.x) + Math.Abs(y.y - goal.y);
                    y.prev = x;
                    Console.Error.WriteLine(y + ".prev = " + y.prev);

                }

            }
        }
        return null;
    }
    
    Game()
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        rCount = int.Parse(inputs[0]); // number of rows.
        cCount = int.Parse(inputs[1]); // number of columns.
        alarm = int.Parse(inputs[2]); // number of rounds between the time the alarm countdown is activated and the time the alarm goes off.
        rows = new String[rCount];
        fScore = new int[rCount, cCount];
        gScore = new int[rCount, cCount];
        GetGameInfo();
        start = new Vertex(kr, kc);
    }

    void GetGameInfo()
    {
        Console.Error.WriteLine("gettin' game info");
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        kr = int.Parse(inputs[0]); // row where Kirk is located.
        kc = int.Parse(inputs[1]); // column where Kirk is located.
        for (int i = 0; i < rCount; i++)
        {
            rows[i] = Console.ReadLine(); // C of the characters in '#.TC?' (i.e. one line of the ASCII maze).
        }

    }
}
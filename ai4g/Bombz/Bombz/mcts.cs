using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MCT
{
    public static int whomoves = -1;
    MCTNode root;
    public static readonly Random random = new Random();
    public static readonly float explorationRate = 1;
    Board board; //represents the state of root node
    int MyPlayerId;
    List<Move> mmss;


    public MCT(Board board, int myid)
    {
        this.board = board;
        MyPlayerId = myid;
        root = new MCTNode(board, null);
        mmss = new List<Move>();
    }

    public void Reuse(Move[] moves)
    {
        if (!root.board.Legal(moves))
        {
            Console.Error.WriteLine("Wat");
            Console.Error.WriteLine(root.board);
        }
        foreach (Move m in moves)
            mmss.Add(m);
        int c = 0;
        for (int i = 0; i < root.Children.Length; i++)
        {
            MCTNode child = root.Children[i];
            int m = 0;
            bool thisIsIt = true;
            foreach (Move move in root.Moves[i])
            {
                if (move != moves[m])
                {
                    thisIsIt = false;
                    break;
                }
                m++;
            }
            if (thisIsIt)
            {
                foreach (Move move in root.Moves[i])
                    Console.Error.WriteLine($"reusing {move}");
                Console.Error.WriteLine($"Time of root is not {child.board.timer}");
                root = child;
                if (root == null)
                {
                    Console.Error.WriteLine("Had to make a new tree");
                    root = new MCTNode(board, null);
                }
                else
                    root.Parent = null;
                return;
            }
            c++;
        }
        Console.Error.WriteLine("Nothing to reuse, wth?");
    }

    public void Rebind()
    {
        root = new MCTNode(board, null);
    }

    public Move Run(DateTime deadline)
    {
        if (DateTime.Now >= deadline)
        {
            Console.Error.WriteLine("srsly? Time gone before I started");
        }

        while (DateTime.Now < deadline)
        {
            Board curBoard = board.Copy();
            MCTNode vl = TreePolicy(root, curBoard);
            // after TreePolicy curBoard represents the state
            // of the returned node
            int[] scores = DefaultPolicy(curBoard);
            BackUp(vl, scores);
        }
        if (root.expansion == 0)
            return root.Moves[0].ElementAt(MyPlayerId);
        Console.Error.WriteLine($"Let's see who's the winner, root pulled {root.TimesPulled} times, has {root.expansion} children!");
        int winner = root.BestChild(MyPlayerId);
        Console.Error.WriteLine($"The winner node had {root.Children[winner].TimesPulled} pulls, scores: ");
        foreach(int score in root.Children[winner].scores)
            Console.Error.Write($"{((float)score) / root.Children[winner].TimesPulled}, ");
        Console.Error.WriteLine();

        return root.Moves[winner].ElementAt(MyPlayerId);
    }

    void BackUp(MCTNode v, int[] scores)
    {
        while (v != null)
        {
            v.TimesPulled++;
            for (int i = 0; i < scores.Length; i++)
            {
                v.scores[i] += scores[i];
            }
            v = v.Parent;
        }
    }

    MCTNode TreePolicy(MCTNode v, Board curBoard)
    {
        if (v == null)
            Console.Error.WriteLine("I have lost my abillity to even");
        MCTNode cur = v;
        int d = 0;
        int time = curBoard.timer;
        while (!curBoard.Terminal) //"v is not terminal"
        {
            if (time+d != cur.board.timer)
                throw new Exception("life is strange");
            if (cur == null)
            {
                Console.Error.WriteLine($"srsly wth at depth {d} 1");
                throw new Exception($"srsly wth at depth {d} 1");
            }
            if (!cur.FullyExpanded)
            {
                try
                {
                    return cur.Expand(curBoard);
                }
                catch(Exception e)
                {
                    int i = 0;
                   foreach(Move m in mmss)
                   {
                       Console.Error.Write($"\"{m}\", ");
                       i++;
                       if (i % 4 == 0) Console.Error.WriteLine();
                   }
                    Console.Error.WriteLine($"I THINK (depth {d}):");
                    Console.Error.WriteLine(curBoard);
                    throw e;
                }
            }
            else
            {
                int winner = cur.BestChild(MyPlayerId);
                whomoves = 2;
                try
                {
                    curBoard.Move(cur.Moves[winner]);
                }
                catch(Exception ex)
                {
                    int i = 0;
                    foreach (Move m in mmss)
                    {
                        Console.Error.Write($"\"{m}\", ");
                        i++;
                        if (i % 4 == 0) Console.Error.WriteLine();
                    }
                    Console.Error.WriteLine($"I THINK (depth {d}):");
                    Console.Error.WriteLine(curBoard);
                    Console.Error.WriteLine($"NODE THINKs (depth {d}):");
                    Console.Error.WriteLine(cur.board);
                    throw ex;
                }
                d++;

                cur = cur.Children[winner];
                if (cur == null)
                {
                    throw new Exception($"srsly wth at depth {d} 2");
                }
            }
        }
        return cur;
    }


    int[] DefaultPolicy(Board curBoard)
    {
        int remBoxes = curBoard.remainingBoxes;
        while (curBoard.remainingBoxes > remBoxes * 0.9 && !curBoard.Terminal)
        {
            curBoard.RandomMove();
        }
        return curBoard.scores;
    }
    
}

class MCTNode
{
    public IEnumerable<Move>[] Moves;
    public int ChildrenCount;
    public MCTNode[] Children;
    public MCTNode Parent;
    public bool FullyExpanded { get { return expansion == ChildrenCount; } }
    public int TimesPulled;

    //mean score of a single player in this Node's state
    public float MeanScoreOfPlayer(int which)
    {
        return ((float)scores[which]) / TimesPulled;
    }

    public int[] scores;
    public int expansion;

    public MCTNode Expand(Board board)
    {
        MCT.whomoves = 3;
        try
        {
            board.Move(Moves[expansion]); //state(newChild) = f(state(this), move);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Node thinks: {this.board}");
            Console.Error.WriteLine($"To make it funnier: board.Legal(Moves[expansion]) = {this.board.Legal(Moves[expansion])}"); 

            throw ex;
        }
        Children[expansion] = new MCTNode(board, this); 
        expansion++;
        return Children[expansion - 1];
    }

    public int BestChild(int playerID)
    {
        int theOne = -1;
        float bestScore = int.MinValue;
        for (int i = 0; i < ChildrenCount; i++)
        {
            if (Children[i] != null)
            {
                float myScore = Children[i].MeanScoreOfPlayer(playerID);
                float theirScore = 0;
                float maxEnemy = 0;
                for (int j = 0; j < scores.Length; j++)
                {
                    if (j != playerID)
                    {
                        theirScore += Children[i].MeanScoreOfPlayer(j);
                        if (Children[i].MeanScoreOfPlayer(j) > maxEnemy)
                            maxEnemy = Children[i].MeanScoreOfPlayer(j);
                    }
                }
                theirScore /= scores.Length - 1;

                float score = (myScore - maxEnemy) * theirScore / (Math.Abs(myScore - theirScore) + 1);
                score += MCT.explorationRate * (float)Math.Sqrt(2 * Math.Log(TimesPulled) / Children[i].TimesPulled);
                if (score > bestScore)
                {
                    bestScore = score;
                    theOne = i;
                }
            }
        }
        if (theOne < 0)
            throw new Exception($"wat, but {ChildrenCount}! exp = {expansion} children[{0}] = {Children[0]!=null}");
        return theOne;
    }

    public Board board;

    public MCTNode(Board board, MCTNode par)
    {
        this.board = board.Copy();
        Moves = board.LegalMoves().OrderBy(x => MCT.random.Next()).ToArray();

        ChildrenCount = Moves.Length;

        Children = new MCTNode[ChildrenCount];
        Parent = par;
        TimesPulled = 0;
        expansion = 0;
        scores = new int[board.playersAmm];
    }
}
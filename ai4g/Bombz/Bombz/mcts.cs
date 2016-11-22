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


    public MCT(Board board, int myid)
    {
        this.board = board;
        MyPlayerId = myid;
        root = new MCTNode(board, null);
    }

    public void Reuse(Move[] moves)
    {
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
                root = child;
                if (root == null)
                {
                    Console.Error.WriteLine("Had to make a new tree");
                    root = new MCTNode(board, null);
                }
                else
                {
                    if (root.Board != board)
                    {
                        Console.Error.WriteLine("Difference!");
                        Console.Error.WriteLine($"left (root) {root.Board}");
                        Console.Error.WriteLine($"root.parent {root.Parent.Board}");
                        Console.Error.WriteLine($"right (main) {board}");

                        throw new Exception("cannot into sim");
                    }
                    root.Parent = null;

                }
                return;
            }
            c++;
        }
        Console.Error.WriteLine("Nothing to reuse, wth?");
        Console.Error.WriteLine($"Real {board}");
        Console.Error.WriteLine($"root: {root.Board}");
        Console.Error.WriteLine("Apparently the move we guessed is not legal on it");
        foreach (Move m in moves)
            Console.Error.Write($"{m}, ");
        throw new Exception("nothing to reuse");
    }

    public void Rebind()
    {
        root = new MCTNode(board, null);
    }

    public Move Run(DateTime deadline)
    {
        Console.Error.WriteLine($"starting search, roottime {root.Board.timer}");
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
        Console.Error.WriteLine($"The winner node had {root.Children[winner].TimesPulled} pulls, rating: {root.RateChild(winner,MyPlayerId)}");
        foreach (Move m in root.Moves[winner])
            Console.Error.Write($"{m}, ");
        Console.Error.WriteLine($"this move makes {root.MoveSenses[winner]} sense");
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
        List<Move> movvs = new List<Move>();

        MCTNode cur = v;
        int time = curBoard.timer;
        while (!curBoard.Terminal) //"v is not terminal"
        {
            if (!cur.FullyExpanded)
            {
                try
                {
                    MCTNode res =  cur.Expand(curBoard);
                    return res;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("-----I think----");
                    Console.Error.WriteLine(curBoard);
                    Console.Error.WriteLine("-----Node thinks----");
                    Console.Error.WriteLine(cur.Board);
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
                catch(Exception e)
                {
                    Console.Error.WriteLine("Moves:");
                    foreach (Move move in movvs)
                        Console.Error.WriteLine("movvs");
                    Console.Error.WriteLine("-----I think----");
                    Console.Error.WriteLine(curBoard);
                    Console.Error.WriteLine("-----Node thinks----");
                    Console.Error.WriteLine(cur.Board);
                    throw e;
                }
                foreach (Move move in cur.Moves[winner])
                    movvs.Add(move);
                cur = cur.Children[winner];
            }
        }
        return cur;
    }


    int[] DefaultPolicy(Board curBoard)
    {
        int remBoxes = curBoard.remainingBoxes;
        int time = curBoard.timer;
        while (curBoard.remainingBoxes > remBoxes * 0.9 && !curBoard.Terminal && curBoard.timer - time < 32)
        {
            curBoard.RandomMove();
        }
        //Console.WriteLine($"----after sim for {curBoard.timer - time}----");
        //Console.WriteLine($"remBoxes * 0.9 = {remBoxes * 0.9}, remaining: {curBoard.remainingBoxes}");
        for(int i = 0; i < board.playersAmm; i++)
        {
            //Console.Error.WriteLine($"player {i} has score {curBoard.scores[i]}, upgrades {board.upgrades[i]}");
            curBoard.scores[i] += board.upgrades[i]*20;
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
    public int[] MoveSenses;

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
        board.Move(Moves[expansion]); //state(newChild) = f(state(this), move);
        

        Children[expansion] = new MCTNode(board, this); 
        expansion++;
        return Children[expansion - 1];
    }

    public float RateChild(int i, int playerID)
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
        return (myScore - maxEnemy) * theirScore / (Math.Abs(myScore - theirScore) + 10);
    }

    public int BestChild(int playerID)
    {
        int theOne = -1;
        float bestScore = int.MinValue;
        for (int i = 0; i < ChildrenCount; i++)
        {
            if (Children[i] != null)
            {

                float score = 2000 + RateChild(i, playerID);
                if (score < 0)
                    throw new Exception("less!!!!");
                score += MCT.explorationRate * (float)Math.Sqrt(2 * Math.Log(TimesPulled) / Children[i].TimesPulled);
                    score *= MoveSenses[i];
                if (score > bestScore)
                {
                    bestScore = score;
                    theOne = i;
                }
            }
        }
        return theOne;
    }

    public Board Board;
    public MCTNode(Board board, MCTNode par)
    {
        Board = board.Copy();
        Moves = board.LegalMoves().OrderBy(x => MCT.random.Next()).ToArray();
        MoveSenses = new int[Moves.Count()];
        for (int i = 0; i < Moves.Count(); i++)
        {
            MoveSenses[i] = 4096;
            int player = 0;
            foreach(Move m in Moves[i])
            {
                if (m.bomb)
                    MoveSenses[i] = Math.Min(MoveSenses[i], board.BoxesInRange(board.players[player].position, (int)board.players[player].param2));
                player++;
            }
            if (MoveSenses[i] == 4096)
                MoveSenses[i] = 1;
        }
        ChildrenCount = Moves.Length;

        Children = new MCTNode[ChildrenCount];
        Parent = par;
        TimesPulled = 0;
        expansion = 0;
        scores = new int[board.playersAmm];
    }
}
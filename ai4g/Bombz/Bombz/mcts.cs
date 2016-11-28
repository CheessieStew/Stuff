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
    public static readonly float explorationRate = 20;
    Board board; //represents the state of root node
    int MyPlayerId;


    public MCT(Board board, int myid)
    {
        this.board = board;
        MyPlayerId = myid;
        root = new MCTNode(board.Copy(), null);
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
                if (root == null || root.Moves == null)
                {
                    Console.Error.WriteLine("Had to make a new tree");
                    root = new MCTNode(board.Copy(), null);
                }
                else
                {

                    root.Parent = null;
                }
                return;
            }
            c++;
        }
        Console.Error.WriteLine("Nothing to reuse, wth?");
        Console.Error.WriteLine($"main: {board}");

        throw new Exception("nothing to reuse");
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
            float[] scores;
            if (vl.Moves != null)
            {
                scores = DefaultPolicy(curBoard);
            }
            else
            {

                scores = new float[board.playersAmm];
                for (int i = 0; i < scores.Length; i++)
                    scores[i] = vl.Parent.MeanScoreOfPlayer(i);
            }
            BackUp(vl, scores);
        }
        if (root.expansion == 0)
        {
            Console.Error.WriteLine("Couldn't even search!");
            return root.Moves[0].ElementAt(MyPlayerId);
        }

        Console.Error.WriteLine($"Let's see who's the winner, root pulled {root.TimesPulled} times, has {root.expansion} children!");

        Move m = root.PickMove(MyPlayerId);
        if (m.bomb)
        {
            Console.Error.WriteLine($"it has {board.BoxesInRange(board.players[MyPlayerId].position, (int)board.players[MyPlayerId].param2)}");
        }
        return m;
    }

    void BackUp(MCTNode v, float[] scores)
    {
        while (v != null)
        {
            v.TimesPulled++;
            bool killed = false;
            for (int i = 0; i < scores.Length; i++)
            {

                if (scores[i] < 0 && !killed)
                {
                    v.kills++;
                    killed = true;
                    break;
                }
            }
            if (!killed)
                for (int i = 0; i < scores.Length; i++)
                {
                    v.scores[i] += scores[i];
                }

            v = v.Parent;
        }
    }

    MCTNode TreePolicy(MCTNode v, Board curBoard)
    {
        MCTNode cur = v;

        while (!curBoard.Terminal) //"v is not terminal"
        {
            if (!cur.FullyExpanded)
            {
                return cur.Expand(curBoard);
            }
            else
            {
                int winner = cur.BestChild(MyPlayerId);
                whomoves = 2;
                curBoard.Move(cur.Moves[winner]);
                cur = cur.Children[winner];
            }
        }
        return cur;
    }

    int d = 0;
    float[] DefaultPolicy(Board curBoard)
    {
        //Console.Error.WriteLine($"SIM at board {curBoard}");
        d = 0;
        int remBoxes = curBoard.remainingBoxes;
        int time = curBoard.timer;
        float[] scores = new float[curBoard.playersAmm];
        for (int i = 0; i < curBoard.playersAmm; i++)
        {
            if (curBoard.players[i].empty)
            {
                scores[i] = -100;
            }
            else
            {
                scores[i] += curBoard.scores[i] * 0.1f;
                scores[i] += (curBoard.Movements(curBoard.players[i].position).Count() - 1) * 3;
                double b = 0;
                Point p = curBoard.players[i].position;
                for (int r = 1; r < 4; r++)
                {
                    for (int a = 0; a <= r; a++)
                    {
                        int x = p.x + a,
                            y = p.y + (r - a);
                        if (curBoard.Grid.InBounds(x, y) && !curBoard.Grid[x, y].empty)
                        {

                            if (curBoard.players[i].param1 > 0 && curBoard.Grid[x, y].type == 3 && curBoard.Grid[x, y].param1 < 15)
                                b += 0.5 / (r * r + 2);
                            else if (curBoard.Grid[x, y].type == 2)
                                b += 3 / (r * r + 1);
                        }
                        x = p.x + a;
                        y = p.y - (r - a);
                        if (curBoard.Grid.InBounds(x, y) && !curBoard.Grid[x, y].empty)
                        {

                            if (curBoard.players[i].param1 > 0 && curBoard.Grid[x, y].type == 3 && curBoard.Grid[x, y].param1 < 15)
                                b += 0.5 / (r * r + 2);
                            else if (curBoard.Grid[x, y].type == 2)
                                b += 3 / (r * r + 1);
                        }
                        x = p.x - a;
                        y = p.y + (r - a);
                        if (curBoard.Grid.InBounds(x, y) && !curBoard.Grid[x, y].empty)
                        {

                            if (curBoard.players[i].param1 > 0 && curBoard.Grid[x, y].type == 3 && curBoard.Grid[x, y].param1 < 15)
                                b += 0.5 / (r * r + 2);
                            else if (curBoard.Grid[x, y].type == 2)
                                b += 3 / (r * r + 1);
                        }
                        x = p.x - a;
                        y = p.y - (r - a);
                        if (curBoard.Grid.InBounds(x, y) && !curBoard.Grid[x, y].empty)
                        {

                            if (curBoard.players[i].param1 > 0 && curBoard.Grid[x, y].type == 3 && curBoard.Grid[x, y].param1 < 15)
                                b += 0.5 / (r * r + 2);
                            else if (curBoard.Grid[x, y].type == 2)
                                b += 3 / (r * r + 1);
                        }
                    }
                }
                //Console.Error.WriteLine($"B value of {i} = {b} at time {curBoard.timer} ");
                scores[i] += (float)b;
                scores[i] += curBoard.upgrades[i] * 0.8f;
            }
        }

        // -----------------------------------------
        // SIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIM!
        //------------------------------------------
        while (curBoard.remainingBoxes > remBoxes * 0.8 && !curBoard.Terminal && curBoard.timer - time < 9)
        {
            curBoard.RandomMove();
            d++;
        }

        for (int i = 0; i < curBoard.playersAmm; i++)
        {
            if (curBoard.players[i].empty && scores[i] >= 0)
            {
                //Console.Error.Write($"sim kills {i}");
                scores[i] = -0.001f;
            }
            else
            {
                scores[i] += curBoard.upgrades[i] * 0.4f;
                scores[i] += curBoard.scores[i] * 0.05f;
            }
        }
        //foreach (float s in scores)
        //    Console.Error.Write($"[{s}]");
        //Console.Error.WriteLine();
        //Console.Error.WriteLine($"after {d} steps Returning scores {scores[0]}, {scores[1]}");
        return scores;
    }

}

class MCTNode
{
    public int kills = 0;
    public IEnumerable<Move>[] Moves;
    public int ChildrenCount;
    public MCTNode[] Children;
    public MCTNode Parent;
    public bool FullyExpanded { get { return expansion == ChildrenCount; } }
    public int TimesPulled;
    public int[] MoveSenses;
    public bool lethal { get { return kills == TimesPulled; } }

    //mean score of a single player in this Node's state
    public float MeanScoreOfPlayer(int which)
    {
        if (TimesPulled != kills)
            return ((float)scores[which]) / (TimesPulled - kills);
        else return 0;
    }

    public float[] scores;
    public int expansion;

    public Move PickMove(int playerID)
    {
        int last = -1;
        Move[] possibilities = new Move[10];
        float[] finalRatings = new float[10];
        int[] occurences = new int[10];
        float[] kills = new float[10];
        for (int i = 0; i < ChildrenCount; i++)
        {
            if (MoveSenses[i] > 0 && Children[i] != null)
            {
                Move m = Moves[i].ElementAt(playerID);
                bool found = false;
                for (int j = 0; j <= last; j++)
                    if (possibilities[j] == m)
                    {
                        found = true;
                        if (!Children[i].lethal)
                            finalRatings[j] += Children[i].scores[playerID];
                        occurences[j] += Children[i].TimesPulled - Children[i].kills;
                        kills[j] += Children[i].kills;

                        break;
                    }
                if (!found)
                {
                    //if (possibilities.Contains(m))
                    //    Console.Error.WriteLine($"I'm disapppoint, {m} already in");
                    last++;
                    possibilities[last] = m;
                    occurences[last] = Children[i].TimesPulled - Children[i].kills;
                    if (!Children[i].lethal)
                        finalRatings[last] = Children[i].scores[playerID];
                    else
                        finalRatings[last] = 0;
                    kills[last] += Children[i].kills;
                    //Console.Error.Write($"{m} at {i} = {RateChild(i, playerID)}  ");
                    //for (int x = 0; x < scores.Length; x++)
                    //    Console.Error.Write($"[{Children[i].MeanScoreOfPlayer(x)}]");
                }
            }
        }
        int maxi = -1;
        float max = -1000;
        for (int i = 0; i <= last; i++)
        {
            float rating = finalRatings[i] / occurences[i];
            if (occurences[i] == 0)
                rating = -66666666;
            Console.Error.WriteLine($"{possibilities[i]} is rated {rating}, killed {kills[i]}, occured {occurences[i]}");
            if (rating > max)
            {
                max = rating;
                maxi = i;
            }
        }
        if (maxi < 0)
        {
            Console.Error.WriteLine("We're dead anyway");
            maxi = 0; //we're dead anyway
        }

        return possibilities[maxi];
    }

    public MCTNode Expand(Board board)
    {

        board.Move(Moves[expansion]); //state(newChild) = f(state(this), move);

        Children[expansion] = new MCTNode(board, this, MoveSenses[expansion] == 0);
        expansion++;

        return Children[expansion - 1];
    }


    public float RateChild(int i, int playerID)
    {
        float myScore = Children[i].MeanScoreOfPlayer(playerID);
        //float theirScore = 0;
        float maxEnemy = float.MinValue;
        //float totalScore = 0;
        for (int j = 0; j < scores.Length; j++)
        {

            //totalScore += Children[i].MeanScoreOfPlayer(j);
            if (j != playerID)
            {
                //theirScore += Children[i].MeanScoreOfPlayer(j);
                if (Children[i].MeanScoreOfPlayer(j) > maxEnemy)
                    maxEnemy = Children[i].MeanScoreOfPlayer(j);
            }
        }
        // theirScore /= scores.Length - 1;
        // if (theirScore > 0 && totalScore > 0)
        //     return (myScore - maxEnemy) * (1 + theirScore / totalScore);

        return (myScore - maxEnemy);
    }

    public int BestChild(int playerID)
    {
        int theOne = -1;
        float bestScore = int.MinValue;
        for (int i = 0; i < ChildrenCount; i++)
        {
            if (Children[i] != null)
            {
                float score;
                if (MoveSenses[i] == 0)
                    score = -5000;
                else
                {
                    score = 100 + RateChild(i, playerID);
                    score += MCT.explorationRate * (float)Math.Sqrt(2 * Math.Log(TimesPulled) / Children[i].TimesPulled);
                    score *= 1 + (float)MoveSenses[i] / 4;
                }
                if (score > bestScore)
                {
                    bestScore = score;
                    theOne = i;
                }
            }
        }
        return theOne;
    }

    public MCTNode(Board board, MCTNode par, bool trash = false)
    {
        if (trash)
        {
            Parent = par;
            scores = new float[board.playersAmm];
            return;
        }
        Moves = board.LegalMoves().OrderBy(x => MCT.random.Next()).ToArray();
        MoveSenses = new int[Moves.Count()];
        for (int i = 0; i < Moves.Count(); i++)
        {
            bool freeze = false;
            MoveSenses[i] = 4096;
            int player = 0;
            foreach (Move m in Moves[i])
            {
                if (m.bomb)
                    MoveSenses[i] = Math.Min(MoveSenses[i], board.BoxesInRange(board.players[player].position, (int)board.players[player].param2));
                if (m.destination == board.players[player].position)
                {
                    freeze = true;
                }
                player++;

            }
            if (MoveSenses[i] == 4096)
                MoveSenses[i] = 1;
            if (!freeze)
                MoveSenses[i] *= 8;
        }
        ChildrenCount = Moves.Length;

        Children = new MCTNode[ChildrenCount];
        Parent = par;
        TimesPulled = 0;
        expansion = 0;
        scores = new float[board.playersAmm];
    }
}

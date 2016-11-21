using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



class Board
{
    public static readonly uint bombTimer = 8;

    public Grid Grid;
    public int playersAmm;
    public int timer;

    public Move[] guessedMoves;
    public List<Point> bombs;
    public Entity[] players;
    public int[] scores;
    public int remainingBoxes;
    public bool Terminal
    {
        get { return remainingBoxes == 0; }
    }

    private Grid copiedGrid;

    public Board(uint width, uint height, int playersAmm)
    {
        Grid = new Grid(width, height);
        copiedGrid = new Grid(width, height);
        this.playersAmm = playersAmm;

        bombs = new List<Point>();

        players = new Entity[playersAmm];

        scores = new int[playersAmm];

        for (int i = 0; i < playersAmm; i++)
            scores[i] = 0;

        guessedMoves = new Move[playersAmm];
        remainingBoxes = 0;
        timer = 0;
    }

    public Board Copy()
    {
        Board res = new Board(Grid.width, Grid.height, playersAmm);
        Grid.CopyTo(res.Grid);
        res.bombs = bombs.ConvertAll(b => b);
        players.CopyTo(res.players, 0);
        scores.CopyTo(res.scores, 0);
        res.remainingBoxes = remainingBoxes;
        res.timer = timer;
        return res;
    }


    #region sim
    public void Boom(Entity e)
    {
        if (e.param2 == 0) //it's a bomb's ghost
        {
            Console.Error.WriteLine("GHOST TRIGGERED");
            return;
        }
        int j;
        Entity bomb = e;
        int x = (int)e.xPos;
        int y = (int)e.yPos;
        bomb.param2 = 0;
        Grid[bomb.position] = bomb; //bomb stays on the grid, but we reduce it's range to 0
        copiedGrid[x, y] = new Entity(); // it is not on the new grid, though

        players[e.owner].param1++;

        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x + j, y) && Grid.blocking(x+j, y))
            {
                Entity en = Grid[x + j, y];
                Entity res = en;
                res.empty = true;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.type = 2;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x + j, y] = res;
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x - j, y) && Grid.blocking(x - j, y))
            {
                Entity en = Grid[x - j, y];
                Entity res = en;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.type = 2;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x - j, y] = new Entity();
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x, y + j) && Grid.blocking(x, y + j))
            {
                Entity en = Grid[x, y + j];
                Entity res = en;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.type = 2;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x, y + j] = res;
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x, y - j) && Grid.blocking(x, y - j))
            {
                Entity en = Grid[x, y - j];
                Entity res = en;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.type = 2;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x, y - j] = res;
                break;
            }
        }
    }

    public void DropBomb(int playerID)
    {
        if (!Grid.free(players[playerID].position))
            return;
            //throw new Exception($"droppin bomb by ({players[playerID]}) on top of ({Grid[players[playerID].position]})");
        bombs.Add(players[playerID].position);
        Grid[players[playerID].position] = new Entity(1, players[playerID].owner, bombTimer, players[playerID].param2, players[playerID].position);
        if (players[playerID].param1 > 0) players[playerID].param1--;
    }

    public bool Legal(IEnumerable<Move> moves)
    {
        int i = 0;
        foreach (var move in moves)
        {
            if ((!Grid.free(move.destination) && move.destination != players[i].position) || move.bomb && players[i].param1 == 0)
            {
                Console.Error.WriteLine($"Illegal move: {move}, from {players[i].position}, by {MCT.whomoves}");
                Console.Error.WriteLine($"Cell occupied by {Grid[move.destination]}");
                Console.Error.WriteLine($"InBounds: {Grid.InBounds(move.destination)}, Free: {Grid.free(move.destination)}");
                if (move.bomb && players[i].param1 == 0)
                    Console.Error.WriteLine("MOVE ILLEGAL CAUSE OF BOMB STUFF");
                return false;
            }
            i++;
        }
        return true;
    }

    public void Move(IEnumerable<Move> moves)
    {

        //TODO: this is probably really really really slowing things down
        //remove after the exception is no longer being thrown
        if (!Legal(moves))
            throw new Exception("wat");

        timer++;

        //all the booms are made to the copied grid
        Grid.CopyTo(copiedGrid);

        //tick the bombs
        for (int i = 0; i < bombs.Count; i++)
        {
            Entity b = Grid[bombs[i]];
            if (b.param1 > 0)
                b.param1--;
            copiedGrid[b.position] = b;
            if (b.param1 == 0)
            {
                if (b.param2 > 0)
                    Boom(b);
            }
        }
        bombs.RemoveAll(b => Grid[b].param2 == 0);

        //accept the state

        copiedGrid.CopyTo(Grid);

        //consider the moves on the regular grid
        int player = 0;
        foreach (Move move in moves)
        {
            if (move.bomb)
                DropBomb(player);
            players[player].position = move.destination;
            if (Grid[players[player].position].type == 2)
            {
                switch(Grid[players[player].position].param1)
                {
                    case 1:
                        players[player].param2++;                            
                        break;
                    case 2:
                        players[player].param1++;
                        break;
                }
            }
            player++;
        }
    }

    public IEnumerable<IEnumerable<Move>> LegalMoves()
    {
        IEnumerable<Point>[] playerMoves = new IEnumerable<Point>[playersAmm];
        for (int i = 0; i < playersAmm; i++)
        {
            Point pos = players[i].position;
            playerMoves[i] = pos.Neighbours().
                  Where(p => Grid.free(p)).Concat(Enumerable.Repeat(pos,1));
        }
        var res = playerMoves[0].Select(p => Enumerable.Repeat(new Move(false, p),1));
        if (players[0].param1 > 0)
            res = res.Concat(playerMoves[0].Select(p => Enumerable.Repeat(new Move(true, p), 1)));

        for (int i = 1; i < playersAmm; i++)
        {
            IEnumerable<Move> myMoves = playerMoves[i].Select(p => new Move(false, p) );
            if (players[i].param1 > 0)
                myMoves = myMoves.Concat(playerMoves[i].Select(p =>  new Move(true, p) ));

            res = res.SelectMany(a => myMoves, (rest, mine) => rest.Concat(Enumerable.Repeat(mine, 1)));
        }

        return res;

    }

    public void RandomMove()
    {
        Move[] moves = new Move[playersAmm];
        for (int i = 0; i < playersAmm; i++)
        {
            IEnumerable<Point> dests = players[i].position.Neighbours().
                  Where(p => Grid.free(p)).Concat(Enumerable.Repeat(players[i].position, 1));
            if (dests.Count() == 0)
            {
                Console.Error.WriteLine(this);
                foreach(Point p in players[i].position.Neighbours())
                {
                    Console.Error.WriteLine($"pos {p}: {Grid[p]}");
                }
                throw new Exception($"wat, player {i} can't move from {players[i].position}");
            }
                
            int m = MCT.random.Next(dests.Count());
            if (m >= dests.Count())
                throw new Exception($"WAT, m {m}, dests.count {dests.Count()}");
            if (i >= moves.Count())
                throw new Exception("WAT");
            moves[i].destination = dests.ElementAt(m);
            if (players[i].param1 > 0 && MCT.random.Next() % 2 == 0)
                moves[i].bomb = true;
            else
                moves[i].bomb = false;
        }
        MCT.whomoves = 1;
        Move(moves);
    }
    #endregion

    // board.Clear(), followed by board.Grid.UpdateRow() and board.UpdateEntity()
    // load a complete gamestate to the board
    #region utils
    public void Clear()
    {
        Move[] moves = new Move[playersAmm];
        for (int i = 0; i < playersAmm; i++)
        {
            moves[i].destination = players[i].position;
            moves[i].bomb = false;
            guessedMoves[i].bomb = false;
        }
        Move(moves);
        bombs.Clear();
    }

    

    public void UpdateEntity(Entity e)
    {
        if (e.type == 0)
        {
            guessedMoves[e.owner].destination = e.position;
            players[e.owner] = e;
        }
        else
        {

            if (e.type == 1)
            {
                if (e.param1 == 8)
                {
                    Console.Error.WriteLine($"new bomb of team {e.owner}");
                    guessedMoves[e.owner].bomb = true;
                }
                bombs.Add(e.position);
            }
            Grid[e.position] = e;
        }

    }

    public override String ToString()
    {
        Grid g = new Grid(Grid.width, Grid.height);
        Grid.CopyTo(g);
        StringBuilder res = new StringBuilder();
        res.AppendLine($"TIME: {timer}");
        foreach (Point bomb in bombs)
        {
            res.Append(Grid[bomb].ToString());
            res.AppendLine();
        }
        foreach (Entity player in players)
        {
            res.Append(player.ToString());
            res.Append($" SCORE: {scores[player.owner]}");
            res.AppendLine();
        }
        res.Append("\n\n");
        res.Append(g.ToString());
        return res.ToString();
    }
    #endregion
}

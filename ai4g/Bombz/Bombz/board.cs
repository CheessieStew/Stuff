﻿using System;
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
    public List<Entity> extraBombs;
    public Entity[] players;
    public int[] scores;
    public int[] upgrades;
    public int remainingBoxes;
    public bool Terminal
    {
        get { return remainingBoxes == 0; }
    }

    public Grid copiedGrid;

    public Board(uint width, uint height, int playersAmm)
    {
        Grid = new Grid(width, height);
        copiedGrid = new Grid(width, height);
        this.playersAmm = playersAmm;

        bombs = new List<Point>();
        extraBombs = new List<Entity>();
        players = new Entity[playersAmm];
        upgrades = new int[playersAmm];
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
        res.extraBombs = extraBombs.ConvertAll(b => b);

        upgrades.CopyTo(res.upgrades, 0);
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
        if (e.type != 1)
            throw new Exception($"can't boom {e}");
        if (e.isExtra && Grid[e.position].param2 > 0)
        {
            // because of <insert reasons>
            // we want to trigger the main bomb first
            // and let IT trigger all the extras
            Boom(Grid[e.position]);
            return;
        }

        int j;
        Entity bomb = e;
        int x = (int)e.xPos;
        int y = (int)e.yPos;
        bomb.param2 = 0;
        if (!e.isExtra)
        {
            Grid[bomb.position] = bomb; //bomb stays on the grid, but we reduce it's range to 0
            copiedGrid[x, y] = new Entity(); // it is not on the new grid, though
        }
        //no need to set the extra's range to 0
        //it can only get triggered if the main bomb on this cell gets triggered

        if (copiedGrid[x, y].empty != true)
            throw new Exception("GOTCHA");
        players[e.owner].param1++;

        if (e.extra < extraBombs.Count) //this bomb has another one underneath
        {
            if (extraBombs[(int)e.extra].param2 > 0)
                Boom(extraBombs[(int)e.extra]);
        }

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
                        if (res.position != new Point(x + j, y))
                            throw new Exception($"wow! {res.position} instead of {x + j}, {y}");
                        res.empty = false;
                        res.isExtra = false;
                        res.extra = 2047;
                        res.type = 2;
                        res.param2 = 0;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x + j, y] = res; //if we did a boom, it will be empty anyway
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x - j, y) && Grid.blocking(x - j, y))
            {
                Entity en = Grid[x - j, y];
                Entity res = en;
                res.empty = true;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.isExtra = false;
                        res.extra = 2047;
                        res.type = 2;
                        res.param2 = 0;
                    }
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    Boom(en);
                // item? simply destroy
                copiedGrid[x - j, y] = res;
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x, y + j) && Grid.blocking(x, y + j))
            {
                Entity en = Grid[x, y + j];
                Entity res = en;
                res.empty = true;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.isExtra = false;
                        res.extra = 2047;
                        res.type = 2;
                        res.param2 = 0;
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
                res.empty = true;
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    if (en.param1 > 0)
                    {
                        res.empty = false;
                        res.isExtra = false;
                        res.extra = 2047;
                        res.type = 2;
                        res.param2 = 0;
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

    /*
    public void FalseBoom(Entity e)
    {
        if (e.param2 == 0) //it's a bomb's ghost
        {
            Console.Error.WriteLine("GHOST TRIGGERED");
            return;
        }
        if (e.type != 1)
            throw new Exception($"can't boom {e}");
        if (e.isExtra && Grid[e.position].param2 > 0)
        {
            // because of <insert reasons>
            // we want to trigger the main bomb first
            // and let IT trigger all the extras
            FalseBoom(Grid[e.position]);
            return;
        }

        int j;
        Entity bomb = e;
        int x = (int)e.xPos;
        int y = (int)e.yPos;
        bomb.param2 = 0;
        if (!e.isExtra)
        {
            Grid[bomb.position] = bomb; //bomb stays on the grid, but we reduce it's range to 0
        }

        if (e.extra < extraBombs.Count) //this bomb has another one underneath
        {
            if (extraBombs[(int)e.extra].param2 > 0)
                FalseBoom(extraBombs[(int)e.extra]);
        }

        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x + j, y) && Grid.blocking(x + j, y))
            {
                Entity en = Grid[x + j, y];
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    FalseBoom(en);
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x - j, y) && Grid.blocking(x - j, y))
            {
                Entity en = Grid[x - j, y];
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;

                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    FalseBoom(en);

                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x, y + j) && Grid.blocking(x, y + j))
            {
                Entity en = Grid[x, y + j];
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    FalseBoom(en);
                break;
            }
        }
        for (j = 1; j < e.param2; j++)
        {
            if (Grid.InBounds(x, y - j) && Grid.blocking(x, y - j))
            {
                Entity en = Grid[x, y - j];
                if (en.type == 3) //box? score
                {
                    scores[e.owner]++;
                    remainingBoxes--;
                    
                }
                else if (en.type == 1 && en.param2 > 0) //bomb? boom
                    FalseBoom(en);
                break;
            }
        }
    }
    */

    public void DropBomb(int playerID)
    {
        Entity newBomb = new Entity(1, players[playerID].owner, bombTimer, players[playerID].param2, players[playerID].position);
        if (newBomb.param2 == 0)
            Console.Error.WriteLine($"{newBomb} should not exist");
        if (players[playerID].param1 > 0) players[playerID].param1--;
        if (!Grid.free(players[playerID].position))
        {
            if (Grid[players[playerID].position].type != 1)
                throw new Exception("Throwing bomb on top of not-bomb");
            if (Grid[players[playerID].position].param1 != 8)
                throw new Exception("Throwing bomb on top of not-fresh bomb");
            Entity theMain = Grid[players[playerID].position];
            newBomb.isExtra = true; // the new bomb is an extra
            newBomb.extra = theMain.extra; // point the new bomb to whatever the main one was pointing to
            extraBombs.Add(newBomb); // add the new bomb to the extra list
            theMain.extra = (uint)(extraBombs.Count - 1); // point the main bomb to the new bomb
            Grid[theMain.position] = theMain; // update the main bomb
        }
        else
        {
            bombs.Add(players[playerID].position);
            newBomb.extra = 2047; //2047 if there's no other bomb on this cell, index on extraBombs if there is one
            Grid[players[playerID].position] = newBomb;
        }
        //only the main bomb exists on the bombs list
    }

    public bool Legal(IEnumerable<Move> moves)
    {
        int i = 0;
        foreach (var move in moves)
        {
            if ((!Grid.free(move.destination) && move.destination != players[i].position) || move.bomb && (players[i].param1 == 0 || (!Grid.free(players[i].position) && Grid[players[i].position].param1 != 8)))
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
            if (b.param2 > 0)
                copiedGrid[b.position] = b;
            if (b.param1 == 0)
            {
                if (b.param2 > 0)
                {
                    try
                    {
                        Boom(b);
                    }
                    catch(Exception ex)
                    {
                        Console.Error.WriteLine(ex.StackTrace);
                        Console.Error.WriteLine($"many bombs, tried booming {b}");
                        Console.Error.WriteLine(this);
                        throw ex;
                    }
                }
            }
        }
        //tick the extras
        for (int i = 0; i < extraBombs.Count; i++)
        {
            Entity b = extraBombs[i];
            if (b.param1 > 0)
                b.param1--;
            if (b.param1 == 0)
            {
                if (Grid[b.position].param2 > 0)
                {
                    try
                    {
                        Boom(Grid[b.position]); //both are gonna explode anyway
                                                // but if we trigger only the main bomb
                                                // we have a guarantee of no duplicates, cause it can explode only once
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.StackTrace);
                        
                        Console.Error.WriteLine($"many bombs, tried booming {Grid[b.position]}");
                        Console.Error.WriteLine(this);
                        throw ex;
                    }
                }
            }
            extraBombs[i] = b;
        }

        // O(n^2), but n is almost always 0, disgustingly rarely bigger than 1
        for (int i = 0; i < extraBombs.Count; i++)
        {
            // an extra can only explode when it's main bomb explodes, and vice-versa
            if (Grid[extraBombs[i].position].param2 == 0)
            {
                for (int j = 0; j < extraBombs.Count; j++)
                {
                    if (extraBombs[j].extra > i && extraBombs[j].extra < extraBombs.Count)
                    {
                        Entity e = extraBombs[j];
                        e.extra--;
                        extraBombs[j] = e; //each pointer pointing to the right of what we're removing
                        // must get lower by 1
                    }
                    if (j > i && copiedGrid[extraBombs[j].position].extra==j)
                    {
                        Entity e = copiedGrid[extraBombs[j].position];
                        e.extra--;
                        copiedGrid[extraBombs[j].position] = e;
                        // if we're moving and are directly above a non-extra bomb
                        // update the non-extra bomb
                    }
                }
                extraBombs.RemoveAt(i);
                i--;
            }
        }
        bombs.RemoveAll(b =>
        {
            if (Grid[b].param2 == 0 && !copiedGrid.free(b) && copiedGrid[b].type != 2)
            {
                Console.Error.WriteLine($"ok, {copiedGrid[b]} has problems, time {timer}");
                throw new Exception($"ok, {copiedGrid[b]} has problems");
            }
            return Grid[b].param2 == 0;
        });

        //accept the state

        copiedGrid.CopyTo(Grid);

        //consider the moves on the regular grid
        int player = 0;
        foreach (Move move in moves)
        {
            try
            {
                if (move.bomb)
                    DropBomb(player);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(this);
                Console.Error.WriteLine($"bad move: {move}");
                throw e;
            }
            players[player].position = move.destination;
            if (!Grid[players[player].position].empty && Grid[players[player].position].type == 2)
            {
                switch(Grid[players[player].position].param1)
                {
                    case 1:
                        players[player].param2++;
                        upgrades[player]++;
                        break;
                    case 2:
                        players[player].param1++;
                        upgrades[player]++;
                        break;
                }
            }
            player++;
        }
        foreach (Entity p in players)
            if (Grid[p.position].type == 2)
                Grid[p.position] = new Entity();
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
        if (players[0].param1 > 0 && Grid.free(players[0].position))
            res = res.Concat(playerMoves[0].Select(p => Enumerable.Repeat(new Move(true, p), 1)));

        for (int i = 1; i < playersAmm; i++)
        {
            IEnumerable<Move> myMoves = playerMoves[i].Select(p => new Move(false, p) );
            if (players[i].param1 > 0 && Grid.free(players[i].position))
                myMoves = myMoves.Concat(playerMoves[i].Select(p =>  new Move(true, p) ));

            res = res.SelectMany(a => myMoves, (rest, mine) => rest.Concat(Enumerable.Repeat(mine, 1)));
        }

        return res;

    }

    public int BoxesInRange(Point p, int range)
    {
        int res = 0;
        bool right = true, left = true, up = true, down = true;
        for (int r = 1; r < range; r++)
        {
            if (right && Grid.InBounds(p.x + r, p.y) && Grid.blocking(p.x + r, p.y))
            {
                right = false;
                if (Grid[p.x + r, p.y].type == 3)
                    res++;
            }
            if (left && Grid.InBounds(p.x - r, p.y) && Grid.blocking(p.x - r, p.y))
            {
                left = false;
                if (Grid[p.x - r, p.y].type == 3)
                    res++;
            }
            if (up && Grid.InBounds(p.x, p.y - r) && Grid.blocking(p.x, p.y - r))
            {
                up = false;
                if (Grid[p.x, p.y - r].type == 3)
                    res++;
            }
            if (down && Grid.InBounds(p.x, p.y + r) && Grid.blocking(p.x, p.y + r))
            {
                down = false;
                if (Grid[p.x, p.y + r].type == 3)
                    res++;
            }
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
            if (m == 0)
                m = MCT.random.Next(dests.Count());
            moves[i].destination = dests.ElementAt(m);
            
            if (MCT.random.Next() % 2 == 0  
                && players[i].param1 > 0 
                && Grid.free(players[i].position)
                && BoxesInRange(players[i].position, (int)players[i].param2) > 0)
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
       // Move[] moves = new Move[playersAmm];
        for (int i = 0; i < playersAmm; i++)
        {
            //moves[i].destination = players[i].position;
           // moves[i].bomb = false;
            guessedMoves[i].bomb = false;
        }
        Grid.CopyTo(copiedGrid);
        foreach(Point bomb in bombs)
            if (Grid[bomb].param1 == 1 && Grid[bomb].param2 > 0)
            {
                Boom(Grid[bomb]);
            }
        foreach (Entity bomb in extraBombs)
            if (bomb.param1 == 1 && bomb.param2 > 0)
            {
                Boom(bomb);
            }
        copiedGrid.CopyTo(Grid);
        bombs.Clear();
        extraBombs.Clear();
        this.timer++;
    }

    public void UpdateEntity(Entity e)
    {

        if (e.type == 0)
        {
            guessedMoves[e.owner].destination = e.position;
            Console.Error.WriteLine($"Player {e.owner} moves onto {Grid[e.position]}");
            if (!Grid[e.position].empty && Grid[e.position].type == 2)
                upgrades[e.owner]++;

            players[e.owner] = e;
        }
        else
        {
            if (e.type == 1)
            {
                if (e.param1 == 8)
                {
                    guessedMoves[e.owner].bomb = true;
                }
                bombs.Add(e.position);
            }
            else
            { 
                if (e.type == 2)
                {
                    e.owner = 0;
                    e.param2 = 0;
                }
            }
            copiedGrid[e.position] = e;
        }

    }

    public static bool operator ==(Board b1, Board b2)
    {
        if (b1.bombs.Count != b2.bombs.Count)
        {
            Console.Error.WriteLine($"bc {b1.bombs.Count} != {b2.bombs.Count}");
            return false;
        }
        for (int i = 0; i < b1.bombs.Count; i++)
        {
            if (b1.bombs[i] != b2.bombs[i])
            {
                Console.Error.WriteLine($"b {b1.bombs[i]} != {b2.bombs[i]}");
                return false;
            }
        }
        for (int i = 0; i < b1.playersAmm; i++)
        {
            if (b1.players[i] != b2.players[i])
            {
                Console.Error.WriteLine($"p {b1.players[i]} != {b2.players[i]}");
                return false;
            }
            if (b1.upgrades[i] != b2.upgrades[i])
            {
                Console.Error.WriteLine($"u {b1.upgrades[i]} != {b2.upgrades[i]}");
                return false;
            }
            if (b1.scores[i] != b2.scores[i])
            {
                Console.Error.WriteLine($"s {b1.scores[i]} != {b2.scores[i]}");
                return false;
            }
        }
        if (b1.extraBombs.Count != b2.extraBombs.Count)
        {
            Console.Error.WriteLine($"exc {b1.extraBombs.Count} != {b2.extraBombs.Count}");
            return false;
        }
        for (int i = 0; i < b1.extraBombs.Count; i++)
        {
            if (b1.extraBombs[i] != b2.extraBombs[i])
                if (b1.bombs[i] != b2.bombs[i])
                {
                    Console.Error.WriteLine($"ex {b1.extraBombs[i]} != {b2.extraBombs[i]}");
                    return false;
                }
        }
        for (int y = 0; y < b1.Grid.height; y++)
            for (int x = 0; x < b1.Grid.width; x++)
                if (b1.Grid[x, y] != b2.Grid[x, y])
                {
                    Console.Error.WriteLine($"g at {x},{y} {b1.Grid[x,y]} != {b2.Grid[x,y]}");
                    return false;
                }

        return true;
        
    }

    public static bool operator !=(Board b1, Board b2)
    {
        return !(b1 == b2);
    }

    public override String ToString()
    {
        Grid g = new Grid(Grid.width, Grid.height);
        Grid.CopyTo(g);
        StringBuilder res = new StringBuilder();
        res.AppendLine($"TIME: {timer}");
       
        foreach (Point bomb in bombs)
        {
            res.Append(Grid[bomb]);
            res.AppendLine();
        }
        foreach (Entity bomb in extraBombs)
        {
            res.Append("EXTRA: ");
            res.Append(bomb);
            res.AppendLine();
        }
        foreach (Entity player in players)
        {
            res.Append(player.ToString());
            res.Append($" SCORE: {scores[player.owner]}");
            res.Append($" U: {upgrades[player.owner]}");

            res.AppendLine();
        }
        res.Append("\n\n");
        res.Append(g.ToString());
        return res.ToString();
    }
    #endregion
}
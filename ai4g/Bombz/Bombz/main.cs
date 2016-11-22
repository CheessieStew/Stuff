﻿using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/




class Player
{

    static void Main(string[] args)
    {
        Entity e1 = new Entity(1, 1, 1, 1, new Point(1, 1));
        Entity e2 = new Entity(1, 1, 1, 1, new Point(1, 1));
        DateTime deadline = DateTime.Now.AddMilliseconds(980);
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        uint width = uint.Parse(inputs[0]);
        uint height = uint.Parse(inputs[1]);
        int myId = int.Parse(inputs[2]);

        Board board = new Board(width, height, 2);

        //first turn
        for (int i = 0; i < height; i++)
        {
            string row = Console.ReadLine();
            foreach (char c in row)
                if (c != '.')
                    board.remainingBoxes++;
            board.Grid.UpdateRow(i, row);
        }

        int entities = int.Parse(Console.ReadLine());
        for (int i = 0; i < entities; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            Entity e = new Entity();
            e.empty = false;
            e.extra = 2047;
            e.isExtra = false; e.type = uint.Parse(inputs[0]);
            e.owner = uint.Parse(inputs[1]);
            e.xPos = uint.Parse(inputs[2]);
            e.yPos = uint.Parse(inputs[3]);
            e.param1 = uint.Parse(inputs[4]);
            e.param2 = uint.Parse(inputs[5]);
            board.UpdateEntity(e);
        }

        //TestSim(200, board);
        for (int i = 0; i < board.playersAmm; i++)
            board.upgrades[i] = 0;
        Console.Error.WriteLine(board);

        MCT mct = new MCT(board, myId);


        Console.WriteLine(mct.Run(deadline));

        // game loop
        while (true)
        {
            deadline = DateTime.Now.AddMilliseconds(85);
            board.Clear();
            for (int i = 0; i < height; i++)
            {
                string row = Console.ReadLine();
                board.copiedGrid.UpdateRow(i, row);
            }
            entities = int.Parse(Console.ReadLine());
            for (int i = 0; i < entities; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                Entity e = new Entity();
                e.empty = false;
                e.extra = 2047;
                e.isExtra = false;
                e.type = uint.Parse(inputs[0]);
                e.owner = uint.Parse(inputs[1]);
                e.xPos = uint.Parse(inputs[2]);
                e.yPos = uint.Parse(inputs[3]);
                e.param1 = uint.Parse(inputs[4]);
                e.param2 = uint.Parse(inputs[5]);
                board.UpdateEntity(e);
            }
            board.copiedGrid.CopyTo(board.Grid);
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
            mct.Reuse(board.guessedMoves);
            Console.WriteLine(mct.Run(deadline));

            Console.Error.WriteLine($"{(deadline-DateTime.Now).Milliseconds}ms left");

        }
    }

}


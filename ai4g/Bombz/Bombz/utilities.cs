using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

struct Entity : IEquatable<Entity>
{
    private uint _val;
    public Point position
    {
        get
        {
            return new Point(xPos, yPos);
        }
        set
        {
            yPos = (uint)value.y;
            xPos = (uint)value.x;
        }

    }

    // get: << (31-left) >> (31 - left + right)
    // set: << right

    public bool Equals(Entity e)
    {
        return (_val << 12) == (e._val << 12);
    }


    public bool empty //0-1, bit 19
    {
        //0 is empty
        get
        {
            return 0 == ((_val << 12) >> 31);
        }
        set
        {
            if (value)
                _val &= ~(uint)(1 << 19);
            else
                _val |= (1 << 19);
        }
    }
   
    public uint yPos // 0-15, bits 18 to 15
    {
        get
        {
            return (_val << 13) >> 28;
        }
        set
        {
            if (value > 15)
                throw new Exception("ypos is 0-15");
            _val &= ~(uint)(15 << 15);
            _val |= value << 15;
        }
    }
    public uint xPos // 0-15, bits 14 to 11
    {
        get
        {
            return (_val << 17) >> 28;
        }
        set
        {
            if (value > 15)
                throw new Exception("xpos is 0-15");
            _val &= ~(uint)(15 << 11);
            _val |= value << 11;
        }
    }

    /// <summary>
    /// 0 is player, 1 is bomb, 2 is item, 3 is box
    /// </summary>
    ///
    public uint type // 0-3, bits 10 to 9
    {
        get
        {
            return (_val << 21) >> 30;
        }
        set
        {
            if (value > 3)
                throw new Exception("type is 0-3");
            _val &= ~(uint)(3 << 9);
            _val |= value << 9;
        }
    }

    /// <summary>
    /// id of the player/player who put the bomb
    /// </summary>
    public uint owner // 0-1, 8th bit
    {
        get
        {
            return (_val << 23) >> 31;
        }
        set
        {
            if (value > 1)
                throw new Exception("owner is 0-1");
            _val &= ~(uint)(1 << 8);
            _val |= value << 8;
        }
    }

    /// <summary>
    /// bombs remaining / bomb timer / item type
    /// </summary>
    public uint param1 // 0-15, bits 7 to 4
    {
        get
        {
            return (_val << 24) >> 28;
        }
        set
        {
            if (value > 15)
                throw new Exception("param1 is 0-15");
            _val &= ~(uint)(15 << 4);
            _val |= value << 4;
        }
    }

    /// <summary>
    /// explosion range
    /// </summary>
    public uint param2 //0-15, bits 3 to 0
    {
        get
        {
            return (_val << 28) >> 28;
        }
        set
        {
            if (value > 15)
                throw new Exception("param2 is 0-15");
            _val &= ~(uint)15;
            _val |= value;
        }
    }

    public Entity(uint t, uint own, uint p1, uint p2, Point position)
    {
        _val = 0;
        this.position = position;
        type = t; owner = own; param1 = p1; param2 = p2;
        empty = false;
    }


    public override string ToString()
    {

        String t = "";
        if (empty)
            t = "(empty)";
        switch(type)
        {
            case 0:
                t+= "player";
                break;
            case 1:
                t+= "bomb";
                break;
            case 2:
                t+= "item";
                break;
        }
        return $"{t} of team {owner} at {position}, p1 {param1}, p2 {param2}";
    }
}

struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Point(uint x, uint y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    public IEnumerable<Point> Neighbours()
    {
        yield return new Point(x - 1, y);
        yield return new Point(x + 1, y);
        yield return new Point(x, y - 1);
        yield return new Point(x, y + 1);
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }

    public static bool operator !=(Point p1, Point p2)
    {
        return (p1.x != p2.x || p1.y != p2.y);
    }

    public static bool operator ==(Point p1, Point p2)
    {
        return (p1.x == p2.x && p1.y == p2.y);
    }

}

struct Move
{
    public bool bomb;
    public Point destination;

    public Move(bool b, Point p)
    {
        bomb = b;
        destination = p;
    }

    public static bool operator !=(Move m1, Move m2)
    {
        return (m1.bomb != m2.bomb || m1.destination != m2.destination);
    }

    public static bool operator ==(Move m1, Move m2)
    {
        return (m1.bomb == m2.bomb && m1.destination == m2.destination);
    }

    public override string ToString()
    {
        String comm = bomb ? "BOMB" : "MOVE";
        return $"{comm} {destination.x} {destination.y}";
    }
}

class Grid
{
    Entity[,] ents;
    public uint height { get; private set; }
    public uint width { get; private set; }

    /// <summary>
    /// true if a player can move on this cell, false otherwise
    /// </summary>
    public bool free(int x, int y)
    {
        // false if cell out of bounds, is a box, or is a bomb
        return (InBounds(x, y) && (ents[y,x].empty || (ents[y, x].type != 3 && ents[y, x].type != 1)));
    }
    public bool free(Point p)
    {
        // false if cell out of bounds, is a box, or is a bomb
        return (InBounds(p) && (ents[p.y, p.x].empty || (ents[p.y, p.x].type != 3 && ents[p.y, p.x].type != 1)));
    }

    /// <summary>
    /// true if this cell stops a bomb's explosion
    /// </summary>
    public bool blocking(int x, int y)
    {
        // true if cell out of bounds, is a box, an item, or a bomb
        return (ents[y, x].type != 0 && !ents[y,x].empty);
    }
    public bool blocking(Point p)
    {
        // true if cell out of bounds, is a box, or is a bomb
        return (ents[p.y, p.x].type != 0);
    }

    public Entity this[Point p]
    {
        get
        {
            return ents[p.y, p.x];
        }
        set
        {
            ents[p.y, p.x] = value;
        }
    }

    public Entity this[int x, int y]
    {
        get
        {
            if (!InBounds(x, y))
                throw new Exception("blah");
            return ents[y, x];
        }
        set
        {
            ents[y, x] = value;
        }
    }

    public override String ToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (this[j, i].empty)
                    builder.Append(".");
                else if (this[j, i].type == 3)
                {
                    builder.Append(this[j, i].param1);
                }
                else if (this[j,i].type == 1)
                {
                    builder.Append("b");
                }
                else if (this[j,i].type == 2)
                {
                    if (this[j, i].param1 == 1)
                        builder.Append("*");
                    else if (this[j, i].param1 == 2)
                        builder.Append("+");
                    else
                        builder.Append("?");
                }

            }
            builder.AppendLine();
        }
        return builder.ToString();
    }

    public bool InBounds(Point p)
    {
        return (p.x >= 0 && p.x < width && p.y >= 0 && p.y < height);
    }
    public bool InBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }
    public bool InBounds(uint x, uint y)
    {
        return (x < width && y < height);
    }
    public Grid(uint width, uint height)
    {
        this.width = width;
        this.height = height;
        ents = new Entity[height,width];
    }
    public void CopyTo(Grid grid)
    {
        
        Array.Copy(ents, grid.ents, width * height);
    }

    public void UpdateRow(int which, String row)
    {
        for (int i = 0; i < row.Length; i++)
        {
            Entity t = new Entity();
            if (row[i] == '.')
                t.empty = true;
            else
            {
                t.empty = false;
                t.type = 3;
                t.param1 = (uint)(row[i] - 48);
            }
            ents[which, i] = t;
        }

    }
}

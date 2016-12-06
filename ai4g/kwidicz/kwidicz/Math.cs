using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


struct Vector2
{
    public float x;
    public float y;

    public Vector2(int x = -1, int y = -1)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2(float x = -1, float y = -1)
    {
        this.x = x;
        this.y = y;
    }


    public Vector2(double x = -1, double y = -1)
    {
        this.x = (float)x;
        this.y = (float)y;
    }

    public static Vector2 operator *(float a, Vector2 v) => new Vector2(v.x * a, v.y * a);
    

    public static Vector2 operator *(Vector2 v, float a) => new Vector2(v.x * a, v.y * a);
    
    public static Vector2 operator /(Vector2 v, float a) => new Vector2(v.x / a, v.y / a);
    

    public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;

    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);

    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);


    public bool Equals(Vector2 other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Vector2 && Equals((Vector2)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (int)((x * 20000) + y);
        }
    }

    public static float Square(float x) => x * x;

    public float DistanceTo(Vector2 other)
        => (float)Math.Sqrt(Square(other.x - x) + Square(other.y - y));

    public float AngleTo(Vector2 other)
    {
        float angle = (float)( Math.Atan2(other.y, other.x) - Math.Atan2(y, x));
        if (angle < 0)
            angle += 2 * (float)Math.PI;
        return angle;
    }

    public float Angle(Vector2 other)
    {
        float angle = AngleTo(other);
        if (angle > Math.PI)
            return 2 * (float) Math.PI - angle;
        else
            return angle;
    }

    public override string ToString() => $"({x},{y})";

    public double DistanceToLine(Vector2 a, Vector2 b)
    {
        var up = (b.y - a.y) * x - (b.x - a.x) * y + b.x * a.y - b.y * a.x;
        return Math.Abs(up) / a.DistanceTo(b);
    }

    public bool InCone(Vector2 thin, Vector2 thick, float thickness)
    {
        var dist = DistanceToLine(thin, thick);
        var touch = TouchPointOnLine(thin, thick);
        if (!touch.Between(thin, thick))
            return false;
        var howfar = thin.DistanceTo(touch) / thin.DistanceTo(thick);
        return (dist < thickness * howfar);
    }

    public bool InCone(Vector2 thin, Vector2 thick, float thickness1, float thickness2)
    {
        if (DistanceTo(thin) < thickness1)
            return true;
        var dist = DistanceToLine(thin, thick);
        var touch = TouchPointOnLine(thin, thick);
        if (touch.Between(thin, thick))
        {
            var howfar = thin.DistanceTo(touch) / thin.DistanceTo(thick);
            //Console.Error.WriteLine($"point's distance is {dist}");
            return (dist < thickness1 * (1 - howfar) + thickness2 * howfar);
        }
        else
        {
            //Console.Error.WriteLine($"prob with {this}: {touch} is not between {thin} and {thick}");
            //Console.Error.WriteLine($"x cond: {Math.Min(thin.x, thick.x) <= x && x <= Math.Max(thin.x, thick.x)}, y cond: {Math.Min(thin.y, thick.y) <= y && y <= Math.Max(thin.y, thick.y)}");
            return false;
        }
    }

    public bool Between(Vector2 a, Vector2 b)
    {

        return (Math.Min(a.x, b.x) <= x && x <= Math.Max(a.x, b.x) &&
                Math.Min(a.y, b.y) <= y && y <= Math.Max(a.y, b.y));
    }

    public float Magnitude
    {
        get
        {
            return (DistanceTo(new Vector2(0, 0)));
        }
    }

    public Vector2 Rotate(float angle)
    {
        return new Vector2(Math.Cos(angle) * x + Math.Sin(angle) * y, Math.Cos(angle) * y - Math.Sin(angle) * x);
    }

    public Vector2 Normalized
    {
        get
        {
            if (Magnitude != 0)
                return this / Magnitude;
            else
                return this;
        }
    }
    public Vector2 PerpenNorm
    {
        get
        {
            if (Magnitude != 0)
            {

                return new Vector2(this.y, -this.x).Normalized;
            }
            else
                return this;
        }
    }

    public Vector2 TouchPointOnLine(Vector2 a, Vector2 b)
    {
        var down = Square(b.y - a.y) + Square(b.x - a.x);
        var aa = (b.y - a.y);
        var bb = -(b.x - a.x);
        var cc = b.x * a.y - b.y * a.x;
        return new Vector2((bb * (bb * x - aa * y) - aa * cc) / down, (aa * (-bb*x + aa * y) - bb * cc )/ down);
    }
}


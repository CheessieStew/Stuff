using System;
using JSIL;
using JSIL.Meta;


public static class Program
{

    public static int Width = 400;
    public static int Height = 400;
    public static int ClockRadius = 180;

    public static float seconds;
    public static float minutes;
    public static float hours;

    public static int cx, cy;
    public static double kat1;
    public static double kat2;

    public static void Main()
    {
        dynamic document = Builtins.Global["document"];
        dynamic window = Builtins.Global["window"];

        var body =
          document.getElementsByTagName("body")[0];
        var canvas = document.createElement("canvas");
        canvas.width = Width;
        canvas.height = Height;
        var ctx = canvas.getContext("2d");
        var img = ctx.createImageData(Width, Height);

        body.appendChild(canvas);

        window.setInterval(
          (Action)(
            () =>
            {
                Loop();
                Redraw(ctx);
            }),
          200);
    }

    public static void Loop()
    {
        float angle  = (float)(2 * Math.PI / 60.0f * 0.2);
        seconds += angle;
        angle /= 60;
        minutes += angle;
        angle /= 60;
        hours += angle;

    }

    public static void Redraw(
      dynamic ctx)
    {
        
        ctx.fillStyle = "rgb(200,200,200)";
        ctx.clearRect(0, 0, Width, Height);
        ctx.fillRect(0, 0, Width, Height);
        ctx.lineCap = "round";
        ctx.lineWidth = 2.0f;
        DrawHand(ctx, ClockRadius * 0.9f, seconds);
        ctx.lineWidth = 4.0f;
        DrawHand(ctx, ClockRadius * 0.8f, minutes);
        ctx.lineWidth = 6.0f;
        DrawHand(ctx, ClockRadius * 0.5f, hours);

        ctx.lineWidth = 2.0f;
        DrawClock(ctx);


        //ctx.putImageData(img, 0, 0);
    }



    public static void DrawHand(dynamic ctx, float length, float angle)
    {

        int x = (int)(Math.Sin(angle) * length);
        int y = -(int)(Math.Cos(angle) * length);
        ctx.beginPath();
        ctx.moveTo(Width / 2, Height / 2);
        ctx.lineTo(Width / 2 + x, Height / 2 + y);
        ctx.stroke();
        ctx.closePath();
    }

    public static void DrawClock(dynamic ctx)
    {
        float angle = (float)(Math.PI / 6);
        for (int i = 0; i < 12; i++)
        {
            float x = (float)(Math.Sin(angle * i));
            float y = (float)(Math.Cos(angle * i));
            ctx.beginPath();
            ctx.moveTo(Width / 2 + (int)(x * 0.95f * ClockRadius),
                       Height / 2 + (int)(y * 0.95 * ClockRadius));

            ctx.lineTo(Width / 2 + (int)(x * ClockRadius),
                       Height / 2 + (int)(y * ClockRadius));
            ctx.stroke();
            ctx.closePath();
        }
    }
}

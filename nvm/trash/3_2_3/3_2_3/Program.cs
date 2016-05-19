using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace _3_2_3
{
    public class CMainForm : Form
    {
        Timer timer;


        public CMainForm()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(TickTock);
            timer.Interval = 1000;
            timer.Start();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

        }

        void TickTock(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen myPen = new Pen(Color.Black,4);
            double radius = Math.Min(Width, Height -6) / 3;
            Point center = new Point((int)radius +10,(int)radius +10);
            
            e.Graphics.DrawEllipse(myPen, new Rectangle(10, 10, (int)radius*2, (int)radius*2));
            double hourAngle = 2*Math.PI*(DateTime.Now.Hour % 12) / 12.0;
            double minuteAngle = 2*Math.PI*(DateTime.Now.Minute % 60) / 60.0;
            Point hourP = new Point((int)(Math.Sin(hourAngle) * radius * 0.5) + center.X, -(int)(Math.Cos(hourAngle) * radius * 0.5) + center.Y);
            Point minuteP = new Point((int)(Math.Sin(minuteAngle) * radius * 0.8) + center.X, -(int)(Math.Cos(minuteAngle) * radius * 0.8) + center.Y);

            Font font = new Font("LED", 10);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(DateTime.Now.ToShortTimeString(), font, Brushes.Gray, center.X, center.Y + 20, sf);
            e.Graphics.DrawLine(myPen, center, hourP);
            e.Graphics.DrawLine(myPen, center, minuteP);
        }

        public static void Main()
        {
            Application.Run(new CMainForm());
        }
    }
}

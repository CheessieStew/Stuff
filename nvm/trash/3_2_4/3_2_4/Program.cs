using System;

using System.Windows.Forms;
using System.Drawing;
namespace _3_2_4
{
    public class SmoothProgressBar : Control
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        private int _value;
        public int Value { 
            get
            {
                return _value;
            }
            set
            {
                this.Invalidate();
                if (value < Minimum || value > Maximum)
                    throw new ArgumentOutOfRangeException();
                _value = value;
            }
        }

        public SmoothProgressBar() : base()
        {
            Size = new Size(32500, 20);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Pen myPen = new Pen(Color.Black);

            double width = (double)(Width-2) * (Value - Minimum) / (Maximum - Minimum);
            if (Maximum == Minimum) width = Width - 2;
            System.Drawing.Drawing2D.LinearGradientBrush myBrush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(1, 1, (int)width, Height - 2), Color.Gray, Color.Black,0.0f);

            e.Graphics.FillRectangle(myBrush, new Rectangle(1,1,(int)width,Height-2));
            e.Graphics.DrawRectangle(myPen, new Rectangle(0, 0, Width-1, Height-1));
        }

    }
    public class CMainForm : Form
    {

        Button plusB;
        Button minusB;
        ProgressBar progBar;
        SmoothProgressBar smoothBar;

        public CMainForm()
        {
            this.Text = "Paseczek";
            this.Size = new Size(600, 300);


            minusB = new Button();
            minusB.Text = "-";
            minusB.Click += new EventHandler(MinusHandler);
            minusB.Location = new Point(40, 220);

            plusB = new Button();
            plusB.Text = "+";
            plusB.Click += new EventHandler(PlusHandler);
            plusB.Location = new Point(500, 220);

            progBar = new ProgressBar();
            progBar.Minimum = 0;
            progBar.Maximum = 100;
            progBar.Value = 50;
            progBar.Location = new Point(50, 50);
            progBar.Width = 500;

            smoothBar = new SmoothProgressBar();
            smoothBar.Minimum = 0;
            smoothBar.Maximum = 100;
            smoothBar.Value = 50;
            smoothBar.Location = new Point(50, 100);
            smoothBar.Width = 500;
            

            this.Controls.AddRange(new Control[] { progBar, smoothBar, minusB, plusB });
        }

        private void PlusHandler(object sender, EventArgs e)
        {
            if (progBar.Value < progBar.Maximum)
                progBar.Value++;
            if (smoothBar.Value < smoothBar.Maximum)
            smoothBar.Value++;
        }

        private void MinusHandler(object sender, EventArgs e)
        {
            if (progBar.Value > progBar.Minimum)
                progBar.Value--;
            if (smoothBar.Value > smoothBar.Minimum)
                smoothBar.Value--;
        }

        public static void Main()
        {
            Application.Run(new CMainForm());
        }
    }
}

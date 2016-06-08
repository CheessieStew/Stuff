using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Timers;

namespace _3_3_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int rise;
        ProgressBar[] bars;
        Timer timer;

        public MainWindow()
        {
            InitializeComponent();
            rise = 1;
            slider.Minimum = 1;
            slider.Maximum = 100;
            slider.Value = 10;
            bars = new ProgressBar[4] { PBar0, PBar1, PBar2, PBar3 };

            foreach (var bar in bars)
            {
                bar.Minimum = 0;
                bar.Maximum = 50;
                bar.Value = 0;
            }
            timer = new Timer();
            timer.Interval = 10;
            timer.Elapsed += new ElapsedEventHandler(TickTock);
            timer.Start();
            mediaElement.Play();
        }



        void TickTock(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    mediaElement.Play();
                    if (PBar1.Value == PBar1.Maximum)
                        rise = -1;
                    if (PBar1.Value == PBar1.Minimum)
                        rise = 1;
                    foreach (var bar in bars)
                        bar.Value += rise;
                }
                catch(TaskCanceledException ex)
                { }
            }));
        }

        private void mediaElement_MediaEnded(object sender, EventArgs e)
        {
            mediaElement.Stop();
            mediaElement.Play();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer!= null)
              timer.Interval = slider.Value;
        }
    }
}

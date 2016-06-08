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

namespace _3_3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string[] ComboBoxLabels = {"2-letnie", "3-letnie", "whatever"};

        public MainWindow()
        {
            InitializeComponent();
            foreach (var label in ComboBoxLabels)
            {
                cyklCombo.Items.Add(label);
            }
            cyklCombo.SelectedIndex = 0;
        }

        private void AkceptujB_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(nazwaTB.Text);
            sb.AppendLine();
            sb.Append(adresTB.Text);
            sb.AppendLine();
            sb.Append(cyklCombo.Text);
            sb.AppendLine();
            if (dzienneCB.IsChecked != null && (bool)dzienneCB.IsChecked
                && uzupełniająceCB.IsChecked != null && (bool)uzupełniająceCB.IsChecked)
            {
                sb.Append("Dzienne, uzupełniające");
            }
            else if (dzienneCB.IsChecked != null && (bool)dzienneCB.IsChecked)
            {
                sb.Append("Dzienne");
            }
            else if (uzupełniająceCB.IsChecked != null && (bool)uzupełniająceCB.IsChecked)
            {
                sb.Append("Uzupełniające");
            }
            MessageBox.Show(sb.ToString());
        }

        private void AnulujB_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

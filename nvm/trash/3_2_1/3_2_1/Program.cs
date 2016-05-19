using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace _3_2_1
{
    public class CMainForm : Form
    {
        GroupBox uczelniaGB;
        GroupBox rodzajGB;
        TextBox nazwaTB;
        TextBox adresTB;
        ComboBox cyklCB;
        CheckBox dzienneChB;
        CheckBox uzupChB;
        Button akceptujB;
        Button anulujB;

        public CMainForm()
        {
            this.Text = "Wybór uczelni";
            this.Size = new Size(600, 300);

            uczelniaGB = new GroupBox();
            uczelniaGB.Text = "Uczelnia";
            uczelniaGB.Location = new Point(10, 10);
            uczelniaGB.Size = new Size(560, 80);

            Label nazwaLab = new Label();
            nazwaLab.Text = "Nazwa:";
            nazwaLab.Location = new Point(10, 20);
            nazwaLab.Width = 60;

            nazwaTB = new TextBox();
            nazwaTB.Text = "Nazwa uczelni";
            nazwaTB.Location = new Point(70, 18);
            nazwaTB.Width = 480;

            Label adresLab = new Label();
            adresLab.Text = "Adres:";
            adresLab.Location = new Point(10, 50);
            adresLab.Width = 60;

            adresTB = new TextBox();
            adresTB.Text = "Adres uczelni";
            adresTB.Location = new Point(70, 48);
            adresTB.Width = 480;

            uczelniaGB.Controls.AddRange(new Control[] { nazwaLab, nazwaTB, adresLab, adresTB });

            rodzajGB = new GroupBox();
            rodzajGB.Text = "Rodzaj studiów";
            rodzajGB.Location = new Point(10, 120);
            rodzajGB.Size = new Size(560, 80);

            Label cyklLab = new Label();
            cyklLab.Text = "Cykl nauki:";
            cyklLab.Location = new Point(10, 20);
            cyklLab.Width = 60;

            cyklCB = new ComboBox();
            cyklCB.Location = new Point(70, 18);
            cyklCB.Width = 480;
            cyklCB.Items.Add("3-letnie");
            cyklCB.Items.Add("II stopnia");
            cyklCB.Items.Add("Próba Bólu");
            cyklCB.SelectedIndex = 0;


            dzienneChB = new CheckBox();
            dzienneChB.Text = "dzienne";
            dzienneChB.Location = new Point(70, 48);
            dzienneChB.Width = 80;

            uzupChB = new CheckBox();
            uzupChB.Location = new Point(150, 48);
            uzupChB.Text = "uzupełniające";

            rodzajGB.Controls.AddRange(new Control[] { cyklLab, cyklCB, dzienneChB, uzupChB });

            akceptujB = new Button();
            akceptujB.Text = "Akceptuj";
            akceptujB.Click += new EventHandler(AkceptujHandler);
            akceptujB.Location = new Point(40, 220);

            anulujB = new Button();
            anulujB.Text = "Anuluj";
            anulujB.Click += new EventHandler(AnulujHandler);
            anulujB.Location = new Point(500, 220);

            this.Controls.AddRange(new Control[] { uczelniaGB, rodzajGB, akceptujB, anulujB});
        }

        private void AkceptujHandler(object sender, EventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.Append(nazwaTB.Text);
            str.Append('\n');
            str.Append(adresTB.Text);
            str.Append('\n');
            str.Append(cyklCB.SelectedItem);

            if (dzienneChB.Checked && uzupChB.Checked)
                str.Append("\nStudia dzienne, uzupełniające");
            else
            {
                if (dzienneChB.Checked)
                    str.Append("\nStudia dzienne");
                if (uzupChB.Checked)
                    str.Append("\nStudia uzupełniające");
            }
            MessageBox.Show(str.ToString());
        }

        private void AnulujHandler(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static void Main()
        {
            Application.Run( new CMainForm() );
        }
    }
}
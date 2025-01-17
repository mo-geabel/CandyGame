using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project
{
    public partial class Form3 : Form
    {


        public Form3(List<KeyValuePair<string, int>> highScores)
        {
            InitializeComponent();

            // Clear any existing items
            lstHighScores.Items.Clear();

            // Add high scores to the ListBox
            foreach (var entry in highScores)
            {
                lstHighScores.Items.Add($"{entry.Key}: {entry.Value}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}

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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        class person
        {
            private string name; // field
            public string Name   // property
            {
                get { return name; }
                set { name = value; }
            }
        }

        //private void trackBar1_Scroll(object sender, EventArgs e)
        //{
        //    TrackBar trackBar = (TrackBar)sender;

        //    // Change the color based on the value (0 = Off, 1 = On)
        //    if (trackBar.Value == 1)
        //    {
        //        trackBar.BackColor = Color.FromArgb(45, 45, 48);
        //        label1.ForeColor = Color.White;
        //        this.BackColor = Color.FromArgb(45, 45, 48);
        //        label1.Text = "Light Mood";
        //    }
        //    else
        //    {
        //        trackBar.BackColor = Color.White;
        //        this.BackColor = Color.White;
        //        label1.ForeColor = Color.Black;
        //        label1.Text = "Dark Mood";

        //    }
        //}

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    MessageBox.Show("Please enter your name to start the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                person p = new person();
                p.Name = textBox1.Text;
                textBox1.Clear();
                Form2 form2 = new Form2(p.Name);
                form2.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Please enter your name to start the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            person p = new person();
            p.Name = textBox1.Text;
            textBox1.Clear();
            Form2 form2 = new Form2(p.Name);
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var highScores = HighScoreManager.LoadHighScores();

            // Debug: Verify loaded high scores
            foreach (var entry in highScores)
            {
                Console.WriteLine($"Loaded High Score - Name: {entry.Key}, Score: {entry.Value}");
            }

            // Pass high scores to Form3
            Form3 highScoresForm = new Form3(highScores);
            highScoresForm.Show();
            this.Hide(); // Hide the main menu
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show();
        }
    }
  
}

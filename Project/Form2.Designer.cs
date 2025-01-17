namespace Project
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Player_name = new System.Windows.Forms.Label();
            this.Timer = new System.Windows.Forms.Label();
            this.Score = new System.Windows.Forms.Label();
            this.PlayerName = new System.Windows.Forms.Label();
            this.gameGrid = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.Player_name);
            this.panel1.Controls.Add(this.Timer);
            this.panel1.Controls.Add(this.Score);
            this.panel1.Controls.Add(this.PlayerName);
            this.panel1.Location = new System.Drawing.Point(1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(802, 67);
            this.panel1.TabIndex = 0;
            // 
            // Player_name
            // 
            this.Player_name.AutoSize = true;
            this.Player_name.Location = new System.Drawing.Point(24, 30);
            this.Player_name.Name = "Player_name";
            this.Player_name.Size = new System.Drawing.Size(0, 16);
            this.Player_name.TabIndex = 3;
            // 
            // Timer
            // 
            this.Timer.AutoSize = true;
            this.Timer.Location = new System.Drawing.Point(358, 30);
            this.Timer.Name = "Timer";
            this.Timer.Size = new System.Drawing.Size(42, 16);
            this.Timer.TabIndex = 2;
            this.Timer.Text = "Timer";
            // 
            // Score
            // 
            this.Score.AutoSize = true;
            this.Score.Location = new System.Drawing.Point(700, 30);
            this.Score.Name = "Score";
            this.Score.Size = new System.Drawing.Size(56, 16);
            this.Score.TabIndex = 2;
            this.Score.Text = "Score: 0";
            // 
            // PlayerName
            // 
            this.PlayerName.AutoSize = true;
            this.PlayerName.Location = new System.Drawing.Point(24, 30);
            this.PlayerName.Name = "PlayerName";
            this.PlayerName.Size = new System.Drawing.Size(0, 16);
            this.PlayerName.TabIndex = 1;
            // 
            // gameGrid
            // 
            this.gameGrid.Location = new System.Drawing.Point(1, 64);
            this.gameGrid.Name = "gameGrid";
            this.gameGrid.Size = new System.Drawing.Size(799, 387);
            this.gameGrid.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gameGrid);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "Crash";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label PlayerName;
        private System.Windows.Forms.Label Timer;
        private System.Windows.Forms.Label Score;
        private System.Windows.Forms.Label Player_name;
        private System.Windows.Forms.Panel gameGrid;
    }
}
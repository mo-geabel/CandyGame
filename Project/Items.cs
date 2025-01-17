using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project
{
    public abstract class GameTile : PictureBox
    {
        public int Row { get; set; }
        public int Column { get; set; }

        protected GameTile(int row, int col, int width, int height)
        {
            Row = row;
            Column = col;
            Width = width;
            Height = height;
            BorderStyle = BorderStyle.FixedSingle;
        }

        public virtual void ActivateEffect()
        {
            Console.WriteLine("the item is clicked");
        }
    }

    public class ColorTile : GameTile
    {
        public Color TileColor { get; }

        public ColorTile(int row, int col, int width, int height, Color color)
            : base(row, col, width, height)
        {
            TileColor = color;
            BackColor = color;
        }

        public virtual void ActivateEffect()
        {
            Console.WriteLine($"the {this.TileColor} item is clicked");
        }
    }

    public class SpecialItemTile : GameTile
    {
        public string SpecialType { get; }
        public Image SpecialImage { get; }

        public SpecialItemTile(int row, int col, int width, int height, string specialType, Image image)
            : base(row, col, width, height)
        {
            SpecialType = specialType;
            SpecialImage = image;
            Image = image;
            SizeMode = PictureBoxSizeMode.StretchImage;
        }

        public override void ActivateEffect()
        {
            MessageBox.Show($"Activated effect: {SpecialType}!");
        }
    }
}

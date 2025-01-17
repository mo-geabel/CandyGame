using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class ColorManager
    {
        public static readonly Color[] Colors = { Color.Red, Color.Blue, Color.Green, Color.Yellow };

        public static Color GetRandomColor(Random random)
        {
            return Colors[random.Next(Colors.Length)];
        }
    }
}

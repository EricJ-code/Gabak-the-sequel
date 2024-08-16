using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GabakWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int[,] numbers = { { 10, 40, 20, 50 }, { 30, 60, 80, 90 } };
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));

            for (int i = 0; i < numbers.GetLength(0); i++)
            {
                e.Graphics.DrawLine(pen, numbers[i,0], numbers[i, 1], numbers[i, 2], numbers[i, 3]);
            }
                

            
        }
    }
}

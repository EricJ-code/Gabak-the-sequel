using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;


namespace GabakWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private struct Origin
        {
            public Origin(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            public double X { get;}
            public double Y { get;}
        }

        private struct PixelsPerMeter
        {
            public PixelsPerMeter(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }
            public double X { get; }
            public double Y { get; }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            (var userData, var warehouseData) = Program.FetchData();

            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
            SolidBrush brushRed = new SolidBrush(Color.FromArgb(255, 255, 0, 0));

            Origin origin = new Origin(panel1.Size.Width / 2, panel1.Size.Height * 0.95);
            PixelsPerMeter ppm = new PixelsPerMeter(
                panel1.Size.Width / warehouseData.WarehouseWidth,
                panel1.Size.Height / warehouseData.WarehouseDepth
            );

            for (int i = 0; i < warehouseData.RacksLocation.Count; i++)
            {
                double x = origin.X + warehouseData.RacksLocation[i].X * ppm.X;
                double y = origin.Y + warehouseData.RacksLocation[i].Y * ppm.Y;
                int offsetx = Convert.ToInt32(warehouseData.RackWidth * ppm.X);
                int offsety = Convert.ToInt32(warehouseData.RackDepth * ppm.Y);
                int boxX = Convert.ToInt32(x - (offsetx / 2));
                int boxY = Convert.ToInt32(y - (offsety / 2));
                e.Graphics.FillRectangle(brush, new Rectangle(boxX, boxY, offsetx+1, offsety+1)); // add 1 to fill gaps
            }
            e.Graphics.FillRectangle(brushRed, new Rectangle(Convert.ToInt32(origin.X), Convert.ToInt32(origin.Y), 5, 5));
        }
    }
}

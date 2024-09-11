using Newtonsoft.Json.Bson;
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
        private readonly struct XY
        {
            public XY(double x, double y)
            {
                this.X = (float)x;
                this.Y = (float)y;
            }
            public XY(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }
            public float X { get;}
            public float Y { get;}
        }

        private void drawRectangle(PaintEventArgs e, Pen pen, double width, double height, double degree, double x1, double x2, double x3, double x4, double y1, double y2, double y3, double y4)
        {
            e.Graphics.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
            e.Graphics.DrawLine(pen, (float)x2, (float)y2, (float)x3, (float)y3);
            e.Graphics.DrawLine(pen, (float)x3, (float)y3, (float)x4, (float)y4);
            e.Graphics.DrawLine(pen, (float)x4, (float)y4, (float)x1, (float)y1);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            (var userData, var warehouseData) = Program.FetchData();

            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            pen.Width = 2;

            XY origin = new XY(panel1.Size.Width / 2, (float)(panel1.Size.Height * 0.95));
            XY pixelPerMeter = new XY(
                panel1.Size.Width / warehouseData.WarehouseWidth,
                panel1.Size.Height / warehouseData.WarehouseDepth
            );

            // draw the warehouse racks
            for (int i = 0; i < warehouseData.RacksLocation.Count; i++)
            {
                // x1: bottom left, x2: top left, x3: top right, x4: bottom right 
                double x1 = warehouseData.RacksLocation[i].X2;
                double x2 = warehouseData.RacksLocation[i].X1;
                double x3 = warehouseData.RacksLocation[i].X3;
                double x4 = warehouseData.RacksLocation[i].X4;
                double y1 = warehouseData.RacksLocation[i].Y2;
                double y2 = warehouseData.RacksLocation[i].Y1;
                double y3 = warehouseData.RacksLocation[i].Y3;
                double y4 = warehouseData.RacksLocation[i].Y4; 
                double degree = warehouseData.RacksLocation[i].Angle;
                this.drawRectangle(e, pen, warehouseData.RackWidth * pixelPerMeter.X, warehouseData.RackDepth * pixelPerMeter.Y, degree, x1 * pixelPerMeter.X, x2 * pixelPerMeter.X, x3 * pixelPerMeter.X, x4 * pixelPerMeter.X, y1 * pixelPerMeter.Y, y2 * pixelPerMeter.Y, y3 * pixelPerMeter.Y, y4 * pixelPerMeter.Y);
            }
        }
    }
}
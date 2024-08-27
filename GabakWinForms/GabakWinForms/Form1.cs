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

        private void drawRectangle(PaintEventArgs e, Pen pen, XY center, double width, double height, double degree)
        {
            double radian = Math.PI / 180 * degree;
            XY topLeft = new XY(center.X - width / 2, center.Y - height / 2);
            XY topRight = new XY(topLeft.X + width, topLeft.Y);
            XY bottomRight = new XY(topLeft.X + width, topLeft.Y + height);
            XY bottomLeft = new XY(topLeft.X, topLeft.Y + height);
            e.Graphics.DrawLine(pen, topLeft.X, topLeft.Y, topRight.X, topRight.Y);
            e.Graphics.DrawLine(pen, topRight.X, topRight.Y, bottomRight.X, bottomRight.Y);
            e.Graphics.DrawLine(pen, bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
            e.Graphics.DrawLine(pen, bottomLeft.X, bottomLeft.Y, topLeft.X, topLeft.Y);
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
                double x = origin.X + warehouseData.RacksLocation[i].X * pixelPerMeter.X;
                double y = origin.Y + warehouseData.RacksLocation[i].Y * pixelPerMeter.Y;
                double degree = warehouseData.RacksLocation[i].Angle;
                this.drawRectangle(e, pen, new XY(x, y), warehouseData.RackWidth * pixelPerMeter.X, warehouseData.RackDepth * pixelPerMeter.Y, degree);
            }
        }
    }
}

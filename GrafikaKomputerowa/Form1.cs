using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GrafikaKomputerowa
{
    public partial class Form1 : Form
    {
        enum ToolType
        { Line, Rectangle, Circle, Select }
        ToolType currentTool = ToolType.Line;
        Bitmap canvas;
        Bitmap preview;
        Bitmap baseCanvas;
        Point startPoint;
        bool isDrawing = false;
        List<Shape> shapes = new List<Shape>();
        Shape selectedShape = null;
        Point dragOffset;
        enum HandleType { None, Start, End }
        HandleType activeHandle = HandleType.None;

        public Form1()
        {
            InitializeComponent();
            canvas = new Bitmap(800, 600);
            pictureBox1.Image = canvas;
        }

        /*private void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if ((x0 >= 0 && x0 < canvas.Width && y0 >= 0 && y0 < canvas.Height))
                {
                    canvas.SetPixel(x0, y0, color);
                }
                

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            pictureBox1.Invalidate(); // odświeżenie obrazu
        }*/
        //^podstawa do algorytmu rysowania linii

        public static void DrawLineOnBitmap(Bitmap bmp, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < bmp.Width && y0 >= 0 && y0 < bmp.Height)
                    bmp.SetPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        public static void DrawRectangle(Bitmap bmp, Point p1, Point p2, Color color)
        {
            DrawLineOnBitmap(bmp, p1.X, p1.Y, p2.X, p1.Y, color); // góra
            DrawLineOnBitmap(bmp, p2.X, p1.Y, p2.X, p2.Y, color); // prawa
            DrawLineOnBitmap(bmp, p2.X, p2.Y, p1.X, p2.Y, color); // dół
            DrawLineOnBitmap(bmp, p1.X, p2.Y, p1.X, p1.Y, color); // lewa
        }

        //Rysowanie po Mid-Point Ellipse Drawing Algorithm
        public static void PlotEllipsePoints(Bitmap bmp, double xc, double yc, double x, double y, Color color)
        {
            void Set(int px, int py)
            {
                if (px >= 0 && px < bmp.Width && py >= 0 && py < bmp.Height)
                    bmp.SetPixel(px, py, color);
            }

            Set((int)(xc + x), (int)(yc + y));
            Set((int)(xc - x), (int)(yc + y));
            Set((int)(xc + x), (int)(yc - y));
            Set((int)(xc - x), (int)(yc - y));
        }


        // Mid-Point Ellipse Drawing Algorithm
        public static void DrawEllipse(Bitmap bmp, double rx, double ry,
                        double xc, double yc)
        {

            double dx, dy, d1, d2, x, y;
            x = 0;
            y = ry;

            // Initial decision parameter of region 1
            d1 = (ry * ry) - (rx * rx * ry) +
                            (0.25f * rx * rx);
            dx = 2 * ry * ry * x;
            dy = 2 * rx * rx * y;

            // For region 1
            while (dx < dy)
            {
                PlotEllipsePoints(bmp, xc, yc, x, y, Color.Black);


                if (d1 < 0)
                {
                    x++;
                    dx = dx + (2 * ry * ry);
                    d1 = d1 + dx + (ry * ry);
                }
                else
                {
                    x++;
                    y--;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d1 = d1 + dx - dy + (ry * ry);
                }
            }

            // Decision parameter of region 2
            d2 = ((ry * ry) * ((x + 0.5f) * (x + 0.5f)))
                + ((rx * rx) * ((y - 1) * (y - 1)))
                - (rx * rx * ry * ry);

            // Plotting points of region 2
            while (y >= 0)
            {


                PlotEllipsePoints(bmp, xc, yc, x, y, Color.Black);

                // Checking and updating parameter
                // value based on algorithm
                if (d2 > 0)
                {
                    y--;
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + (rx * rx) - dy;
                }
                else
                {
                    y--;
                    x++;
                    dx = dx + (2 * ry * ry);
                    dy = dy - (2 * rx * rx);
                    d2 = d2 + dx - dy + (rx * rx);
                }
            }
        }

        void DrawShape(Bitmap bmp, Point start, Point end)
        {
           
            switch (currentTool)
            {
                case ToolType.Line:
                    DrawLineOnBitmap(bmp, start.X, start.Y, end.X, end.Y, Color.Black);
                 
                    break;
                case ToolType.Rectangle:
                    DrawRectangle(bmp, start, end, Color.Black);
                  
                    break;
                case ToolType.Circle:
                    DrawEllipse(bmp,
                        Math.Abs(end.X - start.X) / 2.0,
                        Math.Abs(end.Y - start.Y) / 2.0,
                        (end.X + start.X) / 2.0,
                        (end.Y + start.Y) / 2.0);
                   
                    break;
            }

            
        }



        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            if (currentTool == ToolType.Select)
            {
                activeHandle = HandleType.None;

                if (selectedShape != null)
                {
                    activeHandle = HitTestHandle(e.Location, selectedShape);
                    if (activeHandle != HandleType.None)
                        return;
                }

                var hit = shapes.LastOrDefault(s => s.HitTest(e.Location));
                if (hit != null)
                {
                    selectedShape = hit;
                    dragOffset = new Point(e.Location.X - hit.Start.X, e.Location.Y - hit.Start.Y);
                }
                else
                {
                    selectedShape = null;
                }

                RedrawAll(canvas);
            }
            else
            {
                isDrawing = true;
                startPoint = e.Location;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                shapes.Add(new Shape((Shape.ShapeType)currentTool, startPoint, e.Location));
                isDrawing = false;
                RedrawAll(canvas);
            }
            else if (currentTool == ToolType.Select && selectedShape != null)
            {
                activeHandle = HandleType.None;
                RedrawAll(canvas); // zatwierdź zmianę
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentTool == ToolType.Select && selectedShape != null && e.Button == MouseButtons.Left)
            {
                Shape tempShape = selectedShape;

                if (activeHandle == HandleType.Start)
                    tempShape.Start = e.Location;
                else if (activeHandle == HandleType.End)
                    tempShape.End = e.Location;
                else
                {
                    Point newStart = new Point(
                        e.Location.X - dragOffset.X,
                        e.Location.Y - dragOffset.Y);

                    int dx = newStart.X - selectedShape.Start.X;
                    int dy = newStart.Y - selectedShape.Start.Y;

                    tempShape.Start = newStart;
                    tempShape.End = new Point(selectedShape.End.X + dx, selectedShape.End.Y + dy);
                }

                RedrawAll(canvas);
            }

            else if (isDrawing)
            {
                if (preview != null) preview.Dispose();
                preview = (Bitmap)canvas.Clone();
                DrawShape(preview, startPoint, e.Location);
                pictureBox1.Image = preview;
            }


        }

        private void Line_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Line;
        }
        
        private void Rectangle_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Rectangle;
        }

        private void Draw_Click(object sender, EventArgs e)
        {
            try
            {

                Point StartManual, EndManual;
                int x1 = int.Parse(x1TextBox.Text);
                int y1 = int.Parse(y1TextBox.Text);
                int x2 = int.Parse(x2TextBox.Text);
                int y2 = int.Parse(y2TextBox.Text);
                if (x1 < 0 || x2 < 0 || y1 < 0 || y2 < 0 || x1 >= canvas.Width || x2 >= canvas.Width || y1 >= canvas.Height || y2 >= canvas.Height)
                {
                    MessageBox.Show("Współrzędne muszą mieścić się w obszarze rysunku.");
                    return;
                }
                StartManual = new Point(x1, y1);
                EndManual = new Point(x2, y2);
                DrawShape(canvas, StartManual, EndManual);
                pictureBox1.Image = canvas;
                pictureBox1.Invalidate();
                shapes.Add(new Shape((Shape.ShapeType)currentTool, StartManual, EndManual));


            }
            catch (FormatException)
            {
                
                MessageBox.Show($"Błędnie podane wartości");
            }
          
        }

        private void Circle_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Circle;
        }

        private void Select_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Select;
        }


        void ClearBitmap(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, Color.White);
                }
            }
        }


        void RedrawAll(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White); // lub inne tło
            }
            //usun jak nie dziala
            //canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            foreach (var shape in shapes)
                shape.Draw(bmp);

            if (selectedShape != null)
                Shape.DrawHandles(bmp, selectedShape);

            pictureBox1.Image = bmp;
        }


        HandleType HitTestHandle(Point p, Shape shape)
        {
            int size = 6;

            bool InHandle(Point center)
            {
                return p.X >= center.X - size / 2 &&
                       p.X <= center.X + size / 2 &&
                       p.Y >= center.Y - size / 2 &&
                       p.Y <= center.Y + size / 2;
            }

            if (InHandle(shape.Start)) return HandleType.Start;
            if (InHandle(shape.End)) return HandleType.End;
            return HandleType.None;
        }




    }

    public class Shape
    {
        public enum ShapeType { Line, Rectangle, Circle}

        public ShapeType Type { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
       

        // Można dodać grubość, styl, identyfikator, itp.

        public Shape(ShapeType type, Point start, Point end)
        {
            Type = type;
            Start = start;
            End = end;
        }
        public Shape Clone()
        {
            return new Shape(this.Type, this.Start, this.End);
        }

        public void Draw(Bitmap bmp)
        {
            switch (Type)
            {
                case ShapeType.Line:
                    // Użyj swojej metody
                    Form1.DrawLineOnBitmap(bmp, Start.X, Start.Y, End.X, End.Y, Color.Black);
                    break;
                case ShapeType.Rectangle:
                    Form1.DrawRectangle(bmp, Start, End, Color.Black);
                    break;
                case ShapeType.Circle:
                    Form1.DrawEllipse(bmp,
                        Math.Abs(End.X - Start.X) / 2.0,
                        Math.Abs(End.Y - Start.Y) / 2.0,
                        (End.X + Start.X) / 2.0,
                        (End.Y + Start.Y) / 2.0);
                    break;
            }
        }

        public static void DrawHandles(Bitmap bmp, Shape shape)
        {
            int size = 6;

            Point startHandleTopLeft = new Point(shape.Start.X - size / 2, shape.Start.Y - size / 2);
            Point startHandleBottomRight = new Point(shape.Start.X + size / 2, shape.Start.Y + size / 2);

            Point endHandleTopLeft = new Point(shape.End.X - size / 2, shape.End.Y - size / 2);
            Point endHandleBottomRight = new Point(shape.End.X + size / 2, shape.End.Y + size / 2);

            Form1.DrawRectangle(bmp, startHandleTopLeft, startHandleBottomRight, Color.Red);
            Form1.DrawRectangle(bmp, endHandleTopLeft, endHandleBottomRight, Color.Red);
        }


        public bool HitTest(Point p)
        {
            // Prosty test: czy kliknięto w obrębie prostokąta otaczającego obiekt
            Rectangle bounds = new Rectangle(
                Math.Min(Start.X, End.X),
                Math.Min(Start.Y, End.Y),
                Math.Abs(End.X - Start.X),
                Math.Abs(End.Y - Start.Y));

            return bounds.Contains(p);
        }
    }

}


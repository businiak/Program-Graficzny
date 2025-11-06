using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
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
        private float zoom = 1.0f;
        private const float ZoomStep = 0.1f;
        private const float MinZoom = 0.1f;
        private const float MaxZoom = 100.0f;
        private Bitmap originalImage = null;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.Normal; 
            canvas = new Bitmap(800, 600);
            pictureBox1.Image = canvas;

            
            pictureBox1.Width = canvas.Width;
            pictureBox1.Height = canvas.Height;
            panel1.AutoScroll = true;
            panel1.Dock = DockStyle.Fill;
            panel1.Controls.Add(pictureBox1);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Location = new Point(0, 0);
            panel1.MouseWheel += Panel1_MouseWheel;
            panel1.Focus();

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


        private void ApplyZoom()
        {
            if (originalImage == null) return;

            int newWidth = (int)(originalImage.Width * zoom);
            int newHeight = (int)(originalImage.Height * zoom);

            Bitmap zoomed = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(zoomed))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            pictureBox1.Image = zoomed;
            pictureBox1.Width = zoomed.Width;
            pictureBox1.Height = zoomed.Height;
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
            if (selectedShape != null)
            {
                selectedShape = null;
                RedrawAll(canvas); // odśwież widok bez uchwytów
            }

            currentTool = ToolType.Line;
        }
        
        private void Rectangle_Click(object sender, EventArgs e)
        {
            if (selectedShape != null)
            {
                selectedShape = null;
                RedrawAll(canvas); // odśwież widok bez uchwytów
            }

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

                if (selectedShape != null)
                {
                    selectedShape.Start = StartManual;
                    selectedShape.End = EndManual;
                    RedrawAll(canvas);
                    return;
                }

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
            if (selectedShape != null)
            {
                selectedShape = null;
                RedrawAll(canvas); // odśwież widok bez uchwytów
            }

            currentTool = ToolType.Circle;
        }

        private void Select_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Select;
        }

        private void ManualSelect_Click(object sender, EventArgs e)
        {
            try
            {

                int x = int.Parse(xselect.Text);
                int y = int.Parse(yselect.Text);
                Point testPoint = new Point(x, y);

                if (x < 0 || y < 0 || x >= canvas.Width || y >= canvas.Height)
                {
                    MessageBox.Show("Współrzędne muszą mieścić się w obszarze rysunku.");
                    return;
                }
                selectedShape = shapes.LastOrDefault(s => s.HitTest(testPoint));

                RedrawAll(canvas);
            }
            catch
            {
                MessageBox.Show("Wprowadź poprawne współrzędne do zaznaczenia.");
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if(shapes.Count > 0) 
            {
            SaveLoad.SaveToFile("figury.txt", shapes);
            }
            else
            {
                if (pictureBox1.Image == null)
                {
                    MessageBox.Show("Brak obrazu do zapisania.");
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Obrazy JPEG (*.jpg)|*.jpg";
                    sfd.Title = "Zapisz obraz jako JPEG";

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    // okienko do wyboru jakości (0–100)
                    long quality = 90;
                    using (Form qualityDialog = new Form())
                    {
                        qualityDialog.Text = "Wybierz jakość JPEG (0–100)";
                        qualityDialog.StartPosition = FormStartPosition.CenterParent;
                        qualityDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                        qualityDialog.ClientSize = new Size(200, 80);
                        qualityDialog.MinimizeBox = false;
                        qualityDialog.MaximizeBox = false;

                        var num = new NumericUpDown()
                        {
                            Minimum = 0,
                            Maximum = 100,
                            Value = 90,
                            Dock = DockStyle.Top
                        };

                        var ok = new System.Windows.Forms.Button()
                        {
                            Text = "OK",
                            Dock = DockStyle.Bottom
                        };

                        ok.Click += (s, _) => qualityDialog.DialogResult = DialogResult.OK;

                        qualityDialog.Controls.Add(num);
                        qualityDialog.Controls.Add(ok);

                        if (qualityDialog.ShowDialog() == DialogResult.OK)
                            quality = (long)num.Value;
                    }

                    // zapis JPEG z określoną jakością
                    var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    var encParams = new EncoderParameters(1);
                    encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                    pictureBox1.Image.Save(sfd.FileName, encoder, encParams);
                    MessageBox.Show($"Obraz zapisano jako JPEG ({quality}%).");
                }
            }
        }
        private void Panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) != Keys.Control)
                return; // tylko z Ctrl inaczej przewijanie działa normalnie

            if (originalImage == null) return;

            float oldZoom = zoom;

            if (e.Delta > 0 && zoom < MaxZoom)
                zoom += ZoomStep;
            else if (e.Delta < 0 && zoom > MinZoom)
                zoom -= ZoomStep;

            //  Zabezpieczenie przed crashami 
            if (zoom < 0.05f || zoom > 100f)
            {
                zoom = Math.Max(0.05f, Math.Min(zoom, 100f));
                return;
            }

            
            if (originalImage.Width * zoom < 1 || originalImage.Height * zoom < 1 ||  originalImage.Width * zoom > 9000 || originalImage.Height * zoom > 7000)
                return; // zbyt duży – pomiń

            if (Math.Abs(zoom - oldZoom) > 0.001f)
                ApplyZoom();
        }
        private void Load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Pliki figur (*.txt)|*.txt|Obrazy PPM (*.ppm)|*.ppm|Obrazy JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Wszystkie pliki (*.*)|*.*";
                ofd.Title = "Wybierz plik do wczytania";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string extension = Path.GetExtension(ofd.FileName).ToLower();

                try
                {
                    // FIGURY
                    if (extension == ".txt")
                    {
                        shapes = SaveLoad.LoadFromFile(ofd.FileName);
                        selectedShape = null;
                        RedrawAll(canvas);
                        MessageBox.Show("Wczytano figury z pliku: " + ofd.FileName);
                        return;
                    }

                    Bitmap src;

                    //  PPM 
                    if (extension == ".ppm")
                    {
                        string magic;
                        using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                        using (var reader = new StreamReader(fs))
                        {
                            do
                            {
                                magic = reader.ReadLine()?.Trim();
                            } while (magic != null && (magic.StartsWith("#") || string.IsNullOrWhiteSpace(magic)));
                        }

                        if (magic == "P3")
                            src = SaveLoad.LoadPPM_P3(ofd.FileName);
                        else if (magic == "P6")
                            src = SaveLoad.LoadPPM_P6(ofd.FileName);
                        else
                            throw new Exception("Nieznany format PPM – oczekiwano P3 lub P6.");
                    }
                    //JPEG
                    else if (extension == ".jpg" || extension == ".jpeg")
                    {
                        src = new Bitmap(ofd.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Nieobsługiwany typ pliku.");
                        return;
                    }

                    //Ustawienie obrazu
                    pictureBox1.Image = src;
                    pictureBox1.BackColor = Color.White;
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                    originalImage = new Bitmap(src); // zapamiętaj oryginał do zoomu
                    zoom = 1.0f;
                    if (src.Width < 50 && src.Height < 50)
                    {
                        // oblicz ile razy trzeba powiększyć, żeby był przynajmniej 300px szerokości
                        float desiredWidth = 300f;
                        float factor = desiredWidth / src.Width;

                        
                        zoom = Math.Min(factor, 100f);
                    }
                    else
                    {
                        zoom = 1.0f;
                    }

                    ApplyZoom(); // zastosuj początkowy zoom

                    MessageBox.Show($"Wczytano obraz {src.Width}×{src.Height}.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas wczytywania pliku: " + ex.Message);
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

        public override string ToString()
        {
            return $"{Type};{Start.X};{Start.Y};{End.X};{End.Y}";
        }

        public static Shape FromString(string line)
        {
            var parts = line.Split(';');
            var type = (ShapeType)Enum.Parse(typeof(ShapeType), parts[0]);
            var start = new Point(int.Parse(parts[1]), int.Parse(parts[2]));
            var end = new Point(int.Parse(parts[3]), int.Parse(parts[4]));
            return new Shape(type, start, end);
        }

    }



    public static class SaveLoad
    {
        //odczyt
        /*
         shapes = ShapeSerializer.LoadFromFile("figury.txt");
        selectedShape = null;
        RedrawAll(canvas);
        */
        //Zapis
        /*
         ShapeSerializer.SaveToFile("figury.txt", shapes);
         */
        public static void SaveToFile(string path, List<Shape> shapes)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var shape in shapes)
                    sw.WriteLine(shape.ToString());
            }
        }

        public static List<Shape> LoadFromFile(string path)
        {
            var loadedShapes = new List<Shape>();

            foreach (var line in File.ReadAllLines(path))
            {
                loadedShapes.Add(Shape.FromString(line));
            }

            return loadedShapes;
        }

        public static Bitmap LoadPPM_P3(string path)
        {
            
            string[] allLines = File.ReadAllLines(path);

            List<string> tokens = new List<string>();

            foreach (string line in allLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string clean = line.Split('#')[0]; // obcina komentarz jeśli jest
                if (string.IsNullOrWhiteSpace(clean)) continue;

                tokens.AddRange(clean.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
            }

            if (tokens.Count < 4)
                throw new Exception("Niepoprawny nagłówek PPM — brak wymaganych informacji.");

            // double walidacja, usun
            if (tokens[0] != "P3")
                throw new Exception("To nie jest poprawny plik PPM P3.");

            if (!int.TryParse(tokens[1], out int width) ||
                !int.TryParse(tokens[2], out int height) ||
                !int.TryParse(tokens[3], out int maxColor))
            {
                throw new Exception("Niepoprawny format nagłówka (width/height/maxColor).");
            }

            if (maxColor <= 0)
                throw new Exception("Niepoprawna wartość maxColor.");

            // Dane RGB zaczynają się po 4. tokenie
            int dataStart = 4;
            int expected = width * height * 3;
            if (tokens.Count - dataStart < expected)
                throw new Exception($"Za mało danych RGB (oczekiwano {expected}, znaleziono {tokens.Count - dataStart}).");

            // Utwórz bitmapę i przygotuj bufor
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int stride = bmpData.Stride;
            byte[] pixels = new byte[stride * height];

            int idx = dataStart;
            for (int y = 0; y < height; y++)
            {
                int rowStart = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int rVal = int.Parse(tokens[idx++]);
                    int gVal = int.Parse(tokens[idx++]);
                    int bVal = int.Parse(tokens[idx++]);
                    //SKALOWANIE LINIOWE KONWERSJA RGB WZOR R = (rVal * 255) /maxColor)
                    int r = (int)Math.Round(rVal * 255.0 / maxColor);
                    int g = (int)Math.Round(gVal * 255.0 / maxColor);
                    int b = (int)Math.Round(bVal * 255.0 / maxColor);

                    int pos = rowStart + x * 3;
                    pixels[pos + 2] = (byte)Math.Min(Math.Max(0, r), 255);
                    pixels[pos + 1] = (byte)Math.Min(Math.Max(0, g), 255);
                    pixels[pos + 0] = (byte)Math.Min(Math.Max(0, b), 255);
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }




        public static Bitmap LoadPPM_P6(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                // Czytanie nagłówka P6
                string magic = ReadToken(br);
                if (magic != "P6")
                    throw new Exception("Nieobsługiwany format pliku PPM (oczekiwano P6).");

                int width = int.Parse(ReadToken(br));
                int height = int.Parse(ReadToken(br));
                int maxColor = int.Parse(ReadToken(br));

                // Po nagłówku musi być jeden biały znak 
                // szukamy pierwszy bajt danych (czyli niebiałą wartość)
                int next;
                do
                {
                    next = br.ReadByte();
                } while (char.IsWhiteSpace((char)next));

                // Cofamy o 1 bajt, bo już zaczęliśmy dane RGB
                br.BaseStream.Seek(-1, SeekOrigin.Current);

                // Liczba bajtów na próbkę (dla >255 to 2 bajty na kanał)
                int bytesPerSample = (maxColor > 255) ? 2 : 1;

                int totalBytes = width * height * 3 * bytesPerSample;
                byte[] buffer = br.ReadBytes(totalBytes);
                if (buffer.Length < totalBytes)
                    throw new Exception("Niepełne dane RGB w pliku.");

                // Przygotowanie bitmapy i zapis pikseli szybciej niż SetPixel
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                int stride = bmpData.Stride;
                byte[] pixels = new byte[stride * height];

                int idx = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int r, g, b;
                        if (bytesPerSample == 1)
                        {
                            if (idx + 2 >= buffer.Length) break;
                            r = buffer[idx++];
                            g = buffer[idx++];
                            b = buffer[idx++];
                        }
                        else
                        {
                            if (idx + 5 >= buffer.Length) break;
                            r = (buffer[idx++] << 8) | buffer[idx++];
                            g = (buffer[idx++] << 8) | buffer[idx++];
                            b = (buffer[idx++] << 8) | buffer[idx++];
                        }

                        int pos = y * stride + x * 3;
                        pixels[pos + 2] = (byte)ScaleColor(r, maxColor); // R
                        pixels[pos + 1] = (byte)ScaleColor(g, maxColor); // G
                        pixels[pos + 0] = (byte)ScaleColor(b, maxColor); // B
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
                bmp.UnlockBits(bmpData);

                return bmp;
            }
        }


        // Pomocnicze metody:

        


        private static string ReadToken(BinaryReader br)
        {
            List<byte> bytes = new List<byte>();
            byte b;

            // Pomijamy białe znaki i komentarze
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                b = br.ReadByte();
                if (b == '#')
                {
                    // Pomijamy komentarz do końca linii
                    while (br.BaseStream.Position < br.BaseStream.Length && br.ReadByte() != '\n') ;
                    continue;
                }
                if (!char.IsWhiteSpace((char)b))
                {
                    bytes.Add(b);
                    break;
                }
            }

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                b = br.ReadByte();
                if (char.IsWhiteSpace((char)b))
                    break;
                bytes.Add(b);
            }

            if (bytes.Count == 0)
                return null;

            return Encoding.ASCII.GetString(bytes.ToArray());
        }

       

        private static int ScaleColor(int value, int maxColor)
        {
            return (int)Math.Round(value * 255.0 / maxColor);
        }








    }



}


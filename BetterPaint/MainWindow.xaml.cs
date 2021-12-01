using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterPaint
{
    public partial class MainWindow : Window
    {
        private Polygon polygon = null;
        private PointCollection points = null;
        private PointCollection tempPoints = new PointCollection();
        private List<List<Point>> finalPoints;
        private List<List<Point>> saveListPoints;
        private short polygonCount = 4;
        private Point pt = new Point();
        private Point temppt = new Point();
        private Point startPt = new Point();
        private object obj = null;
        private int x = 0, y = 0, kat=0;
        private double Rotatex=0, Rotatey=0;

        public MainWindow()
        {
            InitializeComponent();
            finalPoints = new List<List<Point>>();
        }

        private void PolygonCountButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!short.TryParse(PolygonCountTB.Text, out var value))
            {
                MessageBox.Show("Wprowadzono niepoprawne dane");
                return;
            }

            polygonCount = value;
        }

        private void AddPointButton_OnClick(object sender, RoutedEventArgs e)
        {
            double xx = 0, yy = 0;
            if(double.TryParse(ValueX.Text, out xx) && double.TryParse(ValueY.Text, out yy))
            {
                AddPoint(xx, yy);
            }
        }

        private void ChangePositionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(VectorX.Text, out x) && int.TryParse(VectorY.Text, out y))
            {
                if (polygon == null) MessageBox.Show("Wybierz obiekt do przesunięcia");
                else
                {
                    points = polygon.Points;
                    tempPoints = new PointCollection();
                    for (int i = 0; i < points.Count; i++)
                    {
                        temppt.X = points[i].X + x;
                        temppt.Y = points[i].Y + y;
                        tempPoints.Add(temppt);
                    }
                    polygon.Points = tempPoints;
                }
                points = null;
            }
            else MessageBox.Show("Wprowadź liczby całkowite");
        }

        private void RotateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(XPosition.Text, out Rotatex) && double.TryParse(YPosition.Text, out Rotatey)&& int.TryParse(Degrees.Text, out kat))
            {
                if (polygon == null) MessageBox.Show("Wybierz obiekt do przesunięcia");
                else
                {
                    points = polygon.Points;
                    tempPoints = new PointCollection();
                    double Angle = (kat * Math.PI) / 180;
                    for (int i = 0; i < points.Count; i++)
                    {
                        temppt.X = Rotatex + (points[i].X - Rotatex) * Math.Cos(Angle) - (points[i].Y - Rotatey) * Math.Sin(Angle);
                        temppt.Y = Rotatey + (points[i].X - Rotatex) * Math.Sin(Angle) + (points[i].Y - Rotatey) * Math.Cos(Angle);
                        tempPoints.Add(temppt);
                    }
                    polygon.Points = tempPoints;
                }
                points = null;
            }
            else MessageBox.Show("Wprowadź liczby");
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (e.OriginalSource is Canvas&& TurnRadio.IsChecked==false)
                {
                    AddPoint((double)Mouse.GetPosition(PolygonsCanvas).X, (double)Mouse.GetPosition(PolygonsCanvas).Y);
                }
                else if(e.OriginalSource is Canvas && TurnRadio.IsChecked == true)
                {
                    pt = e.GetPosition(PolygonsCanvas);
                    Rotatex = pt.X;
                    Rotatey = pt.Y;
                }
            }
        }

        private void AddPoint(double x, double y)
        {
            polygon = new Polygon();
            if (points == null || points.Count == 0)
            {
                points = new PointCollection();
                polygon = new Polygon();
            }

            polygon.Stroke = Brushes.Black;
            polygon.Fill = Brushes.DarkGreen;
            polygon.StrokeThickness = 1;
            polygon.Cursor = Cursors.Arrow;

            points.Add(new Point(x, y));

            var rect = new Rectangle
            {
                RadiusX = x,
                RadiusY = y,
                Height = 5,
                Width = 5,
                Fill = Brushes.DarkGreen
            };

            PolygonsCanvas.Children.Add(rect);
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);

            if (polygonCount == points.Count)
            {

                for (int i = 0; i < PolygonsCanvas.Children.Count; i++)
                {
                    if (PolygonsCanvas.Children[i] is Rectangle)
                    {
                        PolygonsCanvas.Children.RemoveAt(i);
                        i--;
                    }
                }

                finalPoints.Add(new List<Point>());
                finalPoints[finalPoints.Count - 1].AddRange(points);

                polygon.Points = points;
                polygon.MouseDown += Polygon_MouseLeftButtonDown;
                polygon.MouseMove += Polygon_MouseMove;
                polygon.MouseUp += Polygon_MouseLeftButtonUp;
                PolygonsCanvas.Children.Add(polygon);
                points = null;
            }
            polygon = null;
        }

        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pt = e.GetPosition(PolygonsCanvas);
            obj = sender;
            polygon = (Polygon)sender;

            startPt = e.GetPosition(PolygonsCanvas);
            Mouse.Capture((IInputElement)sender);
        }       
        
        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(PolygonsCanvas);
                double diffx = pos.X - pt.X;
                double diffy = pos.Y - pt.Y;
                if (SelectRadio.IsChecked==true)
                {
                    points = polygon.Points;
                    tempPoints = new PointCollection();
                    for(int i=0; i<points.Count;i++)
                    {
                        temppt.X = points[i].X + diffx;
                        temppt.Y = points[i].Y + diffy;
                        tempPoints.Add(temppt);
                    }
                    polygon.Points = tempPoints;
                    pt = pos;
                    points = null;
                }
                else if(TurnRadio.IsChecked==true)
                {
                    double Angle;
                    //Angle = Math.Abs(Math.Acos((((pt.X - Rotatex) * (pos.X - Rotatex)) + ((pt.Y - Rotatey) * (pos.Y - Rotatey))) / (Math.Sqrt(Math.Pow((pt.X - Rotatex), 2) + Math.Pow((pt.Y - Rotatey), 2)) * (Math.Sqrt(Math.Pow((pos.X - Rotatex), 2) + Math.Pow((pos.Y - Rotatey), 2))))));
                    double P12 = Math.Sqrt(Math.Pow(pt.X - Rotatex, 2) + Math.Pow(pt.Y - Rotatey, 2));
                    double P13 = Math.Sqrt(Math.Pow(pos.X - Rotatex, 2) + Math.Pow(pos.Y - Rotatey, 2));
                    double P23 = Math.Sqrt(Math.Pow(pt.X - pos.X, 2) + Math.Pow(pt.Y - pos.Y, 2));
                    Angle = Math.Acos((Math.Pow(P12,2)+Math.Pow(P13,2)-Math.Pow(P23,2))/(2*P12*P13));

                    if (pos.Y < pt.Y && pt.Y < Rotatey && pos.Y < Rotatey && pt.X > Rotatex && pos.X > Rotatex) Angle = -Angle;
                    if (pos.Y < pt.Y && pt.Y > Rotatey && pos.Y > Rotatey  && pos.X > Rotatex) Angle = -Angle;
                    if (pos.Y > pt.Y && pt.Y > Rotatey && pos.Y > Rotatey && pt.X < Rotatex && pos.X < Rotatex) Angle = -Angle;
                    if (pos.Y > pt.Y && pt.Y < Rotatey && pos.Y < Rotatey && pt.X < Rotatex && pos.X < Rotatex) Angle = -Angle;

                    points = polygon.Points;
                    tempPoints = new PointCollection();
                    for (int i = 0; i < points.Count; i++)
                    {
                        temppt.X = Rotatex + (points[i].X-Rotatex)*Math.Cos(Angle) - (points[i].Y-Rotatey)*Math.Sin(Angle);
                        temppt.Y = Rotatey + (points[i].X-Rotatex)*Math.Sin(Angle) + (points[i].Y-Rotatey)*Math.Cos(Angle);
                        tempPoints.Add(temppt);
                    }
                    polygon.Points = tempPoints;
                    pt = pos;
                    points = null;
                }
            }
        }

        private void Polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void ScelaEvent(object sender, MouseWheelEventArgs e)
        {
            if (polygon == null) MessageBox.Show("Wybierz obiekt");
            else
            {
                if (e.Delta > 0) ScalePolygon(1.1);
                else if (e.Delta < 0) ScalePolygon(0.9);
            }
        }

        private void ScalePolygon(double k)
        {
            points = polygon.Points;
            tempPoints = new PointCollection();
            for (int i = 0; i < points.Count; i++)
            {
                temppt.X = points[i].X * k + (1 - k) * Rotatex;
                temppt.Y = points[i].Y * k + (1 - k) * Rotatey;
                tempPoints.Add(temppt);
            }
            polygon.Points = tempPoints;
            points = null;
        }

        private void ScaleButton_OnClick(object sender, RoutedEventArgs e)
        {
            double k = 0;
            if (double.TryParse(ScaleX.Text, out Rotatex) && double.TryParse(ScaleY.Text, out Rotatey) && double.TryParse(Scale.Text, out k))
            {
                if (polygon == null) MessageBox.Show("Wybierz obiekt do przeskalowania");
                else
                {
                    points = polygon.Points;
                    tempPoints = new PointCollection();
                    for (int i = 0; i < points.Count; i++)
                    {
                        temppt.X = points[i].X * k + (1 - k) * Rotatex;
                        temppt.Y = points[i].Y * k + (1 - k) * Rotatey;
                        tempPoints.Add(temppt);
                    }
                    polygon.Points = tempPoints;
                }
                points = null;
            }
            else MessageBox.Show("Wprowadź liczby");
        }


        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
            var openDialong = new OpenFileDialog();

            openDialong.Filter = "Plik tekstowy (*.txt)|*.txt";

            if (openDialong.ShowDialog() == true)
            {
                var path = openDialong.FileName;

                var text = File.ReadAllText(path);

                var polygons = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var polygon in polygons)
                {
                    var polygonPoints = polygon.Split(' ');
                    polygonCount = (short)(polygonPoints.Length / 2);

                    for (int i = 0; i < polygonPoints.Length - 1; i += 2)
                    {
                        AddPoint(double.Parse(polygonPoints[i]), double.Parse(polygonPoints[i + 1]));
                    }
                }
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Plik tekstowy (*.txt)|*.txt";

            if (saveDialog.ShowDialog() == true)
            {
                File.Delete(saveDialog.FileName);

                var text = string.Empty;

                saveListPoints = new List<List<Point>>();
                List<Point> tempList = new List<Point>();

                foreach(Polygon ob in PolygonsCanvas.Children)
                {
                    tempList = new List<Point>();
                    foreach (var p in ob.Points)
                    {
                        tempList.Add(p);
                    }
                    saveListPoints.Add(tempList);
                }

                foreach (var points in saveListPoints)
                {
                    foreach (var point in points)
                    {
                        text += point.X + " " + point.Y + " ";
                    }

                    text += Environment.NewLine;
                }

                File.WriteAllText(saveDialog.FileName, text);
            }
        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            polygon = null;
            points = null;
            PolygonsCanvas.Children.Clear();
            finalPoints.Clear();
        }
    }
}

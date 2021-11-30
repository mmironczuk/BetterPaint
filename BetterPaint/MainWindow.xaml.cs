using System;
using System.Collections.Generic;
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
        private short polygonCount = 4;
        private Point pt = new Point();
        private Point temppt = new Point();
        private Point startPt = new Point();
        private object obj = null;
        private int x = 0, y = 0;

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

        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (e.OriginalSource is Canvas)
                {
                    AddPoint((int)Mouse.GetPosition(PolygonsCanvas).X, (int)Mouse.GetPosition(PolygonsCanvas).Y);
                }
            }
        }

        private void AddPoint(int x, int y)
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
                if(SelectRadio.IsChecked==true)
                {
                    var pos = e.GetPosition(PolygonsCanvas);
                    double diffx = pos.X - pt.X;
                    double diffy = pos.Y - pt.Y;
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
                }
                points = null;
            }
        }

        private void Polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void ChangePosition(object sender, MouseEventArgs e)
        {

        }

        private void MousceButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void EndEditionButton_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

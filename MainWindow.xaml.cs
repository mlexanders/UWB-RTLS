using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using UWB.Models;
using UWB.Services;

namespace UWB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread thread;
        private UdpService<JsonParseLinks> service;
        private Positioning positioning;
        public MainWindow()
        {
            InitializeComponent();

            positioning = new();
            service = new(50000);
            thread = new(() =>
            {
                var t = service.Start();
                
                while (true)
                {
                    Update();
                }
            });
            thread.Start();
        }

        public void MovePoint(double x, double y)
        {
            Canvas.SetLeft(ellipse, x * 100);
            Canvas.SetTop(ellipse, y * 170);
        }

        private void Update()
        {
            var links = service.GetUpdates();
            for (int i = 0; i < links.Length; i++)
            {
                var p = Positioning.Calculate(links[0]);

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MovePoint(p.Item1, p.Item2);
                    tX.Text = string.Format("{0:0.##}", p.Item1);
                    tY.Text = string.Format("{0:0.##}", p.Item2);
                    tZ.Text = string.Format("{0:0.##}", p.Item3);

                    r1.Text = string.Format("{0:0.##}", p.Item4.x);
                    r2.Text = string.Format("{0:0.##}", p.Item4.y);
                    r3.Text = string.Format("{0:0.##}", p.Item4.z);
                }));
            }
        }
    }

    public class Positioning
    {
      
        private static double A12 = 2.7; 
        private static double A13 = 1.8;
        private static Points points = new();

        public static (double, double, double, Points) Calculate(JsonParseLinks p)
        {
            try
            {
                lock (points)
                {
                    foreach (var link in p.Links)
                    {
                        if (link.A.Equals("1")) points.x = link.R;
                        if (link.A.Equals("2")) points.y = link.R;
                        if (link.A.Equals("3")) points.z = link.R * 1.16;
                    }
                }
                

                var x = (points.x * points.x - (points.y * points.y) + A12 * A12) / A12;
                var y = (points.x * points.x - points.z * points.z + A13 * A13) / A13;
                var z = Math.Sqrt(Math.Abs(points.x * points.x - x * x - y * y));
                return (x, y, z, points);
            }
            catch (Exception)
            {
                return (0, 0, 0, points);
            }
        }
    }

    public class Points
    {
        public double x;
        public double y;
        public double z;
    }
}
                //var r = Math.Sqrt((1000) + (1000));
                //var r2 = Math.Sqrt((1000) + (1000));
                //var x = (p.Links[0].R * p.Links[0].R - p.Links[1].R * p.Links[1].R + r * r) / (2 * r);
                //var y = (p.Links[1].R * p.Links[1].R - p.Links[2].R * p.Links[2].R + r2 * r2) / (2 * r2);
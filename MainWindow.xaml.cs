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
        private UdpService<TagData> service;
        private double x, y = 0;

        public MainWindow()
        {
            InitializeComponent();

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
            Canvas.SetLeft(tag, x * 160);
            Canvas.SetTop(tag, y * 160);
        }

        private void Update()
        {
            var links = service.GetUpdates();
            for (int i = 0; i < links.Length; i++)
            {
                var p = Positioning.Calculate(links[i]);
                if (Math.Abs(x - p.Item4.x) <= 0.051 | Math.Abs(y - p.Item4.y) <= 0.051)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tX.Text = string.Format("{0:0.##}", p.Item1);
                        tY.Text = string.Format("{0:0.##}", p.Item2);
                        tZ.Text = string.Format("{0:0.##}", p.Item3);

                        r1.Text = string.Format("{0:0.##}", p.Item4.x);
                        r2.Text = string.Format("{0:0.##}", p.Item4.y);
                        r3.Text = string.Format("{0:0.##}", p.Item4.z);
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MovePoint(p.Item1, p.Item2);
                        try
                        {
                            tX.Text = string.Format("{0:0.##}", p.Item1);
                            tY.Text = string.Format("{0:0.##}", p.Item2);
                            tZ.Text = string.Format("{0:0.##}", p.Item3);

                            r1.Text = string.Format("{0:0.##}", p.Item4.x);
                            r2.Text = string.Format("{0:0.##}", p.Item4.y);
                            r3.Text = string.Format("{0:0.##}", p.Item4.z);

                            ellipseA1.RadiusX = p.Item4.x / 2.7 * 920;
                            ellipseA1.RadiusY = p.Item4.x / 2.7 * 920;

                            ellipseA2.RadiusX = p.Item4.y / 2.7 * 920;
                            ellipseA2.RadiusY = p.Item4.y / 2.7 * 920;

                            ellipseA3.RadiusX = p.Item4.z / 2.7 * 920;
                            ellipseA3.RadiusY = p.Item4.z / 2.7 * 920;
                        }
                        catch { /* ignore */}
                    }));
                }

                x = p.Item4.x;
                y = p.Item4.y;
            }
        }
    }
}

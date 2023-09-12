using System;
using UWB.Models;

namespace UWB.Services
{
    public class Positioning
    {

        public static readonly double A12 = 2.83;
        public static readonly double A13 = 1.73;
        private static readonly Points points = new();

        public static (double, double, double, Points) Calculate(TagData p)
        {
            try
            {
                lock (points)
                {
                    foreach (var link in p.Links)
                    {
                        if (link.A.Equals("1")) points.x = link.R;
                        if (link.A.Equals("2")) points.y = link.R;
                        if (link.A.Equals("3")) points.z = link.R;
                    }
                }

                var x = (points.x * points.x - points.y * points.y + A12 * A12) / A12;
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
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Galaga
{
    class QuadraticBezier
    {

        public static List<Point> CalculatePoints(Point begin, Point middle, Point end, int pointsNumber)
        {

            List<Point> list = new List<Point>();

            float nbr = 1.0f / (float)pointsNumber;
            for (float i = 0; i < 1; i += nbr)
            {
                float xa = getPt(begin.X, middle.X, i);
                float ya = getPt(begin.Y, middle.Y, i);
                float xb = getPt(middle.X, end.X, i);
                float yb = getPt(middle.Y, end.Y, i);
                
                float x = getPt(xa, xb, i);
                float y = getPt(ya, yb, i);

                list.Add(new Point((int)x, (int)y));
            }

            return list;
        }

        private static float getPt(float n1, float n2, float perc)
        {
            float diff = n2 - n1;

            return n1 + (diff * perc);
        }


    }
}

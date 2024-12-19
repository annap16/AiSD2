using System;
using System.Collections.Generic;

namespace ASD
{
    public class WaterCalculator : MarshalByRefObject
    {

        /*
         * Metoda sprawdza, czy przechodząc p1->p2->p3 skręcamy w lewo 
         * (jeżeli idziemy prosto, zwracany jest fałsz).
         */
        private bool leftTurn(Point p1, Point p2, Point p3)
        {
            Point w1 = new Point(p2.x - p1.x, p2.y - p1.y);
            Point w2 = new Point(p3.x - p2.x, p3.y - p2.y);
            double vectProduct = w1.x * w2.y - w2.x * w1.y;
            return vectProduct > 0;
        }


        /*
         * Metoda wyznacza punkt na odcinku p1-p2 o zadanej współrzędnej y.
         * Jeżeli taki punkt nie istnieje (bo cały odcinek jest wyżej lub niżej), zgłaszany jest wyjątek ArgumentException.
         */
        private Point getPointAtY(Point p1, Point p2, double y)
        {
            if (p1.y != p2.y)
            {
                double newX = p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y);
                if ((newX - p1.x) * (newX - p2.x) > 0)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point(p1.x + (p2.x - p1.x) * (y - p1.y) / (p2.y - p1.y), y);
            }
            else
            {
                if (p1.y != y)
                    throw new ArgumentException("Odcinek p1-p2 nie zawiera punktu o zadanej współrzędnej y!");
                return new Point((p1.x + p2.x) / 2, y);
            }
        }


        /// <summary>
        /// Funkcja zwraca tablice t taką, że t[i] jest głębokością, na jakiej znajduje się punkt points[i].
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double[] PointDepths(Point[] points)
        {
            List<Point> list = new List<Point>();
            double prevX = points[0].x;
            int startInd = 0;
            double[] result = new double[points.Length];
            list.Add(points[0]);
            for(int i=1; i<points.Length; i++)
            {
                if (points[i].x >= prevX)
                {
                    list.Add(points[i]);
                  
                }
                else
                {
                    double[]pom =  DepthPom(list.ToArray());
                    if (pom != null)
                    {
                        for (int j = startInd; j < startInd + pom.Length; j++)
                        {
                            result[j] = pom[j - startInd];
                        }
                        
                    }
                    startInd = i;
                    list.Clear();
                    list.Add(points[i]);
                }
                prevX = points[i].x;
            }
            double[] pom2 = DepthPom(list.ToArray());
            if (pom2 != null)
            {
                for (int j = startInd; j < startInd + pom2.Length; j++)
                {
                    result[j] = pom2[j - startInd];
                }
            }
            return result ;
        }
        private double[] DepthPom(Point[]points)
        {
            if (points.Length == 0) return null;
            double[] result = new double[points.Length];
            result[0] = 0;
            Point[] leftMax = new Point[points.Length];
            Point[] rightMax = new Point[points.Length];
            leftMax[0] = points[0];
            for (int i = 1; i < points.Length; i++)
            {
                if (leftMax[i - 1].y < points[i].y)
                {
                    leftMax[i] = points[i];
                }
                else
                {
                    leftMax[i] = leftMax[i - 1];
                }
            }
            rightMax[points.Length - 1] = points[points.Length - 1];
            for (int i = points.Length - 2; i >= 0; i--)
            {
                if (rightMax[i + 1].y < points[i].y)
                {
                    rightMax[i] = points[i];
                }
                else
                {
                    rightMax[i] = rightMax[i + 1];
                }
            }
            result[0] = 0;
            result[result.Length - 1] = 0;
            for (int i = 1; i < points.Length - 1; i++)
            {
                Point minNeighbor;
                if (leftMax[i].y > rightMax[i].y)
                {
                    minNeighbor = rightMax[i];
                }
                else
                {
                    minNeighbor = leftMax[i];
                }
                double depth = minNeighbor.y - points[i].y;
                if (depth < 0)
                {
                    result[i] = 0;
                }
                else
                {
                    result[i] = depth;
                }
            }
            return result;
        }

        /// <summary>
        /// Funkcja zwraca objętość wody, jaka zatrzyma się w górach.
        /// 
        /// Przyjmujemy, że pierwszy punkt z tablicy points jest lewym krańcem, a ostatni - prawym krańcem łańcucha górskiego.
        /// </summary>
        public double WaterVolume(Point[] points)
        {
            double[] depth = PointDepths(points);
            double water = 0;
            for (int i = 0; i < depth.Length - 1; i++)
            {
                if (depth[i] == 0 && depth[i+1]!=0)
                {
                    Point midPoint = getPointAtY(points[i], points[i + 1], points[i + 1].y + depth[i + 1]);
                    double pom = (points[i + 1].x - midPoint.x) * depth[i + 1] / 2;
                    water += pom;
                }
                else if (depth[i + 1] == 0)
                {
                    Point midPoint = getPointAtY(points[i], points[i + 1], points[i].y + depth[i]);
                    double pom = Math.Abs(points[i].x - midPoint.x) * depth[i] / 2;
                    water += pom;
                }
                else
                {
                    double pom = (points[i + 1].x - points[i].x) * (depth[i] + depth[i + 1]) / 2;
                    water += pom;
                }
            }
            return water;
        }
    }

    [Serializable]
    public struct Point
    {
        public double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

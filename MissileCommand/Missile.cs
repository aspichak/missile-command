using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MissileCommand
{
    public class Missile
    {
        Canvas screen;
        private static int missileNum = 0;
        private string name;
        private Line gfx;
        private bool flying = true;
        private int trailSize = 30;
        private List<Point> flightPath { get; } = new List<Point>();

        public Point Start { get; private set; }
        public Point Target { get; private set; }
        public int Tick { get; private set; } = 0;

        private void buildFlightPath()
        {
            // implements Bresenham's line algorithm for our missile to follow
            // pseudo code found on wikipedia after some research into line algorithms
            // supposedly this one is the "best" so whatever~ am implementing it in C#

            // subroutines
            void plotLineLow(int x0, int y0, int x1, int y1)
            {
                int dx = x1 - x0;
                int dy = y1 - y0;
                int yi = 1;
                if (dy < 0)
                {
                    yi = -1;
                    dy = -dy;
                }
                int D = (2 * dy) - dx;
                int y = y0;

                for (int x = x0; x <= x1; x++)
                {
                    //plot(x, y);
                    flightPath.Add(new Point(x, y));
                    if (D > 0)
                    {
                        y = y + yi;
                        D = D + (2 * (dy - dx));
                    }
                    else
                        D = D + 2 * dy;
                }
            } // end plotLineLow
            void plotLineHigh(int x0, int y0, int x1, int y1)
            {
                int dx = x1 - x0;
                int dy = y1 - y0;
                int xi = 1;
                if (dx < 0)
                {
                    xi = -1;
                    dx = -dx;
                }
                int D = (2 * dx) - dy;
                int x = x0;

                for (int y = y0; y <= y1; y++)
                {
                    //plot(x, y);
                    flightPath.Add(new Point(x, y));
                    if (D > 0)
                    {
                        x = x + xi;
                        D = D + (2 * (dx - dy));
                    }
                    else
                        D = D + 2 * dx;
                }
            } // end plotLineHigh

            // main function here
            if (Math.Abs(Target.Y - Start.Y) < Math.Abs(Target.X - Start.X))
            {
                if (Start.X > Target.X)
                    plotLineLow(Target.X, Target.Y, Start.X, Start.Y);
                else
                    plotLineLow(Start.X, Start.Y, Target.X, Target.Y);
            }
            else
            {
                if (Start.Y > Target.Y)
                    plotLineHigh(Target.X, Target.Y, Start.X, Start.Y);
                else
                    plotLineHigh(Start.X, Start.Y, Target.X, Target.Y);
            }
        } // end buildLine

        public void Fly()
        {
            if (!flying)
                return;

            if (gfx != null)
                screen.Children.Remove(gfx);
            gfx = new Line();
            if (Tick < trailSize)
            {
                gfx.X1 = Start.X;
                gfx.Y1 = Start.Y;
            } else
            {
                gfx.X1 = flightPath[Tick - trailSize].X;
                gfx.Y1 = flightPath[Tick - trailSize].Y;
            }
            gfx.X2 = flightPath[Tick].X;
            gfx.Y2 = flightPath[Tick].Y;
            gfx.StrokeThickness = 1;
            gfx.Stroke = System.Windows.Media.Brushes.Red;
            gfx.Name = name;
            screen.Children.Add(gfx);

            Tick++;
            if (Tick == flightPath.Count)
                flying = false;
        }

        public Missile(Canvas cnv, Point start, Point target)
        {
            Start = start;
            Target = target;
            screen = cnv;
            buildFlightPath();
            missileNum++;
            name = $"missile{missileNum}";
        }

        public Missile(Canvas cnv, int sX, int sY, int tX, int tY) : this(cnv, new Point(sX, sY), new Point(tX, tY)) { }
    }
}

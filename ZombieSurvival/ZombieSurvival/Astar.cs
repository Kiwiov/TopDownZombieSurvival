using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvival
{
    public class Astar
    {
        public class Node
        {
            public readonly int X;
            public readonly int Y;

            public int G { get; set; }
            public int H { get; set; }
            public int F { get; set; }
            public int Sx { get; set; }
            public int Sy { get; set; }

            public MapType Type { get; set; }
            public bool IsVisited { get; set; }

            public Node(int x, int y, int sx, int sy, int g, int h)
            {
                X = x;
                Y = y;
                Sx = sx;
                Sy = sy;
                G = g;
                H = h;
                F = g + h;
            }

            public void Reset()
            {
                Type = (Type == MapType.Wall) ? MapType.Wall : MapType.Nothing;
                Sx = 0;
                Sy = 0;
                G = 0;
                H = 0;
                F = 0;
                IsVisited = false;
            }

        }

        public enum MapType
        {
            Nothing = 0,
            Wall = 1,
            Route = 2,
            Start = 3,
            Stop = 4
        };
    }
}

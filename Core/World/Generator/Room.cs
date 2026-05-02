using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.World.Generator
{
    public class Room
    {
        public int X { get; }
        public int Y { get; }
        public int W { get; }
        public int H { get; }
        public List<(int x, int y)> Entrances { get; } = new();

        public int CenterX => X + W / 2;
        public int CenterY => Y + H / 2;

        public Room(int x, int y, int w, int h)
        {
            X = x; Y = y; W = w; H = h;
        }

        public bool Overlaps(Room other) =>
            X - 2 < other.X + other.W && X + W + 2 > other.X &&
            Y - 2 < other.Y + other.H && Y + H + 2 > other.Y;
    }
}

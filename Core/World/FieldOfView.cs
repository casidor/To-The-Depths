using GameCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.World
{
    public class FieldOfView
    {
        private readonly ExplorationState[,] _state;
        public int Width { get; }
        public int Height { get; }
        public const int Radius = 10;

        public FieldOfView(int width, int height)
        {
            Width = width;
            Height = height;
            _state = new ExplorationState[width, height];
        }

        public ExplorationState this[int x, int y] => _state[x, y];

        public void Update(int px, int py, GameField field)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (_state[x, y] == ExplorationState.Visible)
                        _state[x, y] = ExplorationState.Explored;

            _state[px, py] = ExplorationState.Visible;

            for (int oct = 0; oct < 8; oct++)
                ScanOctant(px, py, field, oct, 1, 1.0, 0.0);
        }

        private void ScanOctant(
            int px, int py,
            GameField field,
            int octant,
            int row,
            double start,
            double end)
        {
            if (start < end) return;

            double newStart = 0.0;
            bool blocked = false;

            for (int dist = row; dist <= Radius; dist++)
            {
                bool hitFloorAfterWall = false;

                int dy = -dist;

                for (int dx = -dist; dx <= 0; dx++)
                {
                    int wx = px + TransformX(dx, dy, octant);
                    int wy = py + TransformY(dx, dy, octant);

                    if (wx < 0 || wx >= Width || wy < 0 || wy >= Height)
                        continue;

                    double leftSlope = (dx - 0.5) / (dy + 0.5);
                    double rightSlope = (dx + 0.5) / (dy - 0.5);

                    if (start < rightSlope) continue;

                    if (end > leftSlope) break;

                    if (dx * dx + dy * dy <= Radius * Radius)
                        _state[wx, wy] = ExplorationState.Visible;

                    bool isBlocked = field[wx, wy] is Wall || field[wx, wy] is Door { IsOpen: false };

                    if (blocked)
                    {
                        if (isBlocked)
                        {
                            newStart = rightSlope;
                        }
                        else
                        {
                            blocked = false;
                            start = newStart;
                            hitFloorAfterWall = true;
                        }
                    }
                    else if (isBlocked && dist < Radius)
                    {
                        blocked = true;
                        ScanOctant(px, py, field, octant, dist + 1, start, leftSlope);

                        newStart = rightSlope;
                    }
                }

                if (blocked && !hitFloorAfterWall) break;
            }
        }

        private static int TransformX(int dx, int dy, int octant) => octant switch
        {
            0 => dx, //  dx,  dy
            1 => dy, //  dy,  dx
            2 => dy, //  dy, -dx
            3 => dx, //  dx, -dy
            4 => -dx, // -dx, -dy
            5 => -dy, // -dy, -dx
            6 => -dy, // -dy,  dx
            7 => -dx, // -dx,  dy
            _ => dx
        };

        private static int TransformY(int dx, int dy, int octant) => octant switch
        {
            0 => dy, //  dx,  dy
            1 => dx, //  dy,  dx
            2 => -dx, //  dy, -dx
            3 => -dy, //  dx, -dy
            4 => -dy, // -dx, -dy
            5 => -dx, // -dy, -dx
            6 => dx, // -dy,  dx
            7 => dy, // -dx,  dy
            _ => dy
        };
    }
}

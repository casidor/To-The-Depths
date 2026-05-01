using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI
{
    public class UIConfig
    {
        public const int TileSize = 16;
        public const int SpriteSize = 16;
        public const int SpriteSpacing = 1;
        public const int SpriteStep = SpriteSize + SpriteSpacing; // 17

        public static readonly Typeface GameFont = new Typeface(
            new FontFamily("Cascadia Code, Consolas, Courier New, monospace"));
    }

    public static class SpriteCoords
    {
        public static readonly (int col, int row) Floor = (0, 0);
        public static readonly (int col, int row) Wall = (10, 17);
        public static readonly (int col, int row) Gold = (41, 4);
        public static readonly (int col, int row) Key = (33, 11);
        public static readonly (int col, int row) Exit = (3, 6);
        public static readonly (int col, int row) Enemy = (29, 6);
        public static readonly (int col, int row) Player = (25, 0);
        public static readonly (int col, int row) Altar = (2, 14);
        public static readonly (int col, int row) FullHP = (42, 10);
        public static readonly (int col, int row) HalfHP = (41, 10);
        public static readonly (int col, int row) ZeroHP = (40, 10);
        public static readonly (int col, int row) Door = (11, 11);
        public static readonly (int col, int row) Dagger = (36, 6);
        public static readonly (int col, int row) Bow = (38, 6);
    }
    public static class TileColors
    {
        public static readonly IBrush Wall = new SolidColorBrush(Color.FromRgb(107, 114, 128));
        public static readonly IBrush Floor = new SolidColorBrush(Color.FromRgb(18, 18, 30));
        public static readonly IBrush Gold = new SolidColorBrush(Color.FromRgb(240, 192, 64));
        public static readonly IBrush Key = new SolidColorBrush(Color.FromRgb(64, 170, 255));
        public static readonly IBrush Player = new SolidColorBrush(Color.FromRgb(90, 255, 140));
        public static readonly IBrush Exit = new SolidColorBrush(Color.FromRgb(255, 107, 53));
        public static readonly IBrush Enemy = new SolidColorBrush(Color.FromRgb(226, 75, 74));
        public static readonly IBrush Altar = new SolidColorBrush(Color.FromRgb(220, 50, 50));
        public static readonly IBrush Fog = new SolidColorBrush(Color.FromRgb(15, 15, 25));
        public static readonly IBrush Background = Brushes.Black;
        public static readonly IBrush Door = new SolidColorBrush(Color.FromRgb(139, 90, 43));
    }
}

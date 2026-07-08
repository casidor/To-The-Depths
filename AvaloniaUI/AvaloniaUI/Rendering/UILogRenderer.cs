using Avalonia;
using Avalonia.Media;
using GameCore;
using System.Collections.Generic;
using System.Globalization;

namespace AvaloniaUI.Rendering
{
    internal class UILogRenderer
    {
        private const int MaxLogEntries = 4;

        private static readonly Dictionary<LogColor, Color> _logColors = new()
        {
            { LogColor.Normal, Color.FromRgb(192, 192, 192) },
            { LogColor.Good,   Color.FromRgb(90, 255, 140)  },
            { LogColor.Bad,    Color.FromRgb(226, 75, 74)   }
        };

        private readonly record struct LogEntry(string Text, Color Color);
        private readonly List<LogEntry> _entries = new();

        public void AddLogEntry(string text, LogColor color)
        {
            _entries.Add(new LogEntry(text, _logColors[color]));
            if (_entries.Count > MaxLogEntries)
                _entries.RemoveAt(0);
        }

        public void Draw(DrawingContext context, Size bounds, Typeface font, float zoom)
        {
            if (_entries.Count == 0) return;

            double x = 10;
            double y = bounds.Height - 20;
            double fontSize = UIConfig.TileSize * zoom * 0.4;
            fontSize = System.Math.Clamp(fontSize, 10, 18);

            for (int i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                var brush = new SolidColorBrush(entry.Color);
                var text = new FormattedText(
                    entry.Text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    font,
                    fontSize,
                    brush);

                var bgRect = new Rect(x - 2, y - 2, text.Width + 4, text.Height + 4);
                context.DrawRectangle(new SolidColorBrush(Color.FromArgb(120, 0, 0, 0)), null, bgRect);
                context.DrawText(text, new Point(x, y));
                y -= text.Height + 4;
            }
        }
    }
}
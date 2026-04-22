using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.Views
{
    internal class GameCanvas : Control
    {
        private GameField? _field;
        private Player? _player;
        private readonly Dictionary<Type, IBrush> _brushes = new()
        {
            { typeof(Wall), TileColors.Wall },
            { typeof(Floor), TileColors.Floor },
            { typeof(Gold), TileColors.Gold },
            { typeof(Key), TileColors.Key },
            { typeof(Exit), TileColors.Exit },
            { typeof(Enemy), TileColors.Enemy },
            { typeof(Altar), TileColors.Altar },
            { typeof(GameCore.Models.Void), TileColors.Background },
        };
        private IBrush GetBrush(GameObject obj)
            => _brushes.TryGetValue(obj.GetType(), out var brush) ? brush : TileColors.Floor;
        public void SetGameState(GameField field, Player player)
        {
            _field = field;
            _player = player;
            InvalidateVisual();
        }
        public override void Render(DrawingContext context)
        {
            if (_field == null || _player == null) return;
            for (int y = 0; y < _field.Height; y++)
            {
                for (int x = 0; x < _field.Width; x++)
                {
                    var tile = _field[x, y];
                    bool isPlayer = x == _player.X && y == _player.Y;
                    char symbol = isPlayer ? GameSymbols.Player : tile.Symbol;
                    IBrush brush = isPlayer ? TileColors.Player : GetBrush(tile);
                    DrawTile(context, symbol, brush, x, y);
                }
            }
        }

        private void DrawTile(DrawingContext context, char symbol, IBrush brush, int x, int y)
        {
            var rect = new Rect(
                x * UIConfig.TileSize,
                y * UIConfig.TileSize,
                UIConfig.TileSize,
                UIConfig.TileSize
            );
            context.DrawRectangle(brush, null, rect);
            //context.DrawRectangle(TileColors.Background, null, rect);

            var text = new FormattedText(
                symbol.ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                UIConfig.GameFont,
                UIConfig.TileSize,
                brush
            );

            context.DrawText(text, new Point(
                x * UIConfig.TileSize,
                y * UIConfig.TileSize
            ));
        }
    }
}

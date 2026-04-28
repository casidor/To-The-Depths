using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AvaloniaUI.Views
{
    internal class GameCanvas : Control
    {
        private GameField? _field;
        private Player? _player;
        private Bitmap? _tileset;

        private static Avalonia.Rect GetSourceRect(int col, int row)
        {
            return new Avalonia.Rect(
                col * UIConfig.SpriteStep,
                row * UIConfig.SpriteStep,
                UIConfig.SpriteSize,
                UIConfig.SpriteSize
            );
        }

        private static readonly Dictionary<Type, (int col, int row)> _spriteMap = new()
        {
            { typeof(Floor),               SpriteCoords.Floor  },
            { typeof(Wall),                SpriteCoords.Wall   },
            { typeof(Gold),                SpriteCoords.Gold   },
            { typeof(Key),                 SpriteCoords.Key    },
            { typeof(Exit),                SpriteCoords.Exit   },
            { typeof(Enemy),               SpriteCoords.Enemy  },
            { typeof(Altar),               SpriteCoords.Altar  },
        };

        public void LoadTileset(string path)
        {
            if (File.Exists(path))
                _tileset = new Bitmap(path);
        }

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

                    var destRect = new Avalonia.Rect(
                        x * UIConfig.TileSize,
                        y * UIConfig.TileSize,
                        UIConfig.TileSize,
                        UIConfig.TileSize
                    );

                    if (tile is GameCore.Models.Void)
                    {
                        context.DrawRectangle(Brushes.Black, null, destRect);
                        continue;
                    }

                    if (_tileset != null)
                    {
                        (int col, int row) coords;

                        if (isPlayer)
                            coords = SpriteCoords.Player;
                        else if (!_spriteMap.TryGetValue(tile.GetType(), out coords))
                            coords = SpriteCoords.Floor;

                        var sourceRect = GetSourceRect(coords.col, coords.row);
                        context.DrawImage(_tileset, sourceRect, destRect);
                    }
                    else
                    {
                        DrawFallbackTile(context, tile, isPlayer, destRect, x, y);
                    }
                }
            }
        }

        private void DrawFallbackTile(DrawingContext context, GameObject tile, bool isPlayer, Avalonia.Rect destRect, int x, int y)
        {
            IBrush brush = isPlayer ? TileColors.Player : GetFallbackBrush(tile);
            char symbol = isPlayer ? GameSymbols.Player : tile.Symbol;
            context.DrawRectangle(brush, null, destRect);
            var text = new FormattedText(
                symbol.ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                UIConfig.GameFont,
                UIConfig.TileSize,
                brush
            );
            context.DrawText(text, new Point(x * UIConfig.TileSize, y * UIConfig.TileSize));
        }

        private static readonly Dictionary<Type, IBrush> _brushes = new()
        {
            { typeof(Wall),                TileColors.Wall   },
            { typeof(Floor),               TileColors.Floor  },
            { typeof(Gold),                TileColors.Gold   },
            { typeof(Key),                 TileColors.Key    },
            { typeof(Exit),                TileColors.Exit   },
            { typeof(Enemy),               TileColors.Enemy  },
            { typeof(Altar),               TileColors.Altar  },
            { typeof(GameCore.Models.Void),TileColors.Background },
        };

        private static IBrush GetFallbackBrush(GameObject obj)
            => _brushes.TryGetValue(obj.GetType(), out var brush) ? brush : TileColors.Floor;
    }
}

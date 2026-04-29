using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.IO;

namespace AvaloniaUI.Views
{
    internal class GameCanvas : Control
    {
        private GameField? _field;
        private Player? _player;
        private Bitmap? _tileset;

        private float _zoom = 2f;
        private const float ZoomMin = 0.5f;
        private const float ZoomMax = 6f;
        private const float ZoomStep = 0.15f;

        private double _camX = 0;
        private double _camY = 0;
        private double _targetCamX = 0;
        private double _targetCamY = 0;

        private bool _isDragging = false;
        private bool _centeredOnce = false;
        private Point _dragStart;
        private double _camXAtDrag;
        private double _camYAtDrag;
        private readonly DispatcherTimer _timer;

        private static readonly Dictionary<Type, (int col, int row)> _spriteMap = new()
        {
            { typeof(Floor),                SpriteCoords.Floor },
            { typeof(Wall),                 SpriteCoords.Wall  },
            { typeof(Gold),                 SpriteCoords.Gold  },
            { typeof(GameCore.Models.Key),  SpriteCoords.Key   },
            { typeof(Exit),                 SpriteCoords.Exit  },
            { typeof(Enemy),                SpriteCoords.Enemy },
            { typeof(Altar),                SpriteCoords.Altar },
        };

        public GameCanvas()
        {
            ClipToBounds = true;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _timer.Tick += (_, _) =>
            {
                bool needsRedraw = false;
                double dx = _targetCamX - _camX;
                double dy = _targetCamY - _camY;
                if (Math.Abs(dx) > 0.5 || Math.Abs(dy) > 0.5)
                {
                    _camX += dx * 0.12;
                    _camY += dy * 0.12;
                    needsRedraw = true;
                }

                for (int i = _floatingTexts.Count - 1; i >= 0; i--)
                {
                    var ft = _floatingTexts[i];
                    ft.Life -= 0.016;
                    ft.WorldY -= 0.03;

                    if (ft.Life <= 0)
                    {
                        _floatingTexts.RemoveAt(i);
                    }
                    needsRedraw = true;
                }

                if (needsRedraw) InvalidateVisual();
            };
            _timer.Start();
        }

        private class FloatingText
        {
            public double WorldX { get; set; }
            public double WorldY { get; set; }
            public string Text { get; set; } = "";
            public double Life { get; set; } = 1.0;
            public char? Icon { get; set; }
        }
        private readonly List<FloatingText> _floatingTexts = new();

        public void AddFloatingText(double gridX, double gridY, string text, char? icon = null)
        {
            _floatingTexts.Add(new FloatingText { WorldX = gridX, WorldY = gridY, Text = text, Icon = icon });
        }
        private static Avalonia.Rect GetSourceRect(int col, int row) =>
            new(col * UIConfig.SpriteStep, row * UIConfig.SpriteStep,
                UIConfig.SpriteSize, UIConfig.SpriteSize);

        public void LoadTileset(string path)
        {
            if (File.Exists(path))
                _tileset = new Bitmap(path);
        }

        public void SetGameState(GameField field, Player player)
        {
            _field = field;
            _player = player;
            UpdateTarget();
            InvalidateVisual();
        }

        public void CenterOnPlayer()
        {
            if (_player == null) return;
            float tileSize = UIConfig.TileSize * _zoom;
            _targetCamX = _camX = _player.X * tileSize + tileSize / 2.0 - Bounds.Width / 2.0;
            _targetCamY = _camY = _player.Y * tileSize + tileSize / 2.0 - Bounds.Height / 2.0;
            InvalidateVisual();
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            if (!_centeredOnce && _player != null)
            {
                CenterOnPlayer();
                _centeredOnce = true;
            }
        }

        private void UpdateTarget()
        {
            if (_player == null || Bounds.Width == 0) return;

            float tileSize = UIConfig.TileSize * _zoom;

            double playerScreenX = _player.X * tileSize - _camX;
            double playerScreenY = _player.Y * tileSize - _camY;

            double marginX = Bounds.Width * 0.4;
            double marginY = Bounds.Height * 0.4;

            if (playerScreenX < marginX)
                _targetCamX = _player.X * tileSize - marginX;
            else if (playerScreenX > Bounds.Width - marginX - tileSize)
                _targetCamX = _player.X * tileSize - Bounds.Width + marginX + tileSize;

            if (playerScreenY < marginY)
                _targetCamY = _player.Y * tileSize - marginY;
            else if (playerScreenY > Bounds.Height - marginY - tileSize)
                _targetCamY = _player.Y * tileSize - Bounds.Height + marginY + tileSize;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                _isDragging = true;
                _dragStart = e.GetPosition(this);
                _camXAtDrag = _camX;
                _camYAtDrag = _camY;
                e.Handled = true;
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_isDragging)
            {
                var pos = e.GetPosition(this);
                _camX = _targetCamX = _camXAtDrag - (pos.X - _dragStart.X);
                _camY = _targetCamY = _camYAtDrag - (pos.Y - _dragStart.Y);
                InvalidateVisual();
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _isDragging = false;
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            var mouse = e.GetPosition(this);
            double worldX = (_camX + mouse.X) / _zoom;
            double worldY = (_camY + mouse.Y) / _zoom;

            _zoom += e.Delta.Y > 0 ? ZoomStep : -ZoomStep;
            _zoom = Math.Clamp(_zoom, ZoomMin, ZoomMax);

            _camX = _targetCamX = worldX * _zoom - mouse.X;
            _camY = _targetCamY = worldY * _zoom - mouse.Y;

            InvalidateVisual();
            e.Handled = true;
        }

        public override void Render(DrawingContext context)
        {
            if (_field == null || _player == null) return;

            float tileSize = UIConfig.TileSize * _zoom;
            double viewW = Bounds.Width;
            double viewH = Bounds.Height;

            context.DrawRectangle(Brushes.Black, null, new Avalonia.Rect(0, 0, viewW, viewH));

            int startX = Math.Max(0, (int)(_camX / tileSize));
            int startY = Math.Max(0, (int)(_camY / tileSize));
            int endX = Math.Min(_field.Width, (int)((_camX + viewW) / tileSize) + 2);
            int endY = Math.Min(_field.Height, (int)((_camY + viewH) / tileSize) + 2);

            using var _ = context.PushTransform(Matrix.CreateTranslation(-_camX, -_camY));

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    var tile = _field[x, y];
                    bool isPlayer = x == _player.X && y == _player.Y;

                    var destRect = new Avalonia.Rect(
                        x * tileSize, y * tileSize, tileSize, tileSize);

                    if (tile is GameCore.Models.Void)
                    {
                        context.DrawRectangle(Brushes.Black, null, destRect);
                        continue;
                    }

                    var fov = _field.Fov[x, y];

                    if (fov == ExplorationState.Unknown)
                    {
                        context.DrawRectangle(Brushes.Black, null, destRect);
                        continue;
                    }

                    if (_tileset != null)
                    {
                        (int col, int row) coords;

                        bool isStatic = tile is Wall or Floor;
                        if (fov == ExplorationState.Explored && !isStatic)
                            coords = SpriteCoords.Floor;
                        else if (isPlayer && fov == ExplorationState.Visible)
                            coords = SpriteCoords.Player;
                        else if (!_spriteMap.TryGetValue(tile.GetType(), out coords))
                            coords = SpriteCoords.Floor;

                        context.DrawImage(_tileset, GetSourceRect(coords.col, coords.row), destRect);

                        if (fov == ExplorationState.Explored)
                            context.DrawRectangle(
                                new SolidColorBrush(Color.FromArgb(120, 0, 0, 0)),
                                null, destRect);
                    }
                    else
                    {
                        DrawFallbackTile(context, tile, isPlayer, destRect, x, y);
                    }
                    if (fov == ExplorationState.Visible && tile is Enemy enemy && enemy.HP < enemy.MaxHP && enemy.HP > 0)
                    {
                        double hpPercentage = Math.Max(0, (double)enemy.HP / enemy.MaxHP);
                        double barHeight = 4 * _zoom;
                        double yOffset = -(barHeight + 2);

                        var bgRect = new Avalonia.Rect(destRect.X, destRect.Y + yOffset, destRect.Width, barHeight);
                        context.DrawRectangle(Brushes.Black, null, bgRect);

                        var fgRect = new Avalonia.Rect(destRect.X, destRect.Y + yOffset, destRect.Width * hpPercentage, barHeight);
                        context.DrawRectangle(Brushes.Red, null, fgRect);
                    }
                }
            }
            foreach (var ft in _floatingTexts)
            {
                byte alpha = (byte)(Math.Clamp(ft.Life, 0.0, 1.0) * 255);

                var textBrush = new SolidColorBrush(Color.FromArgb(alpha, 255, 50, 50));
                var outlinePen = new Pen(new SolidColorBrush(Color.FromArgb(alpha, 0, 0, 0)), 2 * _zoom);

                string displayString = (ft.Icon.HasValue ? $"{ft.Icon} " : "") + ft.Text;

                var textFormat = new FormattedText(
                displayString,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                UIConfig.GameFont,
                UIConfig.TileSize * _zoom * 0.6,
                textBrush);

                double screenX = ft.WorldX * tileSize + (tileSize * 0.2);
                double screenY = ft.WorldY * tileSize;

                var textGeometry = textFormat.BuildGeometry(new Avalonia.Point(screenX, screenY));

                context.DrawGeometry(null, outlinePen, textGeometry);

                context.DrawGeometry(textBrush, null, textGeometry);
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
                brush);
            context.DrawText(text, new Point(x * UIConfig.TileSize, y * UIConfig.TileSize));
        }

        private static readonly Dictionary<Type, IBrush> _brushes = new()
        {
            { typeof(Wall),                 TileColors.Wall       },
            { typeof(Floor),                TileColors.Floor      },
            { typeof(Gold),                 TileColors.Gold       },
            { typeof(GameCore.Models.Key),  TileColors.Key        },
            { typeof(Exit),                 TileColors.Exit       },
            { typeof(Enemy),                TileColors.Enemy      },
            { typeof(Altar),                TileColors.Altar      },
            { typeof(GameCore.Models.Void), TileColors.Background },
        };

        private static IBrush GetFallbackBrush(GameObject obj)
            => _brushes.TryGetValue(obj.GetType(), out var brush) ? brush : TileColors.Floor;
    }
}

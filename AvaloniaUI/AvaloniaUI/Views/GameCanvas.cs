using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using GameCore;
using GameCore.Models.Entities;
using GameCore.Models.Items.Weapons;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.IO;

namespace AvaloniaUI.Views
{
    internal class GameCanvas : Control
    {
        #region Constants
        private const float ZoomMin = 1f;
        private const float ZoomMax = 6f;
        private const float ZoomStep = 0.15f;
        private const double TimerInterval = 16;
        private const double CameraEasing = 0.12;
        private const double FloatingTextDecayRate = 0.016;
        private const double FloatingTextSpeedY = 0.03;
        private const double CameraMarginRatio = 0.4;
        private const double ExploredTileDarkness = 120;
        #endregion

        #region Texture Cache
        private static readonly Dictionary<string, Bitmap> _textures = new();
        private readonly SolidColorBrush _fogBrush = new(Color.FromArgb(120, 0, 0, 0));
        private readonly SolidColorBrush _floatingTextBrush = new(Color.FromArgb(255, 255, 50, 50));
        private readonly SolidColorBrush _floatingOutlineBrush = new(Color.FromArgb(255, 0, 0, 0));
        private readonly SolidColorBrush _aimHighlightBrush = new(Color.FromArgb(80, 255, 220, 0));
        private static readonly SolidColorBrush _aimHoverBrush = new(Color.FromArgb(180, 255, 220, 0));
        private Pen _floatingOutlinePen = null!;
        private Bitmap? GetTexture(string spriteName)
        {
            string fileName = $"{spriteName}.png";
            if (!_textures.TryGetValue(fileName, out var bitmap))
            {
                try
                {
                    bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AvaloniaUI/Assets/{fileName}")));
                    _textures[fileName] = bitmap;
                }
                catch
                {
                    return null;
                }
            }
            return bitmap;
        }
        #endregion

        #region Game State Fields
        private GameField? _field;
        private Player? _player;
        #endregion

        #region Rendering Fields
        private float _zoom = 2f;
        private double ScaledTileSize => UIConfig.TileSize * _zoom;
        private static readonly Dictionary<Type, IBrush> _brushes = new()
        {
            { typeof(Wall),                 TileColors.Wall       },
            { typeof(Floor),                TileColors.Floor      },
            { typeof(Gold),                 TileColors.Gold       },
            { typeof(GameCore.Models.Objects.Key),  TileColors.Key        },
            { typeof(Exit),                 TileColors.Exit       },
            { typeof(Enemy),                TileColors.Enemy      },
            { typeof(Altar),                TileColors.Altar      },
            { typeof(GameCore.Models.Objects.Void), TileColors.Background },
            { typeof(Door),                 TileColors.Door      },
        };
        #endregion

        #region Camera Fields
        private double _camX;
        private double _camY;
        private double _targetCamX;
        private double _targetCamY;
        #endregion

        #region Input Fields
        private bool _isDragging;
        private Point _dragStart;
        private double _camXAtDrag;
        private double _camYAtDrag;
        #endregion

        #region Aim Mode
        public bool IsAimingMode { get; set; }
        public int AimRange { get; set; }
        private (int x, int y) _hoveredCell = (-1, -1);
        public event Action<int, int>? AimCellClicked;
        #endregion

        #region Initialization Fields
        private bool _centeredOnce;
        private DispatcherTimer _timer;
        private readonly List<FloatingText> _floatingTexts = new();
        #endregion

        public GameCanvas()
        {
            ClipToBounds = true;
            RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);
            InitializeTimer();
            _floatingOutlinePen = new Pen(_floatingOutlineBrush, 2);
        }

        #region Initialization
        private void InitializeTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(TimerInterval) };
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            bool needsRedraw = UpdateCamera();
            needsRedraw |= UpdateFloatingTexts();
            if (needsRedraw)
                InvalidateVisual();
        }

        private bool UpdateCamera()
        {
            double dx = _targetCamX - _camX;
            double dy = _targetCamY - _camY;

            if (Math.Abs(dx) > 0.5 || Math.Abs(dy) > 0.5)
            {
                _camX += dx * CameraEasing;
                _camY += dy * CameraEasing;
                return true;
            }
            else if (_camX != _targetCamX || _camY != _targetCamY)
            {
                _camX = _targetCamX;
                _camY = _targetCamY;
                return true;
            }
            return false;
        }

        private bool UpdateFloatingTexts()
        {
            bool hasChanges = false;
            for (int i = _floatingTexts.Count - 1; i >= 0; i--)
            {
                var ft = _floatingTexts[i];
                ft.Life -= FloatingTextDecayRate;
                ft.WorldY -= FloatingTextSpeedY;
                if (ft.Life <= 0)
                    _floatingTexts.RemoveAt(i);
                else
                    hasChanges = true;
            }
            return hasChanges;
        }
        #endregion

        #region Public Methods
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
            double tileSize = ScaledTileSize;
            _targetCamX = _camX = _player.X * tileSize + tileSize / 2.0 - Bounds.Width / 2.0;
            _targetCamY = _camY = _player.Y * tileSize + tileSize / 2.0 - Bounds.Height / 2.0;
            InvalidateVisual();
        }

        public void AddFloatingText(double gridX, double gridY, string text, char? icon = null)
        {
            _floatingTexts.Add(new FloatingText { WorldX = gridX, WorldY = gridY, Text = text, Icon = icon });
        }
        #endregion

        #region Camera
        private void UpdateTarget()
        {
            if (_player == null || Bounds.Width == 0) return;
            double tileSize = ScaledTileSize;
            double playerScreenX = _player.X * tileSize - _camX;
            double playerScreenY = _player.Y * tileSize - _camY;
            double marginX = Bounds.Width * CameraMarginRatio;
            double marginY = Bounds.Height * CameraMarginRatio;

            if (playerScreenX < marginX)
                _targetCamX = _player.X * tileSize - marginX;
            else if (playerScreenX > Bounds.Width - marginX - tileSize)
                _targetCamX = _player.X * tileSize - Bounds.Width + marginX + tileSize;

            if (playerScreenY < marginY)
                _targetCamY = _player.Y * tileSize - marginY;
            else if (playerScreenY > Bounds.Height - marginY - tileSize)
                _targetCamY = _player.Y * tileSize - Bounds.Height + marginY + tileSize;
        }
        #endregion

        #region Input
        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            base.OnSizeChanged(e);
            if (!_centeredOnce && _player != null)
            {
                CenterOnPlayer();
                _centeredOnce = true;
            }
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
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && IsAimingMode)
            {
                var pos = e.GetPosition(this);
                double tileSize = Math.Round(UIConfig.TileSize * _zoom);
                int gridX = (int)((Math.Round(_camX) + pos.X) / tileSize);
                int gridY = (int)((Math.Round(_camY) + pos.Y) / tileSize);

                int dx = Math.Abs(gridX - _player!.X);
                int dy = Math.Abs(gridY - _player!.Y);
                if (dx <= AimRange && dy <= AimRange)
                    AimCellClicked?.Invoke(gridX, gridY);

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
            if (IsAimingMode)
            {
                var pos = e.GetPosition(this);
                double tileSize = Math.Round(UIConfig.TileSize * _zoom);
                int gx = (int)((Math.Round(_camX) + pos.X) / tileSize);
                int gy = (int)((Math.Round(_camY) + pos.Y) / tileSize);
                if (_hoveredCell != (gx, gy))
                {
                    _hoveredCell = (gx, gy);
                    InvalidateVisual();
                }
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
        #endregion

        #region Rendering
        public override void Render(DrawingContext context)
        {
            if (_field == null || _player == null) return;
            double tileSize = Math.Round(UIConfig.TileSize * _zoom);
            double viewW = Bounds.Width;
            double viewH = Bounds.Height;

            context.DrawRectangle(Brushes.Black, null, new Avalonia.Rect(0, 0, viewW, viewH));

            int startX = Math.Max(0, (int)(_camX / tileSize));
            int startY = Math.Max(0, (int)(_camY / tileSize));
            int endX = Math.Min(_field.Width, (int)((_camX + viewW) / tileSize) + 2);
            int endY = Math.Min(_field.Height, (int)((_camY + viewH) / tileSize) + 2);

            using (var _ = context.PushTransform(Matrix.CreateTranslation(-Math.Round(_camX), -Math.Round(_camY))))
            {
                RenderTilesLayer(context, tileSize, startX, startY, endX, endY);
                RenderEnemiesLayer(context, tileSize, startX, startY, endX, endY);
                RenderPlayerLayer(context, tileSize);
                RenderHPBarsLayer(context, tileSize, startX, startY, endX, endY);
                RenderAimLayer(context, tileSize);
                RenderFloatingTextsLayer(context, tileSize);
            }
            RenderUILog(context);
        }

        private void RenderTilesLayer(DrawingContext context, double tileSize, int startX, int startY, int endX, int endY)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    var tile = _field![x, y];
                    var destRect = (tile is Wall)
                        ? GetSeamlessRect(x, y, tileSize)
                        : GetTileRectWithGap(x, y, tileSize);

                    if (tile is GameCore.Models.Objects.Void)
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
                    if (tile is not GameCore.Models.Objects.Void && fov != ExplorationState.Unknown)
                    {
                        context.DrawRectangle(_fogBrush, null, destRect);
                    }

                    DrawTileSprite(context, tile, fov, destRect);

                    if (fov == ExplorationState.Explored)
                        context.DrawRectangle(
                            new SolidColorBrush(Color.FromArgb((byte)ExploredTileDarkness, 0, 0, 0)),
                            null, destRect);
                }
            }
        }

        private void RenderEnemiesLayer(DrawingContext context, double tileSize, int startX, int startY, int endX, int endY)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    if (_field!.Fov[x, y] != ExplorationState.Visible)
                        continue;

                    var entity = _field.GetEntity(x, y);
                    if (entity == null)
                        continue;

                    var destRect = GetTileRectWithGap(x, y, tileSize);
                    var bitmap = GetTexture(entity.SpriteName);

                    if (bitmap != null)
                        context.DrawImage(bitmap, destRect);
                    else
                        DrawFallbackTile(context, null, false, destRect, x, y);
                }
            }
        }

        private void RenderPlayerLayer(DrawingContext context, double tileSize)
        {
            var destRect = GetTileRectWithGap(_player!.X, _player.Y, tileSize);
            var bitmap = GetTexture(_player.SpriteName);

            if (bitmap != null)
                context.DrawImage(bitmap, destRect);
            else
                DrawFallbackTile(context, null, true, destRect, _player.X, _player.Y);
        }

        private void RenderHPBarsLayer(DrawingContext context, double tileSize, int startX, int startY, int endX, int endY)
        {
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    if (_field!.Fov[x, y] != ExplorationState.Visible)
                        continue;

                    if (_field.GetEntity(x, y) is Enemy enemy && enemy.HP < enemy.MaxHP)
                    {
                        var destRect = GetTileRectWithGap(x, y, tileSize);
                        DrawHPBar(context, destRect, enemy);
                    }
                }
            }
        }

        private void RenderAimLayer(DrawingContext context, double tileSize)
        {
            if (!IsAimingMode || _player == null) return;

            for (int dy = -AimRange; dy <= AimRange; dy++)
                for (int dx = -AimRange; dx <= AimRange; dx++)
                {
                    int cx = _player.X + dx;
                    int cy = _player.Y + dy;
                    if (cx < 0 || cx >= _field!.Width || cy < 0 || cy >= _field.Height) continue;
                    if (_field.Fov[cx, cy] != ExplorationState.Visible) continue;
                    var rect = GetTileRectWithGap(cx, cy, tileSize);
                    bool isHovered = cx == _hoveredCell.x && cy == _hoveredCell.y;
                    context.DrawRectangle(isHovered ? _aimHoverBrush : _aimHighlightBrush, null, rect);
                }
        }

        private void RenderFloatingTextsLayer(DrawingContext context, double tileSize)
        {
            foreach (var ft in _floatingTexts)
                DrawFloatingText(context, ft, tileSize);
        }

        private void DrawTileSprite(DrawingContext context, GameObject tile, ExplorationState fov, Avalonia.Rect destRect)
        {
            bool isStatic = tile is Wall or Floor;
            string spriteName = fov == ExplorationState.Explored && !isStatic
                ? "floor"
                : tile.SpriteName;

            var bitmap = GetTexture(spriteName);
            if (bitmap != null)
            {
                context.DrawImage(bitmap, destRect);
            }
            else
            {
                DrawFallbackTile(context, tile, false, destRect, (int)(destRect.X / UIConfig.TileSize), (int)(destRect.Y / UIConfig.TileSize));
            }
        }

        private void DrawHPBar(DrawingContext context, Avalonia.Rect destRect, Enemy enemy)
        {
            double hp = Math.Max(0, (double)enemy.HP / enemy.MaxHP);
            double barH = 4 * _zoom;
            double yOff = -(barH + 2);

            context.DrawRectangle(Brushes.Black, null,
                new Avalonia.Rect(destRect.X, destRect.Y + yOff, destRect.Width, barH));
            context.DrawRectangle(Brushes.Red, null,
                new Avalonia.Rect(destRect.X, destRect.Y + yOff, destRect.Width * hp, barH));
        }

        private void DrawFloatingText(DrawingContext context, FloatingText ft, double tileSize)
        {
            byte alpha = (byte)(Math.Clamp(ft.Life, 0.0, 1.0) * 255);
            _floatingTextBrush.Color = Color.FromArgb(alpha, 255, 50, 50);
            _floatingOutlineBrush.Color = Color.FromArgb(alpha, 0, 0, 0);
            _floatingOutlinePen = new Pen(_floatingOutlineBrush, 2 * _zoom);

            string displayString = (ft.Icon.HasValue ? $"{ft.Icon} " : "") + ft.Text;

            var textFormat = new FormattedText(
                displayString,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                UIConfig.GameFont,
                UIConfig.TileSize * _zoom * 0.6,
                _floatingTextBrush);

            double screenX = ft.WorldX * tileSize + (tileSize * 0.2);
            double screenY = ft.WorldY * tileSize;
            var textGeometry = textFormat.BuildGeometry(new Avalonia.Point(screenX, screenY));

            context.DrawGeometry(null, _floatingOutlinePen, textGeometry);
            context.DrawGeometry(_floatingTextBrush, null, textGeometry);
        }

        private void DrawFallbackTile(DrawingContext context, GameObject? tile, bool isPlayer, Avalonia.Rect destRect, int x, int y)
        {
            IBrush brush = isPlayer ? TileColors.Player : GetFallbackBrush(tile);
            char symbol = isPlayer ? GameSymbols.Player : tile!.Symbol;

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
        #endregion

        #region Helpers
        private static IBrush GetFallbackBrush(GameObject? obj)
            => obj != null && _brushes.TryGetValue(obj.GetType(), out var brush)
                ? brush
                : TileColors.Floor;
        private Avalonia.Rect GetTileRectWithGap(double gridX, double gridY, double tileSize)
        {
            double px = Math.Round(gridX * tileSize);
            double py = Math.Round(gridY * tileSize);
            double nextPx = Math.Round((gridX + 1) * tileSize);
            double nextPy = Math.Round((gridY + 1) * tileSize);
            return new Avalonia.Rect(px, py, nextPx - px - 1, nextPy - py - 1);
        }

        private Avalonia.Rect GetSeamlessRect(double gridX, double gridY, double tileSize)
        {
            double px = Math.Round(gridX * tileSize);
            double py = Math.Round(gridY * tileSize);
            double nextPx = Math.Round((gridX + 1) * tileSize);
            double nextPy = Math.Round((gridY + 1) * tileSize);
            return new Avalonia.Rect(px, py, nextPx - px + 1, nextPy - py + 1);
        }
        #endregion

        #region Floating Text
        private class FloatingText
        {
            public double WorldX { get; set; }
            public double WorldY { get; set; }
            public string Text { get; set; } = "";
            public double Life { get; set; } = 1.0;
            public char? Icon { get; set; }
        }
        #endregion

        #region UI Log
        private readonly record struct LogEntry(string Text, Avalonia.Media.Color Color);
        private readonly List<LogEntry> _logEntries = new();
        private const int MaxLogEntries = 4;

        private static readonly Dictionary<LogColor, Avalonia.Media.Color> _logColors = new()
        {
        { LogColor.Normal, Avalonia.Media.Color.FromRgb(192, 192, 192) },
        { LogColor.Good,   Avalonia.Media.Color.FromRgb(90, 255, 140)  },
        { LogColor.Bad,    Avalonia.Media.Color.FromRgb(226, 75, 74)   }
        };

        public void AddLogEntry(string text, LogColor color)
        {
            _logEntries.Add(new LogEntry(text, _logColors[color]));
            if (_logEntries.Count > MaxLogEntries)
                _logEntries.RemoveAt(0);
            InvalidateVisual();
        }

        private void RenderUILog(DrawingContext context)
        {
            if (_logEntries.Count == 0) return;

            double x = 10;
            double y = Bounds.Height - 20;
            double fontSize = UIConfig.TileSize * _zoom * 0.4;
            fontSize = Math.Clamp(fontSize, 10, 18);

            for (int i = _logEntries.Count - 1; i >= 0; i--)
            {
                var entry = _logEntries[i];
                var brush = new SolidColorBrush(entry.Color);
                var text = new FormattedText(
                    entry.Text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    UIConfig.GameFont,
                    fontSize,
                    brush);

                var bgRect = new Avalonia.Rect(x - 2, y - 2, text.Width + 4, text.Height + 4);
                context.DrawRectangle(new SolidColorBrush(Avalonia.Media.Color.FromArgb(120, 0, 0, 0)), null, bgRect);
                context.DrawText(text, new Avalonia.Point(x, y));
                y -= text.Height + 4;
            }
        }
        #endregion
    }
}
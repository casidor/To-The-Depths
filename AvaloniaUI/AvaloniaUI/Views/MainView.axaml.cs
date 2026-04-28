using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaUI.ViewModels;
using System.IO;

namespace AvaloniaUI.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var tilesetPath = FindTileset();
            if (tilesetPath != null)
                MapRenderer.LoadTileset(tilesetPath);

            if (DataContext is MainViewModel vm)
            {
                MapRenderer.SetGameState(vm.Field, vm.Player);

                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName is
                        nameof(vm.IsAltarPopupOpen) or
                        nameof(vm.IsAltarResultOpen) or
                        nameof(vm.IsDescendingPopupOpen) or
                        nameof(vm.IsGameOverPopupOpen))
                    {
                        this.Focus();
                    }
                };
            }
            this.Focus();
        }

        private static string? FindTileset()
        {
            var candidates = new[]
            {
                "Assets/colored.png",
                "colored.png",
                Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "colored.png"),
                Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "colored.png"),
            };

            foreach (var path in candidates)
                if (File.Exists(path)) return path;

            return null;
        }

        private void InputElement_OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (e.Key == Key.Escape)
                {
                    if (vm.IsExitPopupOpen)
                    {
                        vm.IsExitPopupOpen = false;
                    }
                    else
                    {
                        vm.RequestExitCommand.Execute(null);
                    }
                    return;
                }
                switch (e.Key)
                {
                    case Key.W: case Key.Up: vm.MovePlayer(0, -1); break;
                    case Key.S: case Key.Down: vm.MovePlayer(0, 1); break;
                    case Key.A: case Key.Left: vm.MovePlayer(-1, 0); break;
                    case Key.D: case Key.Right: vm.MovePlayer(1, 0); break;
                    case Key.Space: MapRenderer.CenterOnPlayer(); break;
                }
                MapRenderer.SetGameState(vm.Field, vm.Player);
                MapRenderer.InvalidateVisual();
            }
        }
    }
}
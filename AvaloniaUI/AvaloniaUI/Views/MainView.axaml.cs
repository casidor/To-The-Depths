using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaUI.ViewModels;

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
            if (DataContext is MainViewModel vm)
            {
                MapRenderer.Width = vm.Field.Width * UIConfig.TileSize;
                MapRenderer.Height = vm.Field.Height * UIConfig.TileSize;
                MapRenderer.SetGameState(vm.Field, vm.Player);
            }
        }

        private void InputElement_OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                switch (e.Key)
                {
                    case Key.W: case Key.Up: vm.MovePlayer(0, -1); break;
                    case Key.S: case Key.Down: vm.MovePlayer(0, 1); break;
                    case Key.A: case Key.Left: vm.MovePlayer(-1, 0); break;
                    case Key.D: case Key.Right: vm.MovePlayer(1, 0); break;
                }
                MapRenderer.InvalidateVisual();
            }
        }
    }
}
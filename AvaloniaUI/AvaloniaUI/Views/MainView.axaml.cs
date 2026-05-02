using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AvaloniaUI.ViewModels;
using GameCore.Models.Items;
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

            if (DataContext is MainViewModel vm)
            {
                MapRenderer.SetGameState(vm.Field, vm.Player);
                MapRenderer.AimCellClicked += (x, y) =>
                {
                    vm.UseWeaponAt(x, y);
                    MapRenderer.SetGameState(vm.Field, vm.Player);
                    MapRenderer.InvalidateVisual();
                };
                vm.FloatingTextRequested += (x, y, text, icon) => MapRenderer.AddFloatingText(x, y, text, icon);
                vm.LogRequested += (text, color) => MapRenderer.AddLogEntry(text, color);
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

        private void InputElement_OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                if (e.Key == Key.Escape)
                {
                    if (vm.IsAimingMode)
                    {
                        vm.ExitAimMode();
                        MapRenderer.IsAimingMode = false;
                        MapRenderer.InvalidateVisual();
                        return;
                    }
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
                if (e.Key >= Key.D1 && e.Key <= Key.D4)
                {
                    vm.Player.Inventory.SwitchSlot((int)e.Key - (int)Key.D1);
                    MapRenderer.SetGameState(vm.Field, vm.Player);
                    vm.UpdateUIPublic();
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
                if (e.Key == Key.E)
                {
                    if (vm.Player.Inventory.ActiveItem is RangedWeapon)
                    {
                        vm.ToggleAimMode();
                        MapRenderer.IsAimingMode = vm.IsAimingMode;
                        if (vm.IsAimingMode && vm.Player.Inventory.ActiveItem is RangedWeapon rw)
                            MapRenderer.AimRange = rw.Range;
                        MapRenderer.InvalidateVisual();
                    }
                    return;
                }
                MapRenderer.SetGameState(vm.Field, vm.Player);
                MapRenderer.InvalidateVisual();
            }
        }
    }
}
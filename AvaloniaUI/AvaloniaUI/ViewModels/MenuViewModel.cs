using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Reflection;

namespace AvaloniaUI.ViewModels
{
    public partial class MenuViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string appVersion = Assembly.GetExecutingAssembly()?.GetName().Version != null ? $"v{Assembly.GetExecutingAssembly()?.GetName().Version.Major}.{Assembly.GetExecutingAssembly()?.GetName().Version.Minor}.{Assembly.GetExecutingAssembly()?.GetName().Version.Build}" : "v1.0.0";
        public event Action? StartGameRequested;
        public event Action? LoadGameRequested;
        public event Action? HelpRequested;

        [RelayCommand]
        private void StartGame() => StartGameRequested?.Invoke();

        [RelayCommand]
        private void LoadGame() => LoadGameRequested?.Invoke();

        [RelayCommand]
        private void Help() => HelpRequested?.Invoke();

        [RelayCommand]
        private void ExitGame() => Environment.Exit(0);
    }
}

using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.ViewModels
{
    public partial class MenuViewModel : ViewModelBase
    {
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

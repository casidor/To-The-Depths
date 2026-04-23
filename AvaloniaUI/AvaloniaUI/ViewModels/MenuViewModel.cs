using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.ViewModels
{
    public partial class MenuViewModel : ViewModelBase
    {
        public event Action? StartGameRequested;
        [RelayCommand]
        private void StartGame()
        {
            StartGameRequested?.Invoke();
        }
        [RelayCommand]
        private void ExitGame()
        {
            Environment.Exit(0);
        }
    }
}

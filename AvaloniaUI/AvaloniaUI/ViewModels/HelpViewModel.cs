using CommunityToolkit.Mvvm.Input;
using System;

namespace AvaloniaUI.ViewModels
{
    public partial class HelpViewModel : ViewModelBase
    {
        public event Action? BackRequested;

        [RelayCommand]
        private void Back() => BackRequested?.Invoke();
    }
}

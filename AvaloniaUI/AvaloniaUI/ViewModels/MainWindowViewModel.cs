using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentPage;
        public MainWindowViewModel()
        {
            var menu = new MenuViewModel();
            menu.StartGameRequested += () =>
            {
                CurrentPage = new MainViewModel();
            };
            _currentPage = menu;
        }
        public void StartGame()
        {
            CurrentPage = new MainViewModel();
        }
    }
}

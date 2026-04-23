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
            ShowMenu();
        }

        private void ShowMenu()
        {
            var menu = new MenuViewModel();
            menu.StartGameRequested += StartGame;
            CurrentPage = menu;
        }

        private void StartGame()
        {
            var game = new MainViewModel();
            game.ReturnToMenuRequested += ShowMenu;
            CurrentPage = game;
        }
    }
}

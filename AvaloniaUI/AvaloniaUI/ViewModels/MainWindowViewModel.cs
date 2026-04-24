using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase? _currentPage;
        [ObservableProperty]
        private string _loadErrorMessage = "";
        [ObservableProperty]    
        private bool _isLoadErrorVisible = false;
        [RelayCommand]
        private void CloseLoadError()
        {
            IsLoadErrorVisible = false;
        }
        public MainWindowViewModel()
        {
            ShowMenu();
        }

        private void ShowMenu()
        {
            var menu = new MenuViewModel();
            menu.StartGameRequested += StartGame;
            menu.LoadGameRequested += LoadGame;
            CurrentPage = menu;
        }

        private void LoadGame()
        {
            var game = new MainViewModel();
            var result = game.TryLoadGame();
            switch (result)
            {
                case SaveResult.NotFound:
                    LoadErrorMessage = "Save file not found!";
                    IsLoadErrorVisible = true;
                    break;
                case SaveResult.Corrupted:
                    LoadErrorMessage = "Save file is corrupted!\n Save deleted.";
                    IsLoadErrorVisible = true;
                    break;
                case SaveResult.Unverified:
                    LoadErrorMessage = "Data integrity check failed.\n Save deleted.";
                    IsLoadErrorVisible = true;
                    break;
                case SaveResult.Success:
                    game.ReturnToMenuRequested += ShowMenu;
                    CurrentPage = game;
                    break;
            }
        }
        private void StartGame()
        {
            var game = new MainViewModel();
            game.StartNewGame();
            game.ReturnToMenuRequested += ShowMenu;
            CurrentPage = game;
        }
    }
}

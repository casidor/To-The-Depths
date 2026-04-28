using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameCore;
using GameCore.Models;
using GameCore.World;
using System;
using System.Numerics;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaUI.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private int currentSeed;
        public string HPText => $" {Player.HP}/{Player.MaxHP}";
        public int PlayerGold => Player.GoldCollected;
        public string KeysText => $" {Player.KeysCollected}/{Config.KeysAmount}";
        public bool HasAllKeys => Player.KeysCollected >= Config.KeysAmount;
        public int CurrentFloor => Player.CurrentFloor;
        public string MissionText => Player.KeysCollected >= Config.KeysAmount
        ? "Exit is OPEN!\nFind the exit!" : $"Collect {Config.KeysAmount} keys!";
        public GameField Field { get; private set; }
        public Player Player { get; private set; }
        public EnemyAI EnemyAI { get; private set; }
        public event Action? ReturnToMenuRequested;
        //Popups
        [ObservableProperty]
        private bool _isGameOverPopupOpen;
        [ObservableProperty]
        private bool _isDescendingPopupOpen;
        //Exit popup
        [ObservableProperty]
        private bool _isExitPopupOpen;
        private bool _hasUnsavedProgress = false;
        [ObservableProperty]
        public string _exitWarningText = "";
        //Altar popup and result popup
        [ObservableProperty]
        private bool _isAltarPopupOpen;
        [ObservableProperty]
        private bool _isAltarResultOpen;
        [ObservableProperty]
        private string _altarResultText = "";
        private Altar? _currentAltar;
        public string AltarOfferText => $"Heal {Config.AltarHeal} HP for {Config.HealCost} Gold?";
        [RelayCommand]
        private void RequestExit()
        {
            ExitWarningText = _hasUnsavedProgress
                ? "Unsaved progress on this floor will be lost!\nExit anyway?"
                : "Do you want to return to the Main Menu?";
            IsExitPopupOpen = true;
        }
        [RelayCommand]
        private void CancelExit() => IsExitPopupOpen = false;
        [RelayCommand]
        private void Continue()
        {
            IsDescendingPopupOpen = false;
            IsAltarResultOpen = false;
            UpdateUI();
        }
        [RelayCommand]
        private void ConfirmExit() => ReturnToMenuRequested?.Invoke();
        [RelayCommand]
        private void AltarConfirm()
        {
            var (success, message) = _currentAltar!.TryHeal(Player);
            AltarResultText = message;
            IsAltarPopupOpen = false;
            IsAltarResultOpen = true;
            UpdateUI();
        }
        [RelayCommand]
        private void AltarCancel()
        {
            IsAltarPopupOpen = false;
            UpdateUI();
        }
        [RelayCommand]
        private void AltarResultClose()
        {
            IsAltarResultOpen = false;
            UpdateUI();
        }
        //Win popup
        [ObservableProperty]
        private bool _isWinPopupOpen;
        //Attack popup
        [ObservableProperty]
        private bool _isAttackPopupOpen;
        public string AttackInfoText =>
            $"HP lost: {Config.EnemyDamage}\nGold stolen: {Config.GoldStolen}";

        private async void ShowAttackPopup()
        {
            IsAttackPopupOpen = true;
            await Task.Delay(1500);
            IsAttackPopupOpen = false;
        }
        public MainViewModel()
        {
        }
        public void StartNewGame()
        {
            currentSeed = new Random().Next();
            var random = new Random(currentSeed + 1);
            var generator = new LevelGenerator();
            var generated = generator.Generate(Config.FieldWidth, Config.FieldHeight, random);
            Field = generated.field;
            Player = new Player(generated.x, generated.y);
            EnemyAI = new EnemyAI(random);
            Field.Fov.Update(Player.X, Player.Y, Field);
            SaveManager.Save(Player, currentSeed);
            _hasUnsavedProgress = false;
            UpdateUI();
        }
        public SaveResult TryLoadGame()
        {
            var (data, saveResult) = SaveManager.Load();
            if(saveResult == SaveResult.Success)
            {
                currentSeed = data.Seed;
                var random = new Random(currentSeed + data.Floor);
                var restored = new LevelGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random);
                Field = restored.field;
                Player = new Player(restored.x, restored.y, data.HP, data.MaxHP, data.Gold, data.Keys, data.Floor);
                EnemyAI = new EnemyAI(random);
                Field.Fov.Update(Player.X, Player.Y, Field);
                _hasUnsavedProgress = false;
                UpdateUI();
                return SaveResult.Success;
            }
            SaveManager.DeleteSave();
            return saveResult;
        }
        public void MovePlayer(int dx, int dy)
        {
            if (IsExitPopupOpen || IsGameOverPopupOpen || IsDescendingPopupOpen ||
                IsAltarPopupOpen || IsAltarResultOpen || IsAttackPopupOpen || !Player.IsAlive) return;
            var interaction = Player.Move(dx, dy, Field);
            _hasUnsavedProgress = true;
            EnemyAI.BuildDistanceMap(Field, Player);
            var worldInteraction = EnemyAI.UpdateEnemies(Field, Player);
            Field.Fov.Update(Player.X, Player.Y, Field);
            if (!Player.IsAlive)
            {
                IsGameOverPopupOpen = true;
                UpdateUI();
                return;
            }
            if (interaction == InteractionResult.Altar)
            {
                if (Field[Player.X, Player.Y] is Altar altar)
                {
                    _currentAltar = altar;
                    IsAltarPopupOpen = true;
                }
            }
            if (worldInteraction == InteractionResult.PlayerAttacked || interaction == InteractionResult.PlayerAttacked)
            {
                ShowAttackPopup();
            }
            if (Player.IsExited)
            {
                if (Player.CurrentFloor == Config.MaxFloor)
                {
                    IsWinPopupOpen = true;
                    UpdateUI();
                    return;
                }
                var random = new Random(currentSeed + Player.CurrentFloor + 1);
                var nextfloor = new LevelGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random);
                Field = nextfloor.field;
                Player.Descend(nextfloor.x, nextfloor.y);
                EnemyAI = new EnemyAI(random);
                Field.Fov.Update(Player.X, Player.Y, Field);
                SaveManager.Save(Player, currentSeed);
                _hasUnsavedProgress = false;
                IsDescendingPopupOpen = true;
                UpdateUI();
            }
            UpdateUI();
        }
        private void UpdateUI()
        {
            OnPropertyChanged(nameof(Field));
            OnPropertyChanged(nameof(HPText));
            OnPropertyChanged(nameof(PlayerGold));
            OnPropertyChanged(nameof(KeysText));
            OnPropertyChanged(nameof(CurrentFloor));
            OnPropertyChanged(nameof(HasAllKeys));
            OnPropertyChanged(nameof(MissionText));
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameCore;
using GameCore.Models;
using GameCore.World;
using GameCore.World.Generator;
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
        public event Action<double, double, string, char?>? FloatingTextRequested;
        public MainViewModel()
        {
        }
        public void StartNewGame()
        {
            currentSeed = new Random().Next();
            var random = new Random(currentSeed + 1);
            var generator = new RoomCorridorGenerator();
            var generated = generator.Generate(Config.FieldWidth, Config.FieldHeight, random, 1);
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
                var restored = new RoomCorridorGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random, data.Floor);
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
            if (IsInputBlocked()) return;

            var interaction = HandlePlayerTurn(dx, dy);
            var worldInteraction = HandleEnemyTurn();

            if (!Player.IsAlive) { IsGameOverPopupOpen = true; UpdateUI(); return; }

            HandleInteractionResult(interaction, worldInteraction, dx, dy);
            HandleWorldState();
            UpdateUI();
        }
        private bool IsInputBlocked() =>
    IsExitPopupOpen || IsGameOverPopupOpen || IsDescendingPopupOpen ||
    IsAltarPopupOpen || IsAltarResultOpen || IsAttackPopupOpen || !Player.IsAlive;

        private InteractionResult HandlePlayerTurn(int dx, int dy)
        {
            _hasUnsavedProgress = true;
            return Player.Move(dx, dy, Field);
        }

        private InteractionResult HandleEnemyTurn()
        {
            EnemyAI.BuildDistanceMap(Field, Player);
            var result = EnemyAI.UpdateEnemies(Field, Player);
            Field.Fov.Update(Player.X, Player.Y, Field);
            return result;
        }

        private void HandleInteractionResult(InteractionResult interaction, InteractionResult world, int dx, int dy)
        {
            if (interaction == InteractionResult.Altar)
                if (Field[Player.X, Player.Y] is Altar altar)
                { _currentAltar = altar; IsAltarPopupOpen = true; }

            if (interaction is InteractionResult.EnemyAttacked or InteractionResult.EnemyKilled)
                FloatingTextRequested?.Invoke(Player.X + dx, Player.Y + dy, $"-{Player.Damage}", '⚔');

            if (world == InteractionResult.PlayerAttacked)
            {
                float offsetX = dx != 0 ? dx : -0.3f;

                FloatingTextRequested?.Invoke(Player.X - offsetX, Player.Y, $"-{Config.EnemyDamage}", '♥');
            }
        }

        private void HandleWorldState()
        {
            if (!Player.IsExited) return;

            if (Player.CurrentFloor == Config.MaxFloor)
            { IsWinPopupOpen = true; UpdateUI(); return; }

            var random = new Random(currentSeed + Player.CurrentFloor + 1);
            var next = new RoomCorridorGenerator().Generate(Config.FieldWidth, Config.FieldHeight, random, Player.CurrentFloor + 1);
            Field = next.field;
            Player.Descend(next.x, next.y);
            EnemyAI = new EnemyAI(random);
            Field.Fov.Update(Player.X, Player.Y, Field);
            SaveManager.Save(Player, currentSeed);
            _hasUnsavedProgress = false;
            IsDescendingPopupOpen = true;
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

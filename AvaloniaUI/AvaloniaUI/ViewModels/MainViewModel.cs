using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameCore;
using GameCore.Models.Entities;
using GameCore.Models.Items;
using GameCore.Models.Objects;
using GameCore.World;
using GameCore.World.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        // Sidebar - Hearts
        private string GetHeartState(int index)
        {
            int full = Player.HP / 20;
            int half = (Player.HP % 20) >= 10 ? 1 : 0;
            if (index < full) return "fullheart";
            if (index == full && half == 1) return "halfheart";
            return "emptyheart";
        }
        public string Heart1 => GetHeartState(0);
        public string Heart2 => GetHeartState(1);
        public string Heart3 => GetHeartState(2);
        public string Heart4 => GetHeartState(3);
        public string Heart5 => GetHeartState(4);
        private int _lastHP = -1;

        private void UpdateHeartsIfChanged()
        {
            if (_lastHP == Player.HP) return;
            _lastHP = Player.HP;
            OnPropertyChanged(nameof(Heart1));
            OnPropertyChanged(nameof(Heart2));
            OnPropertyChanged(nameof(Heart3));
            OnPropertyChanged(nameof(Heart4));
            OnPropertyChanged(nameof(Heart5));
        }

        // Sidebar - Melee
        public string EquippedMeleeText => Player.Inventory.MeleeStatLine;

        // Sidebar - Hotbar
        public Item?[] HotbarSlots => Player.Inventory.Hotbar;
        public int ActiveSlot => Player.Inventory.ActiveSlot;
        public string ActiveItemStatsText => Player.Inventory.ActiveItemStatLine;
        // Hotbar
        private string GetSlotText(int i)
        {
            var item = Player.Inventory.Hotbar[i];
            if (item == null) return "Empty";
            if (item is RangedWeapon rw) return $"{item.Name} {rw.Ammo}/{rw.MaxAmmo}";
            return item.Name;
        }
        public Item? Slot1Item => Player.Inventory.Hotbar[0];
        public Item? Slot2Item => Player.Inventory.Hotbar[1];
        public Item? Slot3Item => Player.Inventory.Hotbar[2];
        public Item? Slot4Item => Player.Inventory.Hotbar[3];
        public bool Slot1Active => Player.Inventory.ActiveSlot == 0;
        public bool Slot2Active => Player.Inventory.ActiveSlot == 1;
        public bool Slot3Active => Player.Inventory.ActiveSlot == 2;
        public bool Slot4Active => Player.Inventory.ActiveSlot == 3;
        private Item?[] _lastHotbar = new Item?[4];

        private void UpdateHotbarIfChanged()
        {
            var hotbar = Player.Inventory.Hotbar;
            for (int i = 0; i < 4; i++)
            {
                if (_lastHotbar[i] != hotbar[i])
                {
                    _lastHotbar[i] = hotbar[i];
                    OnPropertyChanged(i switch
                    {
                        0 => nameof(Slot1Item),
                        1 => nameof(Slot2Item),
                        2 => nameof(Slot3Item),
                        _ => nameof(Slot4Item)
                    });
                }
            }
        }
        private int _lastActiveSlot = -1;

        private void UpdateActiveSlotIfChanged()
        {
            int current = Player.Inventory.ActiveSlot;
            if (_lastActiveSlot == current) return;
            _lastActiveSlot = current;
            OnPropertyChanged(nameof(Slot1Active));
            OnPropertyChanged(nameof(Slot2Active));
            OnPropertyChanged(nameof(Slot3Active));
            OnPropertyChanged(nameof(Slot4Active));
        }
        public void UpdateUIPublic() => UpdateUI();
        // Aiming mode
        [ObservableProperty]
        private bool _isAimingMode;

        public void ToggleAimMode()
        {
            if (IsAimingMode)
            {
                if (Player.Inventory.ActiveItem is RangedWeapon rw)
                {
                    Field.Log.Clear();
                    ExecuteTurn(rw.Use(Player, Field));
                }
                ExitAimMode();
                return;
            }
            if (Player.Inventory.ActiveItem is RangedWeapon)
                IsAimingMode = true;
        }

        public void ExitAimMode()
        {
            IsAimingMode = false;
            UpdateUI();
        }

        public void UseWeaponAt(int x, int y)
        {
            if (Player.Inventory.ActiveItem is not RangedWeapon rw) return;
            Field.Log.Clear();
            ExecuteTurn(rw.UseAt(Player, Field, x, y));
        }
        // Log
        public event Action<string, LogColor>? LogRequested;
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
            Field.Log.Clear();
            var interaction = HandlePlayerTurn(dx, dy);
            ExecuteTurn(interaction, dx, dy);
        }
        private void ExecuteTurn(UseResult useResult, int dx = 0, int dy = 0)
        {
            var interaction = useResult == UseResult.Hit
                ? InteractionResult.EnemyAttacked
                : InteractionResult.None;
            ExecuteTurn(interaction, dx, dy);
        }

        private void ExecuteTurn(InteractionResult interaction = InteractionResult.None, int dx = 0, int dy = 0)
        {
            var worldInteraction = HandleEnemyTurn();
            if (!Player.IsAlive) { IsGameOverPopupOpen = true; UpdateUI(); return; }
            HandleInteractionResult(interaction, worldInteraction, dx, dy);
            foreach (var e in Field.Log.Get(GameEventType.DamageDealt, GameEventType.EnemyKilled, GameEventType.Missed))
                FloatingTextRequested?.Invoke(e.X, e.Y, e.Text, e.Icon);

            foreach (var e in Field.Log.Get(GameEventType.DamageTaken))
            {
                float offsetX = dx != 0 ? dx : -0.3f;
                FloatingTextRequested?.Invoke(Player.X - offsetX, Player.Y, e.Text, e.Icon);
            }
            foreach (var e in Field.Log.Get(GameEventType.KeyCollected, GameEventType.ItemPickedUp,
                GameEventType.ItemEquipped, GameEventType.NoTarget))
                LogRequested?.Invoke(e.Text, e.Color);
            HandleWorldState();
            UpdateUI();
        }
        private bool IsInputBlocked() =>
    IsExitPopupOpen || IsGameOverPopupOpen || IsDescendingPopupOpen ||
    IsAltarPopupOpen || IsAltarResultOpen || IsAttackPopupOpen || !Player.IsAlive || IsAimingMode;

        private InteractionResult HandlePlayerTurn(int dx, int dy)
        {
            CloseAllDoors();
            _hasUnsavedProgress = true;
            return Player.Move(dx, dy, Field);
        }

        private void CloseAllDoors()
        {
            for (int y = 0; y < Field.Height; y++)
                for (int x = 0; x < Field.Width; x++)
                    if (Field[x, y] is Door door)
                        door.Close();
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
            UpdateHeartsIfChanged();
            OnPropertyChanged(nameof(EquippedMeleeText));
            OnPropertyChanged(nameof(HotbarSlots));
            OnPropertyChanged(nameof(ActiveSlot));
            OnPropertyChanged(nameof(ActiveItemStatsText));
            UpdateHotbarIfChanged();
            UpdateActiveSlotIfChanged();
        }
    }
}

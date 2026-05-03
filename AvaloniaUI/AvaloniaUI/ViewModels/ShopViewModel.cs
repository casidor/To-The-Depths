using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameCore.Models.Entities;
using GameCore.Models.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace AvaloniaUI.ViewModels
{
    public partial class ShopViewModel : ObservableObject
    {
        private readonly Player _player;
        public ShopViewModel(Player player)
        {
            _player = player;
            Refresh();
        }
        // Melee weapon properties
        public MeleeWeapon? Melee => _player.Inventory.EquippedMelee;
        public bool HasMelee => Melee != null;
        public string MeleeName => Melee?.Name ?? "—";
        public string MeleeStatLine => Melee?.StatLine ?? "No melee equipped";
        public bool CanUpgradeMelee => !Melee?.IsMaxUpgrade ?? false
                                       && _player.GoldCollected >= (Melee?.NextUpgradeCost ?? int.MaxValue);
        public string MeleeUpgradeCost => Melee?.IsMaxUpgrade == true
                                          ? "MAX" : $"{Melee?.NextUpgradeCost} ♦";
        // Shop slots
        public ShopSlot Slot1 { get; private set; } = null!;
        public ShopSlot Slot2 { get; private set; } = null!;
        public ShopSlot Slot3 { get; private set; } = null!;
        public ShopSlot Slot4 { get; private set; } = null!;

        public int Gold => _player.GoldCollected;

        public void Refresh()
        {
            var hotbar = _player.Inventory.Hotbar;
            Slot1 = new ShopSlot(hotbar[0], _player);
            Slot2 = new ShopSlot(hotbar[1], _player);
            Slot3 = new ShopSlot(hotbar[2], _player);
            Slot4 = new ShopSlot(hotbar[3], _player);
            OnPropertyChanged(nameof(Slot1));
            OnPropertyChanged(nameof(Slot2));
            OnPropertyChanged(nameof(Slot3));
            OnPropertyChanged(nameof(Slot4));
            OnPropertyChanged(nameof(Gold));
            OnPropertyChanged(nameof(CanUpgradeMelee));
            OnPropertyChanged(nameof(MeleeUpgradeCost));
            OnPropertyChanged(nameof(MeleeStatLine));
        }

        [RelayCommand]
        private void UpgradeMelee()
        {
            if (Melee == null || !_player.SpendGold(Melee.NextUpgradeCost)) return;
            Melee.TryUpgrade();
            Refresh();
        }

        [RelayCommand]
        private void Reload(ShopSlot? slot)
        {
            if (slot?.Item is RangedWeapon rw)
                rw.TryReload(_player);
            Refresh();
        }

        [RelayCommand]
        private void UpgradeDamage(ShopSlot? slot)
        {
            if (slot?.Item is Weapon w && _player.SpendGold(w.NextUpgradeCost))
                w.TryUpgrade();
            Refresh();
        }

        [RelayCommand]
        private void UpgradeAmmo(ShopSlot? slot)
        {
            if (slot?.Item is RangedWeapon rw)
                rw.TryUpgradeAmmo(_player);
            Refresh();
        }
    }

    public class ShopSlot
    {
        private readonly Player _player;
        public Item? Item { get; }
        public bool IsEmpty => Item == null;
        public string Name => Item?.Name ?? "Empty";
        public string StatLine => (Item as Weapon)?.StatLine ?? "—";
        public string AmmoText => Item is RangedWeapon rw ? $"{rw.Ammo}/{rw.MaxAmmo}" : "";
        public bool IsRanged => Item is RangedWeapon;
        public bool HasItem => Item != null;

        // Reload
        public bool CanReload => Item is RangedWeapon rw
                                 && rw.Ammo < rw.MaxAmmo
                                 && _player.GoldCollected >= rw.ReloadCost;
        public string ReloadCost => Item is RangedWeapon rw ? $"{rw.ReloadCost} ♦" : "";

        // Damage upgrade
        public bool CanUpgradeDamage => Item is Weapon w
                                        && !w.IsMaxUpgrade
                                        && _player.GoldCollected >= w.NextUpgradeCost;
        public string DamageCostText => Item is Weapon w
                                        ? (w.IsMaxUpgrade ? "MAX" : $"{w.NextUpgradeCost} ♦") : "";

        // Ammo upgrade
        public bool CanUpgradeAmmo => Item is RangedWeapon rw
                                      && !rw.IsMaxAmmoUpgrade
                                      && _player.GoldCollected >= rw.NextAmmoCost;
        public string AmmoCostText => Item is RangedWeapon rw
                                      ? (rw.IsMaxAmmoUpgrade ? "MAX" : $"{rw.NextAmmoCost} ♦") : "";

        public ShopSlot(Item? item, Player player)
        {
            Item = item;
            _player = player;
        }
    }
}

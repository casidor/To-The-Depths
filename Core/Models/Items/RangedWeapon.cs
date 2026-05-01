using GameCore.Interfaces.GameCore.Interfaces;
using GameCore.Models.Entities;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class RangedWeapon : Weapon, IEquippable
    {
        public int Range { get; protected set; }
        public int MaxAmmo { get; protected set; }
        public int Ammo { get; private set; }
        public bool HasAmmo => Ammo > 0;

        public override string StatLine =>
            $"{Name} [{Damage} dmg | {Range} range | {Ammo}/{MaxAmmo}]";

        public bool Equip(Player player, GameField field, int x, int y)
        {
            var hotbar = player.Inventory.Hotbar;
            for (int i = 0; i < hotbar.Length; i++)
            {
                if (hotbar[i] == null)
                {
                    Ammo = MaxAmmo;
                    hotbar[i] = this;
                    field[x, y] = new Floor();
                    return true;
                }
            }
            return false;
        }

        public bool TrySpendAmmo(Inventory inventory)
        {
            if (!HasAmmo) return false;
            Ammo--;
            if (Ammo == 0)
                for (int i = 0; i < inventory.Hotbar.Length; i++)
                    if (inventory.Hotbar[i] == this)
                        inventory.Hotbar[i] = null;
            return true;
        }
    }
}

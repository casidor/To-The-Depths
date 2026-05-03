using GameCore.Interfaces.GameCore.Interfaces;
using GameCore.Models.Entities;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class MeleeWeapon : Weapon, IEquippable
    {
        public bool Equip(Player player, GameField field, int x, int y)
        {
            if (player.Inventory.EquippedMelee != null &&
                player.Inventory.EquippedMelee.Damage > this.Damage)
                return false;
            var old = player.Inventory.EquippedMelee;
            player.Inventory.EquippedMelee = this;
            field[x, y] = old ?? (GameObject)new Floor();
            return true;
        }
    }
}

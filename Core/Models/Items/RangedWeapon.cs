using GameCore.Interfaces;
using GameCore.Interfaces.GameCore.Interfaces;
using GameCore.Models.Entities;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class RangedWeapon : Weapon, IUsable
    {
        public int Range { get; protected set; }
        public int MaxAmmo { get; protected set; }
        public int Ammo { get; protected set; }
        public bool HasAmmo => Ammo > 0;
        public bool IsEmpty => Ammo == 0;

        public override string StatLine =>
            $"{Name} [{Damage} dmg | {Range} range | {Ammo}/{MaxAmmo}]";

        public bool TrySpendAmmo()
        {
            if (!HasAmmo) return false;
            Ammo--;
            return true;
        }

        public UseResult Use(Player player, GameField field)
        {
            Enemy? closest = null;
            int minDist = int.MaxValue;

            for (int dy = -Range; dy <= Range; dy++)
                for (int dx = -Range; dx <= Range; dx++)
                {
                    int cx = player.X + dx;
                    int cy = player.Y + dy;
                    if (cx < 0 || cx >= field.Width || cy < 0 || cy >= field.Height) continue;
                    if (field.Fov[cx, cy] != ExplorationState.Visible) continue;
                    if (field.GetEntity(cx, cy) is Enemy enemy)
                    {
                        int dist = Math.Abs(dx) + Math.Abs(dy);
                        if (dist < minDist) { minDist = dist; closest = enemy; }
                    }
                }

            if (closest == null) return UseResult.Missed;

            if (!TrySpendAmmo()) return UseResult.Failed;
            if (IsEmpty) player.Inventory.RemoveFromHotbar(this);

            closest.Interact(player, field, closest.X, closest.Y);
            return UseResult.Hit;
        }
        public UseResult UseAt(Player player, GameField field, int x, int y)
        {
            int dx = Math.Abs(x - player.X);
            int dy = Math.Abs(y - player.Y);
            if (dx > Range || dy > Range) return UseResult.Failed;
            if (field.Fov[x, y] != ExplorationState.Visible) return UseResult.Failed;

            if (!TrySpendAmmo()) return UseResult.Failed;
            if (IsEmpty) player.Inventory.RemoveFromHotbar(this);

            if (field.GetEntity(x, y) is Enemy enemy)
            {
                enemy.Interact(player, field, x, y);
                return UseResult.Hit;
            }
            return UseResult.Missed;
        }
    }
}

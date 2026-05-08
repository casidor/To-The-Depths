using GameCore.Models.Entities;
using GameCore.Models.Objects;
using GameCore.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items.Weapons
{
    public class Hammer : RangedWeapon
    {
        public bool IsBreakMode { get; private set; } = false;
        public void ToggleMode() => IsBreakMode = !IsBreakMode;
        public override AimShape AimShape => IsBreakMode ? AimShape.LineBreak : AimShape.Square;

        public Hammer(int ammo)
        {
            Name = "Hammer";
            Description = "Breaks walls and hits enemies in a line.";
            Damage = 30;
            Range = 7;
            MaxAmmo = ammo;
            Ammo = MaxAmmo;
        }
        public override UseResult Use(Player player, GameField field)
        {
            if (IsBreakMode)
            {
                field.Log.Add(GameEventType.NoTarget, "Switch to attack mode!", ' ', color: LogColor.Bad);
                return UseResult.Failed;
            }
            return base.Use(player, field);
        }
        public override UseResult UseAt(Player player, GameField field, int targetX, int targetY)
        {
            if (Math.Abs(targetX - player.X) > Range || Math.Abs(targetY - player.Y) > Range)
                return UseResult.Failed;

            if (!TrySpendAmmo())
            {
                field.Log.Add(GameEventType.NoAmmo, "No ammo!", ' ', color: LogColor.Bad);
                return UseResult.Failed;
            }

            bool hitSomething = false;
            foreach (var (x, y) in GetAxisLine(player.X, player.Y, targetX, targetY).Skip(1))
            {
                if (x < 0 || x >= field.Width || y < 0 || y >= field.Height) break;
                if (field[x, y] is Wall)
                {
                    field[x, y] = new Floor();
                    hitSomething = true;
                }
            }

            field.Fov.Update(player.X, player.Y, field);

            if (!hitSomething)
                field.Log.Add(GameEventType.Missed, "Missed!", ' ', targetX, targetY, color: LogColor.Bad);

            return hitSomething ? UseResult.Hit : UseResult.Missed;
        }

        public static IEnumerable<(int x, int y)> GetAxisLine(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x1 > x0 ? 1 : -1;
            int sy = y1 > y0 ? 1 : -1;

            if (dx >= dy)
            {
                for (int x = x0; x != x1 + sx; x += sx)
                    yield return (x, y0);
            }
            else
            {
                for (int y = y0; y != y1 + sy; y += sy)
                    yield return (x0, y);
            }
        }
    }
}

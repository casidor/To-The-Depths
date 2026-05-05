using GameCore.Interfaces;
using GameCore.Models.Entities;
using GameCore.World;

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
            $"{Name} {Damage} DMG | {Range} range | {Ammo}/{MaxAmmo} ammo";

        public bool TrySpendAmmo()
        {
            if (!HasAmmo) return false;
            Ammo--;
            return true;
        }
        public int AmmoUpgradeLevel { get; private set; } = 0;
        public const int MaxAmmoUpgradeLevel = 2;

        public int ReloadCost => Config.RangedWeaponReloadCost;

        public int NextAmmoCost => AmmoUpgradeLevel switch
        {
            0 => Config.RangedWeaponAmmoUpgradeCostLevel1,
            1 => Config.RangedWeaponAmmoUpgradeCostLevel2,
            _ => int.MaxValue
        };

        public bool IsMaxAmmoUpgrade => AmmoUpgradeLevel >= MaxAmmoUpgradeLevel;

        public bool TryReload(Player player)
        {
            if (Ammo == MaxAmmo) return false;
            if (!player.SpendGold(ReloadCost)) return false;
            Ammo = MaxAmmo;
            return true;
        }

        public bool TryUpgradeAmmo(Player player)
        {
            if (IsMaxAmmoUpgrade) return false;
            if (!player.SpendGold(NextAmmoCost)) return false;
            AmmoUpgradeLevel++;
            MaxAmmo += Config.RangedWeaponAmmoUpgradeAmount;
            Ammo = MaxAmmo;
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

            if (closest == null)
            {
                field.Log.Add(GameEventType.NoTarget, "No targets in range!", ' ', color: LogColor.Bad);
                return UseResult.Missed;
            }

            if (!TrySpendAmmo())
            {
                field.Log.Add(GameEventType.NoAmmo, "No ammo!", ' ', color: LogColor.Bad);
                return UseResult.Failed;
            }

            closest.TakeDamage(Damage, player, field, closest.X, closest.Y);
            return UseResult.Hit;
        }

        public UseResult UseAt(Player player, GameField field, int x, int y)
        {
            int dx = Math.Abs(x - player.X);
            int dy = Math.Abs(y - player.Y);
            if (dx > Range || dy > Range) return UseResult.Failed;
            if (field.Fov[x, y] != ExplorationState.Visible) return UseResult.Failed;

            if (!TrySpendAmmo())
            {
                field.Log.Add(GameEventType.NoAmmo, "No ammo!", ' ', color: LogColor.Bad);
                return UseResult.Failed;
            }

            if (field.GetEntity(x, y) is Enemy enemy)
            {
                enemy.TakeDamage(Damage, player, field, x, y);
                return UseResult.Hit;
            }
            field.Log.Add(GameEventType.Missed, "Missed!", ' ', x, y, color: LogColor.Bad);
            return UseResult.Missed;
        }
        internal void RestoreAmmoState(int ammo, int maxAmmo, int ammoUpgradeLevel)
        {
            AmmoUpgradeLevel = ammoUpgradeLevel;
            MaxAmmo = maxAmmo;
            Ammo = Math.Min(ammo, maxAmmo);
        }
    }
}

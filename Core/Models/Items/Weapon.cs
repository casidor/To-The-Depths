using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class Weapon : Item
    {
        public int Damage { get; protected set; }
        public int UpgradeLevel { get; private set; } = 0;
        public const int MaxUpgradeLevel = 2;
        public virtual string StatLine => $"{Name} {Damage} dmg";

        public virtual int NextUpgradeCost => UpgradeLevel switch
        {
            0 => 120,
            1 => 220,
            _ => int.MaxValue
        };

        public virtual bool TryUpgrade()
        {
            if (UpgradeLevel >= MaxUpgradeLevel) return false;
            UpgradeLevel++;
            Damage += 10;
            return true;
        }

        public bool IsMaxUpgrade => UpgradeLevel >= MaxUpgradeLevel;
    }
}

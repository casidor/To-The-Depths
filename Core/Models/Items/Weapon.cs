namespace GameCore.Models.Items
{
    public abstract class Weapon : Item
    {
        public int Damage { get; protected set; }
        public int UpgradeLevel { get; private set; } = 0;
        public const int MaxUpgradeLevel = 2;
        public virtual string StatLine => $"{Name} {Damage} DMG";

        public virtual int NextUpgradeCost => UpgradeLevel switch
        {
            0 => Config.WeaponUpgradeCostLevel1,
            1 => Config.WeaponUpgradeCostLevel2,
            _ => int.MaxValue
        };

        public virtual bool TryUpgrade()
        {
            if (UpgradeLevel >= MaxUpgradeLevel) return false;
            UpgradeLevel++;
            Damage += Config.WeaponDamageUpgradeAmount;
            return true;
        }

        public bool IsMaxUpgrade => UpgradeLevel >= MaxUpgradeLevel;
        public int GetMaxPossibleDamage()
        {
            return Damage + (MaxUpgradeLevel - UpgradeLevel) * Config.WeaponDamageUpgradeAmount;
        }

        internal void RestoreState(int upgradeLevel)
        {
            while (UpgradeLevel < upgradeLevel && !IsMaxUpgrade)
                TryUpgrade();
        }
    }
}

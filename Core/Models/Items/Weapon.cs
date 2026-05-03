using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items
{
    public abstract class Weapon : Item
    {
        public int Damage { get; protected set; }
        public int UpgradeLevel { get; protected set; }
        public virtual string StatLine => $"{Name} [{Damage} dmg]";
    }
}

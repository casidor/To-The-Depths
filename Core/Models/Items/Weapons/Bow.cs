using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Models.Items.Weapons
{
    public class Bow : RangedWeapon
    {
        public Bow(int ammo)
        {
            Name = "Bow";
            Description = "Fires arrows at nearby enemies.";
            Damage = 20;
            Range = 6;
            MaxAmmo = ammo;
            Ammo = MaxAmmo;
        }
    }
}
